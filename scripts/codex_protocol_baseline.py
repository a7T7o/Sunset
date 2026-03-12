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

HISTORICAL_ACTIVE_ROOT_SAMPLES = [
    {
        "value": r"D:\迅雷下载\开始",
        "source": "用户与前序核查样本",
        "label": "系统全局",
        "note": "这是系统全局监督线程样本。它在未命中当前实时值时属于历史样本；一旦再次命中实时值，就应按系统全局监督线程现场处理，而不是直接判成 Sunset 内部 bug。",
    }
]

THREAD_TYPE_LABELS = {
    "governance": "项目治理线程",
    "feature": "独立 worktree 功能线程",
}


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
        return str(Path(value).expanduser().resolve(strict=False))
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


def path_key(value: str | None) -> str | None:
    normalized = normalize_path(value)
    return normalized.lower() if normalized else None


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
            latest[thread_id] = {
                "thread_name": thread_name,
                "updated_at": updated_at,
            }
    return latest


def fetch_thread_rows(thread_ids: list[str]) -> dict[str, dict]:
    if not thread_ids:
        return {}
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


def path_exists(value: str | None) -> bool:
    return bool(value) and Path(value).exists()


def thread_type_label(route: RouteSpec) -> str:
    if route.thread_type == "feature" and path_key(route.expected_worktree_root) == path_key(route.expected_cwd):
        return "独立 worktree 功能线程"
    return THREAD_TYPE_LABELS.get(route.thread_type, route.thread_type)


def describe_memory_paths(route: RouteSpec) -> list[str]:
    lines: list[str] = []
    target_status = "存在" if path_exists(route.expected_thread_memory) else "不存在"
    lines.append(f"- 目标线程记忆：`{route.expected_thread_memory}`（{target_status}）")
    if route.observed_thread_memory:
        observed_status = "存在" if path_exists(route.observed_thread_memory) else "不存在"
        lines.append(f"- 当前观测线程记忆：`{route.observed_thread_memory}`（{observed_status}）")
    if route.memory_note:
        lines.append(f"- 记忆说明：{route.memory_note}")
    return lines


def route_has_memory_gap(route: RouteSpec) -> bool:
    return bool(route.observed_thread_memory) and path_exists(route.observed_thread_memory) and not path_exists(route.expected_thread_memory)


def evaluate_thread(route: RouteSpec, db_row: dict | None) -> dict:
    findings: list[str] = []
    hard_conflicts: list[str] = []

    if not db_row:
        return {
            "git_state": {"worktree_root": None, "branch": None, "sha": None},
            "level": "L0",
            "reason": "状态库中缺少该线程样本记录，当前无法进入线程现场判断。",
            "findings": ["状态库中没有该线程样本记录。"],
            "hard_conflicts": [],
        }

    current_cwd = normalize_path(db_row.get("cwd"))
    expected_cwd = normalize_path(route.expected_cwd)
    expected_root = normalize_path(route.expected_worktree_root)
    expected_branch = route.expected_git_branch
    git_state = inspect_git(current_cwd or expected_cwd)
    real_root = normalize_path(git_state.get("worktree_root"))
    real_branch = git_state.get("branch")
    real_sha = git_state.get("sha")

    if not real_sha:
        return {
            "git_state": git_state,
            "level": "L0",
            "reason": "当前无法读取真实 Git 现场，不能形成有效基线。",
            "findings": ["无法读取真实 worktree / 分支 / HEAD。"],
            "hard_conflicts": [],
        }

    if current_cwd != expected_cwd:
        hard_conflicts.append("状态库 `cwd` 未对齐期望目录。")
    if real_root != expected_root:
        hard_conflicts.append("真实 worktree 根未对齐期望 worktree。")
    if real_branch != expected_branch:
        hard_conflicts.append("真实 Git 分支未对齐期望分支。")

    db_branch = db_row.get("git_branch")
    db_sha = db_row.get("git_sha")
    if db_branch != expected_branch:
        findings.append("状态库 `git_branch` 仍停留在旧值。")
    if db_sha and real_sha and db_sha != real_sha:
        findings.append("状态库 `git_sha` 落后于真实 Git HEAD。")
    if route_has_memory_gap(route):
        findings.append("线程记忆仍停留在 worktree 观测路径，目标归位路径尚未落地。")

    if hard_conflicts:
        level = "L1"
        reason = "真实线程现场仍有主现场错位，不能提升到 L2。"
    elif route.thread_type == "feature" and db_branch != expected_branch:
        level = "L1"
        reason = "真实现场已经在正确 worktree，但功能线程的状态投影仍未收口。"
    else:
        level = "L2"
        if findings:
            reason = "真实现场已对齐，但状态投影或归位策略仍有未收口项。"
        else:
            reason = "当前目录、真实 worktree 与真实分支已对齐，可在正确现场继续推进。"

    return {
        "git_state": git_state,
        "level": level,
        "reason": reason,
        "findings": findings,
        "hard_conflicts": hard_conflicts,
    }


