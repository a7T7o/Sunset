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
    autofix_policy: dict[str, bool]


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
            autofix_policy=route["autofix_policy"],
        )
    return routes


def load_latest_thread_names(path: Path) -> dict[str, dict[str, str]]:
    latest: dict[str, dict[str, str]] = {}
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


def render_autofix_policy(policy: dict[str, bool]) -> str:
    enabled = [key for key, value in policy.items() if value]
    return "、".join(enabled) if enabled else "无"


def build_deviation_reasons(route: RouteSpec, db_row: dict, git_state: dict[str, str | None]) -> list[str]:
    reasons: list[str] = []

    expected_cwd = normalize_path(route.expected_cwd)
    expected_worktree_root = normalize_path(route.expected_worktree_root)
    current_cwd = normalize_path(db_row.get("cwd"))
    real_worktree_root = normalize_path(git_state.get("worktree_root"))
    real_branch = git_state.get("branch")
    db_branch = db_row.get("git_branch")
    db_git_sha = db_row.get("git_sha")
    real_git_sha = git_state.get("sha")

    if current_cwd != expected_cwd:
        reasons.append("数据库 `cwd` 与期望目录不一致")
    if real_worktree_root != expected_worktree_root:
        reasons.append("真实 worktree 根与期望不一致")
    if real_branch != route.expected_git_branch:
        reasons.append("真实 Git 分支与期望不一致")
    if db_branch != route.expected_git_branch:
        reasons.append("数据库 `git_branch` 与期望分支不一致")
    if db_git_sha and real_git_sha and db_git_sha != real_git_sha:
        reasons.append("数据库 `git_sha` 与真实 Git HEAD 不一致")

    return reasons


def classify_thread(route: RouteSpec, db_row: dict, git_state: dict[str, str | None]) -> str:
    expected_cwd = normalize_path(route.expected_cwd)
    expected_worktree_root = normalize_path(route.expected_worktree_root)
    current_cwd = normalize_path(db_row.get("cwd"))
    real_worktree_root = normalize_path(git_state.get("worktree_root"))
    real_branch = git_state.get("branch")
    db_branch = db_row.get("git_branch")
    db_git_sha = db_row.get("git_sha")
    real_git_sha = git_state.get("sha")

    base_aligned = (
        current_cwd == expected_cwd
        and real_worktree_root == expected_worktree_root
        and real_branch == route.expected_git_branch
    )

    sha_aligned = (not db_git_sha) or (not real_git_sha) or db_git_sha == real_git_sha
    branch_aligned = db_branch == route.expected_git_branch

    if base_aligned and branch_aligned and sha_aligned:
        return "完全一致"
    if base_aligned and (not branch_aligned or not sha_aligned):
        return "半迁移"
    return "明显漂移"


def describe_active_workspace(route: RouteSpec, active_workspace_roots: list[str]) -> str:
    normalized_active_roots = normalize_path_list(active_workspace_roots)
    expected_root = normalize_path(route.expected_worktree_root)

    if not normalized_active_roots:
        return "当前全局激活工作区根为空"
    if expected_root and expected_root in normalized_active_roots:
        return "命中"
    return "未命中"


