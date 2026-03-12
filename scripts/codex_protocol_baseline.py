from __future__ import annotations

import argparse
import json
import sqlite3
import subprocess
from dataclasses import dataclass
from datetime import datetime
from pathlib import Path


CODEX_HOME = Path.home() / ".codex"
STATE_DB_PATH = CODEX_HOME / "state_5.sqlite"
SESSION_INDEX_PATH = CODEX_HOME / "session_index.jsonl"
GLOBAL_STATE_PATH = CODEX_HOME / ".codex-global-state.json"


@dataclass
class RouteSpec:
    thread_id: str
    display_name: str
    thread_type: str
    expected_cwd: str
    expected_worktree_root: str
    expected_git_branch: str
    expected_thread_memory: str
    observed_thread_memory: str | None = None
    memory_note: str | None = None


def normalize_path(value: str | None) -> str | None:
    if not value:
        return None
    try:
        return str(Path(value).resolve(strict=False))
    except OSError:
        return str(Path(value))


def normalize_path_list(values: list[str] | None) -> list[str]:
    if not values:
        return []
    result: list[str] = []
    for value in values:
        normalized = normalize_path(value)
        if normalized:
            result.append(normalized)
    return result


def load_json(path: Path) -> dict:
    return json.loads(path.read_text(encoding="utf-8"))


def load_routes(path: Path) -> dict[str, RouteSpec]:
    payload = load_json(path)
    routes: dict[str, RouteSpec] = {}
    for thread_id, route in payload.get("threads", {}).items():
        routes[thread_id] = RouteSpec(
            thread_id=thread_id,
            display_name=route["display_name"],
            thread_type=route["thread_type"],
            expected_cwd=route["expected_cwd"],
            expected_worktree_root=route["expected_worktree_root"],
            expected_git_branch=route["expected_git_branch"],
            expected_thread_memory=route["expected_thread_memory"],
            observed_thread_memory=route.get("observed_thread_memory"),
            memory_note=route.get("memory_note"),
        )
    return routes


def load_latest_thread_names(path: Path) -> dict[str, dict[str, str]]:
    latest: dict[str, dict[str, str]] = {}
    if not path.exists():
        return latest
    for line in path.read_text(encoding="utf-8").splitlines():
        if not line.strip():
            continue
        record = json.loads(line)
        thread_id = record.get("id")
        thread_name = record.get("thread_name")
        updated_at = str(record.get("updated_at", ""))
        if not thread_id or not thread_name:
            continue
        current = latest.get(thread_id)
        if current is None or updated_at >= current["updated_at"]:
            latest[thread_id] = {"thread_name": thread_name, "updated_at": updated_at}
    return latest


def fetch_thread_rows(thread_ids: list[str]) -> dict[str, dict]:
    conn = sqlite3.connect(STATE_DB_PATH)
    conn.row_factory = sqlite3.Row
    rows = conn.execute(
        """
        select id, title, cwd, git_branch, git_sha, rollout_path, updated_at
        from threads
        where id in ({placeholders})
        """.format(placeholders=",".join("?" for _ in thread_ids)),
        thread_ids,
    ).fetchall()
    conn.close()
    return {row["id"]: dict(row) for row in rows}


def run_git(cwd: str, *arguments: str) -> str | None:
    try:
        completed = subprocess.run(
            ["git", "-C", cwd, *arguments],
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
            check=False,
        )
    except OSError:
        return None

    if completed.returncode != 0:
        return None
    return completed.stdout.strip() or None


def inspect_git(cwd: str | None) -> dict[str, str | None]:
    normalized_cwd = normalize_path(cwd)
    if not normalized_cwd or not Path(normalized_cwd).exists():
        return {"worktree_root": None, "branch": None, "sha": None}
    return {
        "worktree_root": normalize_path(run_git(normalized_cwd, "rev-parse", "--show-toplevel")),
        "branch": run_git(normalized_cwd, "branch", "--show-current"),
        "sha": run_git(normalized_cwd, "rev-parse", "HEAD"),
    }


def short_sha(value: str | None) -> str:
    if not value:
        return "（空）"
    return value[:8]


def path_exists(path_value: str | None) -> bool:
    if not path_value:
        return False
    return Path(path_value).exists()