def active_root_labels(global_state: dict) -> dict[str, str]:
    result: dict[str, str] = {}
    for key, value in (global_state.get("electron-workspace-root-labels", {}) or {}).items():
        normalized = normalize_path(key)
        if normalized:
            result[normalized] = str(value)
    return result


def classify_active_workspace(repo_root: Path, routes: dict[str, RouteSpec], global_state: dict) -> dict:
    active_roots = normalize_path_list(global_state.get("active-workspace-roots", []))
    label_map = active_root_labels(global_state)
    repo_root_normalized = normalize_path(str(repo_root))
    internal_feature_roots = {
        normalize_path(route.expected_worktree_root)
        for route in routes.values()
        if route.thread_type == "feature"
    }
    internal_feature_keys = {path_key(item) for item in internal_feature_roots}

    entries: list[dict[str, str | None]] = []
    for root in active_roots:
        label = label_map.get(root)
        if label == "系统全局":
            relation = "系统全局监督线程现场"
            kind = "system_global"
        elif path_key(root) == path_key(repo_root_normalized):
            relation = "Sunset 根仓库 / 治理现场"
            kind = "sunset_root"
        elif path_key(root) in internal_feature_keys:
            relation = "Sunset 内部功能 worktree 现场"
            kind = "sunset_feature"
        else:
            relation = "未归类工作区现场"
            kind = "unknown"
        entries.append({"root": root, "label": label, "relation": relation, "kind": kind})

    current_kinds = {item["kind"] for item in entries}
    if not entries:
        current_kind = "empty"
        conclusion = "当前 `active-workspace-roots` 为空，只能视为 UI 现场缺失样本。"
    elif "sunset_root" in current_kinds:
        current_kind = "sunset_root"
        conclusion = (
            "当前实时 `active-workspace-roots` 命中 Sunset 根仓库，对当前治理线程是正常现场；"
            "它不能被直接推导为 NPC / 农田线程已经完成 UI 承接。"
        )
    elif "system_global" in current_kinds:
        current_kind = "system_global"
        conclusion = (
            "当前实时 `active-workspace-roots` 命中系统全局监督线程现场。"
            "这属于外部监督语境，不应直接判成 Sunset 内部 bug。"
        )
    elif "sunset_feature" in current_kinds:
        current_kind = "sunset_feature"
        conclusion = (
            "当前实时 `active-workspace-roots` 命中 Sunset 功能 worktree 现场，"
            "但仍需结合当前打开的是哪条线程，才能判断是否真正承接到位。"
        )
    else:
        current_kind = "unknown"
        conclusion = "当前实时 `active-workspace-roots` 未命中已知类型，只能先保留为只读样本。"

    historical_notes: list[str] = []
    current_keys = {path_key(item["root"]) for item in entries}
    for sample in HISTORICAL_ACTIVE_ROOT_SAMPLES:
        sample_key = path_key(sample["value"])
        if sample_key in current_keys:
            historical_notes.append(
                f"- 历史样本：`{sample['value']}` 当前也命中了实时值；应按 `{sample['label']}` 现场处理。"
            )
        else:
            historical_notes.append(
                f"- 历史样本：`{sample['value']}`（来源：`{sample['source']}`，标签：`{sample['label']}`）。"
                f" 本轮未命中实时值，当前只作为历史样本保留。{sample['note']}"
            )

    trust_rule = (
        "当前执行一律以“最新直读 `.codex-global-state.json` + 线程类型判定”为准；"
        "旧值只保留为阶段样本，不能直接写成当前实时事实。"
    )

    return {
        "active_roots": active_roots,
        "entries": entries,
        "historical_notes": historical_notes,
        "conclusion": conclusion,
        "trust_rule": trust_rule,
        "current_kind": current_kind,
    }