def render_markdown(
    routes: dict[str, RouteSpec],
    db_rows: dict[str, dict],
    latest_names: dict[str, dict[str, str]],
    global_state: dict,
) -> str:
    active_workspace_roots = global_state.get("active-workspace-roots", [])
    saved_workspace_roots = global_state.get("electron-saved-workspace-roots", [])

    lines: list[str] = []
    lines.append("# Sunset线程承接审计表")
    lines.append("")
    lines.append(f"- 生成时间：{datetime.now().astimezone().isoformat()}")
    lines.append(
        f"- 数据源：`{STATE_DB_PATH}`、`{SESSION_INDEX_PATH}`、`{GLOBAL_STATE_PATH}`、真实 Git worktree"
    )
    lines.append(
        f"- 当前全局激活工作区根：`{', '.join(active_workspace_roots) if active_workspace_roots else '（空）'}`"
    )
    lines.append(f"- 已保存工作区根数量：{len(saved_workspace_roots)}")
    lines.append("")
    lines.append("## 汇总表")
    lines.append("")
    lines.append(
        "| 线程 | thread id | 最新 thread_name | 当前 cwd | 数据库 git_branch | 真实 Git 分支 | 一致性分类 | 激活根命中 | 允许自动纠偏 |"
    )
    lines.append("|---|---|---|---|---|---|---|---|---|")

    detail_blocks: list[str] = []

    for thread_id, route in routes.items():
        db_row = db_rows.get(thread_id, {})
        latest_name = latest_names.get(thread_id, {}).get("thread_name", "（未找到）")
        git_state = inspect_git(db_row.get("cwd"))
        deviation_reasons = build_deviation_reasons(route, db_row, git_state)
        classification = classify_thread(route, db_row, git_state)
        active_workspace_status = describe_active_workspace(route, active_workspace_roots)

        lines.append(
            "| {display_name} | `{thread_id}` | `{thread_name}` | `{cwd}` | `{db_branch}` | `{real_branch}` | {classification} | {active_workspace_status} | {autofix} |".format(
                display_name=route.display_name,
                thread_id=thread_id,
                thread_name=latest_name.replace("|", "/"),
                cwd=str(db_row.get("cwd", "（未找到）")).replace("|", "/"),
                db_branch=str(db_row.get("git_branch", "（空）")).replace("|", "/"),
                real_branch=str(git_state.get("branch", "（无法获取）")).replace("|", "/"),
                classification=classification,
                active_workspace_status=active_workspace_status,
                autofix=render_autofix_policy(route.autofix_policy),
            )
        )

        detail_blocks.append(f"## {route.display_name}")
        detail_blocks.append("")
        detail_blocks.append(f"- thread id：`{thread_id}`")
        detail_blocks.append(f"- 最新 `thread_name`：`{latest_name}`")
        detail_blocks.append(f"- 当前 `title`：`{db_row.get('title', '（未找到）')}`")
        detail_blocks.append(f"- 当前 `cwd`：`{db_row.get('cwd', '（未找到）')}`")
        detail_blocks.append(f"- 当前 `git_branch`：`{db_row.get('git_branch', '（空）')}`")
        detail_blocks.append(f"- 当前 `git_sha`：`{db_row.get('git_sha', '（空）')}`")
        detail_blocks.append(f"- 当前 `rollout_path`：`{db_row.get('rollout_path', '（空）')}`")
        detail_blocks.append(f"- 当前真实 worktree 根：`{git_state.get('worktree_root', '（无法获取）')}`")
        detail_blocks.append(f"- 当前真实 Git 分支：`{git_state.get('branch', '（无法获取）')}`")
        detail_blocks.append(f"- 当前真实 Git HEAD：`{git_state.get('sha', '（无法获取）')}`")
        detail_blocks.append(f"- 期望目录：`{route.expected_cwd}`")
        detail_blocks.append(f"- 期望 worktree 根：`{route.expected_worktree_root}`")
        detail_blocks.append(f"- 期望分支：`{route.expected_git_branch}`")
        detail_blocks.append(f"- 期望线程记忆：`{route.expected_thread_memory}`")
        detail_blocks.append(f"- 一致性分类：{classification}")
        detail_blocks.append(f"- 当前全局激活工作区根命中：{active_workspace_status}")
        detail_blocks.append(f"- 允许自动纠偏：{render_autofix_policy(route.autofix_policy)}")

        if deviation_reasons:
            detail_blocks.append("- 偏差原因：")
            for reason in deviation_reasons:
                detail_blocks.append(f"  - {reason}")
        else:
            detail_blocks.append("- 偏差原因：无")

        if active_workspace_roots:
            detail_blocks.append(
                f"- 当前全局激活工作区根原值：`{', '.join(active_workspace_roots)}`"
            )

        detail_blocks.append("")

    lines.append("")
    lines.extend(detail_blocks)
    return "\n".join(lines) + "\n"


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--routes", required=True)
    parser.add_argument("--output-md", required=True)
    args = parser.parse_args()

    routes_path = Path(args.routes)
    output_path = Path(args.output_md)

    routes = load_routes(routes_path)
    db_rows = fetch_thread_rows(list(routes.keys()))
    latest_names = load_latest_thread_names(SESSION_INDEX_PATH)
    global_state = load_json(GLOBAL_STATE_PATH)

    markdown = render_markdown(routes, db_rows, latest_names, global_state)
    output_path.write_text(markdown, encoding="utf-8")
    print(str(output_path))
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