def describe_memory_paths(route: RouteSpec) -> list[tuple[str, str, bool]]:
    paths: list[tuple[str, str, bool]] = []
    if route.expected_thread_memory:
        paths.append(("目标线程记忆", route.expected_thread_memory, path_exists(route.expected_thread_memory)))
    if route.observed_thread_memory:
        paths.append(("当前观测线程记忆", route.observed_thread_memory, path_exists(route.observed_thread_memory)))
    return paths


def collect_conflicts(
    route: RouteSpec,
    db_row: dict,
    git_state: dict[str, str | None],
    active_workspace_roots: list[str],
) -> list[str]:
    conflicts: list[str] = []
    expected_cwd = normalize_path(route.expected_cwd)
    expected_root = normalize_path(route.expected_worktree_root)
    current_cwd = normalize_path(db_row.get("cwd"))
    db_branch = db_row.get("git_branch")
    db_sha = db_row.get("git_sha")
    real_root = normalize_path(git_state.get("worktree_root"))
    real_branch = git_state.get("branch")
    real_sha = git_state.get("sha")
    active_roots = normalize_path_list(active_workspace_roots)

    if current_cwd != expected_cwd:
        conflicts.append("数据库 `cwd` 未对齐期望目录")
    if real_root != expected_root:
        conflicts.append("真实 worktree 根未对齐期望 worktree")
    if real_branch != route.expected_git_branch:
        conflicts.append("真实 Git 分支未对齐期望分支")
    if db_branch != route.expected_git_branch:
        conflicts.append("数据库 `git_branch` 仍停留在旧值")
    if db_sha and real_sha and db_sha != real_sha:
        conflicts.append("数据库 `git_sha` 落后于真实 Git HEAD")
    if expected_root and expected_root not in active_roots:
        conflicts.append("UI 激活工作区根未命中该线程期望现场")

    memory_paths = describe_memory_paths(route)
    expected_exists = any(label == "目标线程记忆" and exists for label, _, exists in memory_paths)
    observed_exists = any(label == "当前观测线程记忆" and exists for label, _, exists in memory_paths)
    if route.observed_thread_memory and observed_exists and not expected_exists:
        conflicts.append("线程记忆仍停留在 worktree 内，尚未归位到项目统一路径")

    return conflicts


def evaluate_completion_level(
    route: RouteSpec,
    db_row: dict,
    git_state: dict[str, str | None],
    active_workspace_roots: list[str],
) -> tuple[str, str]:
    if not db_row:
        return ("L0", "只有路由规则存在，数据库样本线程记录缺失。")

    if not git_state.get("sha"):
        return ("L0", "数据库里有样本线程记录，但当前无法读取真实 Git 现场。")

    level = "L1"
    reason = "真实 Git 现场可读，内容已经具备可追溯基线。"

    conflicts = collect_conflicts(route, db_row, git_state, active_workspace_roots)
    hard_conflicts = [
        conflict
        for conflict in conflicts
        if conflict
        in {
            "数据库 `cwd` 未对齐期望目录",
            "真实 worktree 根未对齐期望 worktree",
            "真实 Git 分支未对齐期望分支",
            "数据库 `git_branch` 仍停留在旧值",
        }
    ]

    if not hard_conflicts:
        level = "L2"
        reason = "线程目录、worktree 与分支已经对齐，可在正确现场继续推进。"

    return (level, reason)


def explain_not_higher(level: str, conflicts: list[str]) -> str:
    if level == "L0":
        return "还没有稳定到 Git 记录态，不能进入线程现场态。"
    if level == "L1":
        if conflicts:
            return "线程现场仍有漂移项，尚不能提升到 L2；更无法自动宣称 L3/L4/L5。"
        return "虽已具备 Git 记录态，但本轮未核验线程现场与用户验收现场。"
    if level == "L2":
        return "用户当前打开工程与可见视图尚未核验，不能提升到 L3；文档同步与回滚口径也未收口到 L4/L5。"
    return "本轮脚本只做只读基线，不自动判断更高层级。"


def default_action(route: RouteSpec, conflicts: list[str], level: str) -> str:
    if level == "L0":
        return "先核验该样本线程是否仍在状态库中可读，再决定是否纳入后续治理。"
    if "数据库 `cwd` 未对齐期望目录" in conflicts or "真实 worktree 根未对齐期望 worktree" in conflicts:
        return "继续保持只读，先收口线程默认 `cwd` / worktree 路由口径。"
    if "数据库 `git_branch` 仍停留在旧值" in conflicts:
        return "继续保持只读，下一刀只论证状态投影层 `git_branch` 是否允许最小修正。"
    if "线程记忆仍停留在 worktree 内，尚未归位到项目统一路径" in conflicts:
        return "先补线程记忆归位策略或索引映射，不迁移历史文件。"
    if "UI 激活工作区根未命中该线程期望现场" in conflicts:
        return "把 UI 激活工作区根漂移列为下一刀重点，但暂不自动改写全局状态。"
    return "该样本线程已具备进入 T74 的基础，可用于下一轮真实验收验证。"