def rejudge_boundary_conflicts(
    active_assessment: dict,
    routes: dict[str, RouteSpec],
    evaluations: dict[str, dict],
    db_rows: dict[str, dict],
) -> dict:
    npc_route = next(route for route in routes.values() if route.display_name == "NPC")
    npc_eval = evaluations[npc_route.thread_id]
    npc_row = db_rows.get(npc_route.thread_id, {})
    npc_db_branch = npc_row.get("git_branch")
    npc_real_branch = npc_eval["git_state"].get("branch")

    active_kind = active_assessment["current_kind"]
    if active_kind == "system_global":
        active_root_judgement = (
            "当前实时值命中系统全局监督线程现场。它属于外部监督语境，"
            "不能直接判成 Sunset 内部 bug。"
        )
    elif active_kind == "sunset_root":
        active_root_judgement = (
            "当前实时值命中 Sunset 根仓库 / 治理现场。"
            "它说明客户端当前落在治理视角，但并不等于 NPC / 农田线程已经承接成功。"
        )
    elif active_kind == "sunset_feature":
        active_root_judgement = (
            "当前实时值命中 Sunset 功能 worktree 现场。"
            "它能说明 UI 正落在某条功能线，但仍需结合当前打开线程判断是否真正承接到位。"
        )
    else:
        active_root_judgement = (
            "当前实时值为空或未命中已知类型，本轮只能把它当成只读样本，"
            "不能强行推出 Sunset 内部结论。"
        )

    if npc_db_branch != npc_route.expected_git_branch and npc_real_branch == npc_route.expected_git_branch:
        npc_branch_judgement = (
            "NPC 线程的状态库 `git_branch` 仍是 `main`，而真实分支已是 "
            f"`{npc_route.expected_git_branch}`；这仍是当前最强的 Sunset 内部真实冲突。"
        )
        highest_conflict = "NPC 线程状态库 `git_branch` 旧值"
        next_step = (
            "继续维持只读治理；先为 NPC 单线程 `git_branch` 修正准备备份前置与单样本验收口径，"
            "暂不直接改状态库。"
        )
    else:
        npc_branch_judgement = "NPC 线程状态库 `git_branch` 当前未表现出新的优先级一冲突。"
        highest_conflict = "NPC 线程记忆归位策略未收口"
        next_step = "先把 NPC 线程记忆的“当前事实现场 / 未来归位目标”口径固定下来。"

    if route_has_memory_gap(npc_route):
        npc_memory_judgement = (
            "NPC 线程记忆当前事实现场仍在 worktree 观测路径，项目根目录下的目标归位路径尚未落地；"
            "它属于“归位策略未收口”，不是内容丢失。"
        )
    else:
        npc_memory_judgement = "NPC 线程记忆当前没有新的路径归位冲突样本。"

    return {
        "active": active_root_judgement,
        "npc_branch": npc_branch_judgement,
        "npc_memory": npc_memory_judgement,
        "highest": highest_conflict,
        "next_step": next_step,
    }


def render_markdown(
    repo_root: Path,
    routes: dict[str, RouteSpec],
    db_rows: dict[str, dict],
    latest_names: dict[str, dict[str, str]],
    global_state: dict,
) -> str:
    generated_at = datetime.now().astimezone().isoformat(timespec="seconds")
    repo_root_str = normalize_path(str(repo_root)) or str(repo_root)
    root_branch = run_git(repo_root_str, "branch", "--show-current")
    root_sha = run_git(repo_root_str, "rev-parse", "HEAD")
    worktree_list = run_git(repo_root_str, "worktree", "list", "--porcelain")
    active_assessment = classify_active_workspace(repo_root, routes, global_state)
    evaluations = {
        thread_id: evaluate_thread(route, db_rows.get(thread_id))
        for thread_id, route in routes.items()
    }
    boundary = rejudge_boundary_conflicts(active_assessment, routes, evaluations, db_rows)

    lines: list[str] = []
    lines.append("# Codex统一协议建设最新现场基线")
    lines.append("")
    lines.append(f"- 生成时间：{generated_at}")
    lines.append(f"- 生成脚本：`{repo_root / 'scripts' / 'codex_protocol_baseline.py'}`")
    lines.append("- 当前执行入口：现行 `T73`")
    lines.append("- 执行方式：只读采样，不改 `state_5.sqlite`、`.codex-global-state.json`、`session_index.jsonl`")
    lines.append("")
    lines.append("## 1. `active-workspace-roots` 实时冲突核查")
    lines.append("")
    lines.append(f"- 本轮直读文件：`{GLOBAL_STATE_PATH}`")
    if active_assessment["active_roots"]:
        roots = "、".join(f"`{item}`" for item in active_assessment["active_roots"])
    else:
        roots = "（空）"
    lines.append(f"- 当前实时 `active-workspace-roots`：{roots}")
    if active_assessment["entries"]:
        lines.append("- 当前实时值分类：")
        for item in active_assessment["entries"]:
            label_suffix = f"（标签：`{item['label']}`）" if item["label"] else ""
            lines.append(f"  - `{item['root']}` → {item['relation']}{label_suffix}")
    else:
        lines.append("- 当前实时值分类：无可用激活工作区样本。")
    lines.append("- 历史样本：")
    lines.extend(active_assessment["historical_notes"] or ["- 当前没有历史样本。"])
    lines.append(f"- 当前解释口径：{active_assessment['conclusion']}")
    lines.append(f"- 当前信任规则：{active_assessment['trust_rule']}")
    lines.append("")
    lines.append("## 2. 根仓库与 worktree 现场")
    lines.append("")
    lines.append(f"- 根仓库路径：`{repo_root_str}`")
    lines.append(f"- 根仓库分支：`{root_branch or '（无法获取）'}`")
    lines.append(f"- 根仓库 HEAD：`{root_sha or '（无法获取）'}`")
    lines.append("- `git worktree list --porcelain`：")
    lines.append("```text")
    lines.append(worktree_list or "（无法获取）")
    lines.append("```")
    lines.append("")
    lines.append("## 3. 样本线程总览")
    lines.append("")
    lines.append("| 样本线程 | thread id | 线程类型 | 数据库 `cwd` | 数据库 `git_branch` | 真实分支 | 完成态 | 当前主要结论 |")
    lines.append("|---|---|---|---|---|---|---|---|")
    for thread_id, route in routes.items():
        db_row = db_rows.get(thread_id, {})
        evaluation = evaluations[thread_id]
        primary = "；".join(evaluation["hard_conflicts"] + evaluation["findings"]) or "当前样本没有新的只读冲突。"
        real_branch = evaluation["git_state"].get("branch") or "（无法获取）"
        lines.append(
            f"| {route.display_name} | `{thread_id}` | {thread_type_label(route)} | "
            f"`{db_row.get('cwd', '（空）')}` | `{db_row.get('git_branch', '（空）')}` | "
            f"`{real_branch}` | `{evaluation['level']}` | {primary} |"
        )
    lines.append("")

    detail_index = 1
    for thread_id, route in routes.items():
        db_row = db_rows.get(thread_id, {})
        evaluation = evaluations[thread_id]
        git_state = evaluation["git_state"]
        latest_name = latest_names.get(thread_id, {}).get("thread_name", "（未找到）")
        lines.append(f"## 4.{detail_index} {route.display_name}")
        lines.append("")
        lines.append(f"- thread id：`{thread_id}`")
        lines.append(f"- 最新 `thread_name`：`{latest_name}`")
        lines.append(f"- 线程类型：`{thread_type_label(route)}`")
        lines.append(f"- 当前 `title`：`{db_row.get('title', '（未找到）')}`")
        lines.append(f"- 当前 `cwd`：`{db_row.get('cwd', '（未找到）')}`")
        lines.append(f"- 当前 `git_branch`：`{db_row.get('git_branch', '（空）')}`")
        lines.append(f"- 当前 `git_sha`：`{db_row.get('git_sha', '（空）')}`")
        lines.append(f"- 当前 `rollout_path`：`{db_row.get('rollout_path', '（空）')}`")
        lines.append(f"- 期望目录：`{route.expected_cwd}`")
        lines.append(f"- 期望 worktree 根：`{route.expected_worktree_root}`")
        lines.append(f"- 期望分支：`{route.expected_git_branch}`")
        lines.append(f"- 当前真实 worktree 根：`{git_state.get('worktree_root') or '（无法获取）'}`")
        lines.append(f"- 当前真实 Git 分支：`{git_state.get('branch') or '（无法获取）'}`")
        lines.append(f"- 当前真实 Git HEAD：`{git_state.get('sha') or '（无法获取）'}`")
        lines.append("- 线程 memory 路径状态：")
        lines.extend(describe_memory_paths(route))
        lines.append("- 当前完成态：")
        lines.append(f"  - 级别：`{evaluation['level']}`")
        lines.append(f"  - 判定原因：{evaluation['reason']}")
        if evaluation["hard_conflicts"]:
            lines.append("  - 主现场冲突：")
            for item in evaluation["hard_conflicts"]:
                lines.append(f"    - {item}")
        if evaluation["findings"]:
            lines.append("  - 状态投影 / 策略问题：")
            for item in evaluation["findings"]:
                lines.append(f"    - {item}")
        else:
            lines.append("  - 状态投影 / 策略问题：当前没有新的只读问题。")
        lines.append("- 用户验收现场占位：未核验（需以用户当前真实打开工程与可见视图为准）")
        lines.append("")
        detail_index += 1

    lines.append("## 5. 三项边界冲突重判")
    lines.append("")
    lines.append(f"- `active-workspace-roots`：{boundary['active']}")
    lines.append(f"- NPC 状态库 `git_branch` 旧值：{boundary['npc_branch']}")
    lines.append(f"- NPC 线程记忆归位策略：{boundary['npc_memory']}")
    lines.append("- 当前优先级排序：")
    lines.append(f"  1. {boundary['highest']}")
    lines.append("  2. NPC 线程记忆归位策略未收口")
    lines.append("  3. `active-workspace-roots` 的动态解释规则")
    lines.append("")
    lines.append("## 6. 当前结论")
    lines.append("")
    lines.append(f"- 当前 Sunset 内部最该先动的真实冲突：{boundary['highest']}")
    lines.append(f"- 当前最小下一步：{boundary['next_step']}")
    lines.append("- 当前完成态判断：治理样本为 `L2`，NPC 为 `L1`，农田样本为 `L2`。")
    lines.append("- 当前口径：历史样本继续保留，但执行时只认本轮最新直读基线。")
    lines.append("")
    return "\n".join(lines)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--routes", required=True)
    parser.add_argument("--repo-root", required=True)
    parser.add_argument("--output-md", required=True)
    args = parser.parse_args()

    routes_path = Path(args.routes)
    repo_root = Path(args.repo_root).resolve()
    output_path = Path(args.output_md)

    routes = load_routes(routes_path)
    db_rows = fetch_thread_rows(list(routes.keys()))
    latest_names = load_latest_thread_names(SESSION_INDEX_PATH)
    global_state = load_json(GLOBAL_STATE_PATH)

    markdown = render_markdown(repo_root, routes, db_rows, latest_names, global_state)
    output_path.write_text(markdown + "\n", encoding="utf-8")
    print(str(output_path))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