def render_conflict_summary(conflicts: list[str]) -> str:
    if not conflicts:
        return "无"
    return "；".join(conflicts)


def render_memory_paths(route: RouteSpec) -> list[str]:
    lines: list[str] = []
    for label, path_value, exists in describe_memory_paths(route):
        status = "存在" if exists else "不存在"
        lines.append(f"- {label}：`{path_value}`（{status}）")
    if route.memory_note:
        lines.append(f"- 记忆说明：{route.memory_note}")
    return lines


def render_protocol_markdown(
    repo_root: Path,
    routes: dict[str, RouteSpec],
    db_rows: dict[str, dict],
    latest_names: dict[str, dict[str, str]],
    global_state: dict,
) -> str:
    generated_at = datetime.now().astimezone().isoformat()
    repo_root_str = str(repo_root.resolve())
    root_branch = run_git(repo_root_str, "branch", "--show-current")
    root_sha = run_git(repo_root_str, "rev-parse", "HEAD")
    worktree_list = run_git(repo_root_str, "worktree", "list")
    active_workspace_roots = normalize_path_list(global_state.get("active-workspace-roots", []))
    saved_workspace_roots = normalize_path_list(global_state.get("electron-saved-workspace-roots", []))

    lines: list[str] = []
    lines.append("# Codex统一协议建设最新现场基线")
    lines.append("")
    lines.append(f"- 生成时间：{generated_at}")
    lines.append(f"- 生成脚本：`{repo_root / 'scripts' / 'codex_protocol_baseline.py'}`")
    lines.append("- 任务入口：现行 `T73`")
    lines.append("- 执行方式：只读采样，不改写 `state_5.sqlite`、`.codex-global-state.json` 或任何守护逻辑")
    lines.append(
        f"- 数据源：`{STATE_DB_PATH}`、`{SESSION_INDEX_PATH}`、`{GLOBAL_STATE_PATH}`、真实 Git worktree"
    )
    lines.append("")
    lines.append("## 根仓库现场")
    lines.append("")
    lines.append(f"- 根仓库路径：`{repo_root_str}`")
    lines.append(f"- 根仓库分支：`{root_branch or '（无法获取）'}`")
    lines.append(f"- 根仓库 HEAD：`{root_sha or '（无法获取）'}`")
    lines.append(
        f"- 当前 `active-workspace-roots`：`{', '.join(active_workspace_roots) if active_workspace_roots else '（空）'}`"
    )
    lines.append(f"- 已保存工作区根数量：{len(saved_workspace_roots)}")
    lines.append("")
    lines.append("## git worktree list")
    lines.append("")
    lines.append("```text")
    lines.append(worktree_list or "（无法获取）")
    lines.append("```")
    lines.append("")
    lines.append("## 样本线程汇总")
    lines.append("")
    lines.append(
        "| 样本线程 | thread id | 最新 thread_name | 数据库 `git_branch` | 真实 Git 分支 | 当前完成态 | 当前冲突摘要 | 当前默认建议动作 |"
    )
    lines.append("|---|---|---|---|---|---|---|---|")

    overall_conflicts: list[str] = []
    per_thread_details: list[str] = []

    for thread_id, route in routes.items():
        db_row = db_rows.get(thread_id, {})
        latest_name = latest_names.get(thread_id, {}).get("thread_name", "（未找到）")
        git_state = inspect_git(db_row.get("cwd"))
        conflicts = collect_conflicts(route, db_row, git_state, active_workspace_roots)
        completion_level, completion_reason = evaluate_completion_level(
            route, db_row, git_state, active_workspace_roots
        )
        next_action = default_action(route, conflicts, completion_level)

        overall_conflicts.extend(conflicts)
        lines.append(
            "| {display_name} | `{thread_id}` | `{thread_name}` | `{db_branch}` | `{real_branch}` | `{completion}` | {conflicts} | {action} |".format(
                display_name=route.display_name,
                thread_id=thread_id,
                thread_name=latest_name.replace("|", "/"),
                db_branch=str(db_row.get("git_branch", "（空）")).replace("|", "/"),
                real_branch=str(git_state.get("branch", "（无法获取）")).replace("|", "/"),
                completion=completion_level,
                conflicts=render_conflict_summary(conflicts).replace("|", "/"),
                action=next_action.replace("|", "/"),
            )
        )

        per_thread_details.append(f"## {route.display_name}")
        per_thread_details.append("")
        per_thread_details.append(f"- thread id：`{thread_id}`")
        per_thread_details.append(f"- 最新 `thread_name`：`{latest_name}`")
        per_thread_details.append(f"- 线程类型：`{route.thread_type}`")
        per_thread_details.append(f"- 当前 `title`：`{db_row.get('title', '（未找到）')}`")
        per_thread_details.append(f"- 当前 `cwd`：`{db_row.get('cwd', '（未找到）')}`")
        per_thread_details.append(f"- 当前 `git_branch`：`{db_row.get('git_branch', '（空）')}`")
        per_thread_details.append(f"- 当前 `git_sha`：`{db_row.get('git_sha', '（空）')}`")
        per_thread_details.append(f"- 当前 `rollout_path`：`{db_row.get('rollout_path', '（空）')}`")
        per_thread_details.append(f"- 期望目录：`{route.expected_cwd}`")
        per_thread_details.append(f"- 期望 worktree 根：`{route.expected_worktree_root}`")
        per_thread_details.append(f"- 期望分支：`{route.expected_git_branch}`")
        per_thread_details.append(f"- 当前真实 worktree 根：`{git_state.get('worktree_root', '（无法获取）')}`")
        per_thread_details.append(f"- 当前真实 Git 分支：`{git_state.get('branch', '（无法获取）')}`")
        per_thread_details.append(f"- 当前真实 Git HEAD：`{git_state.get('sha', '（无法获取）')}`")
        per_thread_details.append(
            f"- UI 激活工作区命中：`{'命中' if normalize_path(route.expected_worktree_root) in active_workspace_roots else '未命中'}`"
        )
        per_thread_details.append("- 线程 memory 路径状态：")
        per_thread_details.extend(render_memory_paths(route))
        per_thread_details.append("- 用户验收现场占位：未核验（需以用户当前真实打开工程与可见视图确认为准）")
        per_thread_details.append(f"- 当前完成态：`{completion_level}`")
        per_thread_details.append(f"- 完成态判定原因：{completion_reason}")
        per_thread_details.append(f"- 尚未提升原因：{explain_not_higher(completion_level, conflicts)}")
        per_thread_details.append(f"- 当前冲突摘要：{render_conflict_summary(conflicts)}")
        per_thread_details.append(f"- 当前默认建议动作：{next_action}")
        per_thread_details.append("")

    lines.append("")
    lines.append("## 全局判断")
    lines.append("")

    root_active_hit = repo_root_str in active_workspace_roots
    overall_biggest_conflict = (
        f"当前 `active-workspace-roots` 仍停在 `{', '.join(active_workspace_roots)}`，没有命中 `Sunset` 根仓库或其功能 worktree。"
        if not root_active_hit
        else "根仓库 UI 激活工作区已命中，当前更大的问题转到样本线程元数据漂移。"
    )

    lines.append(f"- 当前最大冲突点：{overall_biggest_conflict}")
    lines.append(
        "- 当前最小下一步：以这份基线为现行 `T73` 证据，继续只读收口“UI 激活工作区漂移 / NPC `git_branch` 漂移 / NPC 线程记忆归位策略”三项边界，不提前进入自动纠偏。"
    )
    lines.append(
        "- 当前执行口径：旧总方案中的历史值继续保留为阶段快照；后续任何动作都以本文件代表的当前执行基线为准。"
    )
    lines.append("")

    lines.extend(per_thread_details)
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--routes", required=True)
    parser.add_argument("--repo-root", required=True)
    parser.add_argument("--output-md", required=True)
    args = parser.parse_args()

    routes_path = Path(args.routes)
    repo_root = Path(args.repo_root)
    output_path = Path(args.output_md)

    routes = load_routes(routes_path)
    db_rows = fetch_thread_rows(list(routes.keys()))
    latest_names = load_latest_thread_names(SESSION_INDEX_PATH)
    global_state = load_json(GLOBAL_STATE_PATH)

    markdown = render_protocol_markdown(repo_root, routes, db_rows, latest_names, global_state)
    output_path.write_text(markdown, encoding="utf-8")
    print(str(output_path))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
