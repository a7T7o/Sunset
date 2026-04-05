#!/usr/bin/env python3
from __future__ import annotations

import argparse
import json
import re
import socket
import subprocess
import sys
import time
from pathlib import Path
from urllib import error as urllib_error
from urllib import request as urllib_request

try:
    import tomllib  # type: ignore[attr-defined]
except ModuleNotFoundError:  # pragma: no cover
    tomllib = None  # type: ignore[assignment]


DEFAULT_CODEX_HOME = Path(r"C:\Users\aTo\.codex")
DEFAULT_ENDPOINT = "http://127.0.0.1:8888/mcp"
DEFAULT_OWNER = "Sunset/UnityMCP转CLI"
MAX_TIMEOUT_SEC = 60
MAX_WAIT_SEC = 90
MAX_CONSOLE_COUNT = 200
MAX_OUTPUT_LIMIT = 20
MAX_PATHS = 12
DEFAULT_MANAGE_SCRIPT_DIR = "Assets/Scripts"
VALIDATION_LEVELS = ("basic", "standard", "comprehensive", "strict")


def result(command: str, success: bool, summary: str, data: object, reason: str | None = None) -> dict[str, object]:
    return {"command": command, "success": success, "summary": summary, "reason": reason, "data": data}


def write(payload: dict[str, object], code: int) -> None:
    sys.stdout.write(json.dumps(payload, ensure_ascii=False, indent=2) + "\n")
    raise SystemExit(code)


def norm(path: str) -> str:
    return path.replace("\\", "/").lstrip("./")


def rel(repo: Path, value: str | Path) -> str:
    path = Path(value)
    if not path.is_absolute():
        path = (repo / path).resolve()
    try:
        return norm(str(path.relative_to(repo.resolve())))
    except ValueError:
        return norm(str(path))


def run(args: list[str], cwd: Path, timeout: int = 300) -> subprocess.CompletedProcess[str]:
    try:
        return subprocess.run(args, cwd=str(cwd), text=True, capture_output=True, timeout=timeout, check=False)
    except subprocess.TimeoutExpired as exc:
        raise RuntimeError(f"subprocess_timeout:{args[0]}:{timeout}s") from exc


def clamp(value: int, lower: int, upper: int) -> tuple[int, bool]:
    bounded = max(lower, min(upper, value))
    return bounded, bounded != value


def build_guards(args: argparse.Namespace) -> dict[str, object]:
    timeout_sec, timeout_clamped = clamp(args.timeout_sec, 1, MAX_TIMEOUT_SEC)
    wait_sec, wait_clamped = clamp(args.wait_sec, 1, MAX_WAIT_SEC)
    count_input = getattr(args, "count", 10)
    console_count, count_clamped = clamp(count_input, 1, MAX_CONSOLE_COUNT)
    output_input = getattr(args, "output_limit", 12)
    output_limit, output_clamped = clamp(output_input, 1, MAX_OUTPUT_LIMIT)
    output_limit = min(output_limit, console_count)
    return {
        "requested_timeout_sec": args.timeout_sec,
        "timeout_sec": timeout_sec,
        "timeout_clamped": timeout_clamped,
        "requested_wait_sec": args.wait_sec,
        "wait_sec": wait_sec,
        "wait_clamped": wait_clamped,
        "requested_console_count": count_input,
        "console_count": console_count,
        "console_count_clamped": count_clamped,
        "requested_output_limit": output_input,
        "output_limit": output_limit,
        "output_limit_clamped": output_clamped or output_input != output_limit,
    }


def limit_entries(entries: list[dict[str, object]], output_limit: int) -> dict[str, object]:
    shown = entries[:output_limit]
    return {
        "entries": shown,
        "returned_count": len(entries),
        "shown_count": len(shown),
        "truncated_count": max(0, len(entries) - len(shown)),
        "output_limit": output_limit,
    }


def port_open(port: int) -> bool:
    try:
        with socket.create_connection(("127.0.0.1", port), timeout=0.5):
            return True
    except OSError:
        return False


def parse_config(path: Path) -> dict[str, object]:
    if not path.exists():
        return {"exists": False, "has_unityMCP_block": False, "has_legacy_mcp_unity_block": False, "configured_endpoint": None}
    raw = path.read_text(encoding="utf-8", errors="replace")
    endpoint = None
    if tomllib is not None:
        try:
            endpoint = tomllib.loads(raw).get("mcp_servers", {}).get("unityMCP", {}).get("url")
        except Exception:
            endpoint = None
    if endpoint is None:
        match = re.search(r'(?m)^url\s*=\s*"([^"]+)"\s*$', raw)
        endpoint = match.group(1) if match else None
    return {
        "exists": True,
        "has_unityMCP_block": bool(re.search(r"(?m)^\[mcp_servers\.unityMCP\]\s*$", raw)),
        "has_legacy_mcp_unity_block": bool(re.search(r"(?m)^\[mcp_servers\.mcp-unity\]\s*$", raw)),
        "configured_endpoint": endpoint,
    }


def baseline(repo: Path, codex_home: Path, endpoint: str) -> dict[str, object]:
    config = parse_config(codex_home / "config.toml")
    pid = repo / "Library" / "MCPForUnity" / "RunState" / "mcp_http_8888.pid"
    terminal = repo / "Library" / "MCPForUnity" / "TerminalScripts" / "mcp-terminal.cmd"
    pid_value = pid.read_text(encoding="utf-8", errors="replace").splitlines()[0].strip() if pid.exists() else None
    issues = []
    if not config["exists"]:
        issues.append("config_missing")
    if not config["has_unityMCP_block"]:
        issues.append("unityMCP_missing")
    if config["has_legacy_mcp_unity_block"]:
        issues.append("legacy_mcp_unity_present")
    if config["configured_endpoint"] != endpoint:
        issues.append("unexpected_endpoint")
    if not port_open(8888):
        issues.append("listener_missing")
    if not pid.exists():
        issues.append("pidfile_missing")
    return {
        "baseline_status": "pass" if not issues else "fail",
        "is_pass": not issues,
        "issues": issues,
        "config": config,
        "listener": {"active": port_open(8888), "port": 8888, "owning_process": pid_value},
        "pidfile": {"path": str(pid), "exists": pid.exists(), "value": pid_value},
        "terminal_script": {
            "path": str(terminal),
            "exists": terminal.exists(),
            "preview": terminal.read_text(encoding="utf-8", errors="replace").splitlines()[:5] if terminal.exists() else [],
        },
    }


class Mcp:
    def __init__(self, endpoint: str, timeout: int) -> None:
        self.endpoint = endpoint
        self.timeout = timeout
        self.headers = {"Accept": "application/json, text/event-stream", "Content-Type": "application/json"}
        self.id = 1
        self.init = self._rpc("initialize", {"protocolVersion": "2025-03-26", "capabilities": {}, "clientInfo": {"name": "sunset-mcp", "version": "0.1.0"}})

    def _messages(self, text: str) -> list[dict[str, object]]:
        text = text.strip()
        if text.startswith("{"):
            return [json.loads(text)]
        out, buf = [], []
        for line in text.splitlines():
            if line.startswith("data: "):
                buf.append(line[6:])
            elif not line.strip() and buf:
                out.append(json.loads("\n".join(buf)))
                buf = []
        if buf:
            out.append(json.loads("\n".join(buf)))
        return out

    def _post(self, payload: dict[str, object]) -> dict[str, object]:
        req = urllib_request.Request(self.endpoint, data=json.dumps(payload).encode("utf-8"), headers=self.headers, method="POST")
        try:
            with urllib_request.urlopen(req, timeout=self.timeout) as resp:
                session = resp.headers.get("mcp-session-id")
                if session:
                    self.headers["mcp-session-id"] = session
                messages = self._messages(resp.read().decode("utf-8"))
        except urllib_error.URLError as exc:
            raise RuntimeError(str(exc)) from exc
        for item in reversed(messages):
            if item.get("id") == payload["id"]:
                if item.get("error"):
                    raise RuntimeError(str(item["error"]))
                return item["result"]  # type: ignore[index]
        raise RuntimeError("No MCP result payload returned")

    def _rpc(self, method: str, params: dict[str, object] | None = None) -> dict[str, object]:
        payload: dict[str, object] = {"jsonrpc": "2.0", "id": self.id, "method": method}
        if params is not None:
            payload["params"] = params
        self.id += 1
        return self._post(payload)

    def resource(self, uri: str) -> dict[str, object]:
        payload = self._rpc("resources/read", {"uri": uri})
        text = ((payload.get("contents") or [{}])[0]).get("text")  # type: ignore[index]
        return json.loads(text) if isinstance(text, str) else payload

    def tool(self, name: str, arguments: dict[str, object] | None = None) -> dict[str, object]:
        params: dict[str, object] = {"name": name}
        if arguments:
            params["arguments"] = arguments
        payload = self._rpc("tools/call", params)
        structured = payload.get("structuredContent")
        if isinstance(structured, dict):
            return structured
        text = ((payload.get("content") or [{}])[0]).get("text")  # type: ignore[index]
        return json.loads(text) if isinstance(text, str) else payload

    def custom(self, tool_name: str, parameters: dict[str, object]) -> dict[str, object]:
        return self.tool("execute_custom_tool", {"tool_name": tool_name, "parameters": parameters})


def editor_state(mcp: Mcp) -> dict[str, object]:
    data = mcp.resource("mcpforunity://editor/state").get("data") or {}
    editor = data.get("editor") or {}
    play = editor.get("play_mode") or {}
    advice = data.get("advice") or {}
    comp = data.get("compilation") or {}
    scene = editor.get("active_scene") or {}
    return {
        "unity_instance": (data.get("unity") or {}).get("instance_id"),
        "active_scene": scene.get("path"),
        "is_playing": play.get("is_playing"),
        "is_paused": play.get("is_paused"),
        "is_changing": play.get("is_changing"),
        "is_compiling": comp.get("is_compiling"),
        "ready_for_tools": advice.get("ready_for_tools"),
        "blocking_reasons": advice.get("blocking_reasons") or [],
        "activity_phase": (data.get("activity") or {}).get("phase"),
        "age_ms": (data.get("staleness") or {}).get("age_ms"),
    }


def console(mcp: Mcp, count: int, warnings: bool) -> dict[str, object]:
    payload = mcp.tool("read_console", {"action": "get", "types": ["error", "warning"] if warnings else ["error"], "count": count, "format": "json", "include_stacktrace": False})
    entries = payload.get("data") or []
    errors = [x for x in entries if str(x.get("type", "")).lower() == "error"]
    warns = [x for x in entries if str(x.get("type", "")).lower() == "warning"]
    return {"entries": entries, "error_count": len(errors), "warning_count": len(warns)}


def manage_script_dir(path: str | None) -> str:
    raw = norm(path or DEFAULT_MANAGE_SCRIPT_DIR).strip("/")
    if raw.lower().endswith(".cs"):
        raise RuntimeError("manage_script 兼容参数里的 --path 应该是目录，不是 .cs 文件。")
    if not raw:
        return "Assets"
    if raw.lower() == "assets":
        return "Assets"
    if raw.lower().startswith("assets/"):
        return raw
    return f"Assets/{raw}"


def build_named_script_path(name: str | None, path: str | None) -> str | None:
    if not name and not path:
        return None
    if not name:
        raise RuntimeError("使用 manage_script 兼容参数时，必须同时提供 --name。")
    if not re.fullmatch(r"[A-Za-z_][A-Za-z0-9_]*", name):
        raise RuntimeError(f"Invalid script name: {name}")
    return norm(f"{manage_script_dir(path)}/{name}.cs")


def single_script_target(repo: Path, args: argparse.Namespace, fallback_paths: list[str] | None = None) -> dict[str, object]:
    cli_paths = list(fallback_paths) if fallback_paths is not None else [rel(repo, p) for p in getattr(args, "paths", [])]
    compat_path = build_named_script_path(getattr(args, "name", None), getattr(args, "path", None))
    if compat_path:
        if cli_paths and {rel(repo, p) for p in cli_paths} != {compat_path}:
            raise RuntimeError("请在显式脚本路径和 --name/--path 之间二选一，不要混用。")
        cli_paths = [compat_path]
    if not cli_paths:
        raise RuntimeError("请传入一个脚本路径，或使用 --name/--path。")
    if len(cli_paths) != 1:
        raise RuntimeError("manage_script 兼容入口一次只支持一个目标脚本。")
    asset_path = rel(repo, cli_paths[0])
    if not asset_path.lower().endswith(".cs"):
        raise RuntimeError(f"Expected a .cs target, got: {asset_path}")
    file_path = Path(asset_path)
    parent = norm(str(file_path.parent))
    if parent in {"", "."}:
        parent = "Assets"
    if not parent.lower().startswith("assets"):
        raise RuntimeError(f"manage_script 兼容入口只支持 Assets/ 下的脚本，当前目标为: {asset_path}")
    return {"asset_path": asset_path, "name": file_path.stem, "path": parent}


def validation_level(args: argparse.Namespace) -> str:
    level = getattr(args, "level", "standard").lower()
    if level not in VALIDATION_LEVELS:
        raise RuntimeError(f"Invalid validation level: {level}")
    return level


def resolve_code_paths(repo: Path, args: argparse.Namespace) -> list[str]:
    compat_path = build_named_script_path(getattr(args, "name", None), getattr(args, "path", None))
    cli_paths = getattr(args, "paths", [])
    if compat_path:
        if cli_paths:
            raise RuntimeError("请在显式脚本路径和 --name/--path 之间二选一，不要混用。")
        return [compat_path]
    return code_paths(repo, cli_paths, getattr(args, "all_changed", False))


def code_paths(repo: Path, paths: list[str], all_changed: bool) -> list[str]:
    if paths:
        return sorted({rel(repo, p) for p in paths})
    status = run(["git", "status", "--porcelain", "--untracked-files=all"], repo, timeout=120)
    if status.returncode != 0:
        raise RuntimeError(status.stderr.strip() or status.stdout.strip() or "git status failed")
    out = []
    for line in status.stdout.splitlines():
        if len(line) >= 4:
            path = line[3:].split(" -> ", 1)[-1]
            if path.lower().endswith(".cs"):
                out.append(norm(path))
    out = sorted(set(out))
    if not all_changed and len(out) > MAX_PATHS:
        raise RuntimeError(f"Too many changed .cs files in repo ({len(out)}). Pass explicit paths or use --all-changed.")
    return out


def codeguard(repo: Path, owner: str, paths: list[str]) -> dict[str, object]:
    if not paths:
        return {"Applies": False, "CanContinue": True, "Summary": "No target .cs paths.", "Reason": "no_target_paths", "Diagnostics": []}
    project = repo / "scripts" / "CodexCodeGuard" / "CodexCodeGuard.csproj"
    dll = repo / "scripts" / "CodexCodeGuard" / "bin" / "Release" / "net9.0" / "CodexCodeGuard.dll"
    if not dll.exists():
        build = run(["dotnet", "build", str(project), "-c", "Release", "--nologo"], repo, timeout=600)
        if build.returncode != 0:
            raise RuntimeError(build.stdout.strip() or build.stderr.strip() or "CodexCodeGuard build failed")
    args = ["dotnet", str(dll), "--repo-root", str(repo), "--phase", "pre-sync", "--owner-thread", owner, "--branch", "main"]
    for path in paths:
        args += ["--path", str((repo / path).resolve())]
    proc = run(args, repo, timeout=600)
    lines = [x for x in proc.stdout.splitlines() if x.strip()]
    if not lines:
        raise RuntimeError(proc.stderr.strip() or "CodexCodeGuard returned no JSON")
    return json.loads(lines[-1])


def codeguard_stats(report: dict[str, object]) -> dict[str, object]:
    diagnostics = report.get("Diagnostics") or []
    errors = [x for x in diagnostics if str(x.get("Severity", "")).lower() == "error"]
    warnings = [x for x in diagnostics if str(x.get("Severity", "")).lower() == "warning"]
    return {
        "error_count": len(errors),
        "warning_count": len(warnings),
        "errors": errors,
        "warnings": warnings,
        "code_gate_pass": len(errors) == 0,
    }


def wait_ready(mcp: Mcp, timeout: int) -> dict[str, object]:
    deadline, samples = time.time() + timeout, []
    last = {}
    while time.time() <= deadline:
        last = editor_state(mcp)
        samples.append(last)
        if last.get("ready_for_tools") and not last.get("is_compiling") and not last.get("is_changing"):
            return {"ready": True, "samples": samples[-5:], "final_state": last}
        time.sleep(1.0)
    return {"ready": False, "samples": samples[-5:], "final_state": last}


def classify(repo: Path, entries: list[dict[str, object]], paths: list[str], output_limit: int) -> dict[str, object]:
    targets = {p.lower() for p in paths}
    owned, external = [], []
    for item in entries:
        file = item.get("file")
        if isinstance(file, str) and rel(repo, file).lower() in targets:
            owned.append(item)
        else:
            external.append(item)
    owned_errors = [x for x in owned if str(x.get("type", "")).lower() == "error"]
    external_errors = [x for x in external if str(x.get("type", "")).lower() == "error"]
    owned_warnings = [x for x in owned if str(x.get("type", "")).lower() == "warning"]
    external_warnings = [x for x in external if str(x.get("type", "")).lower() == "warning"]
    return {
        "owned_counts": {"entries": len(owned), "errors": len(owned_errors), "warnings": len(owned_warnings)},
        "external_counts": {"entries": len(external), "errors": len(external_errors), "warnings": len(external_warnings)},
        "owned": limit_entries(owned, output_limit),
        "external": limit_entries(external, output_limit),
    }


def console_summary(logs: dict[str, object] | None, output_limit: int) -> dict[str, object] | None:
    if not logs:
        return None
    entries = logs.get("entries") or []
    return {
        "error_count": logs.get("error_count", 0),
        "warning_count": logs.get("warning_count", 0),
        **limit_entries(entries, output_limit),
    }


def manage_script_validation_summary(raw: dict[str, object], level: str, output_limit: int) -> dict[str, object]:
    data = raw.get("data") or {}
    diagnostics = data.get("diagnostics") or []
    errors = [x for x in diagnostics if str(x.get("severity", "")).lower() == "error"]
    warnings = [x for x in diagnostics if str(x.get("severity", "")).lower() == "warning"]
    infos = [x for x in diagnostics if str(x.get("severity", "")).lower() == "info"]
    status = "clean"
    if errors:
        status = "error"
    elif warnings and not bool(raw.get("success")):
        status = "failed_on_warnings"
    elif warnings:
        status = "warning"
    elif infos:
        status = "info_only"
    return {
        "requested_level": level,
        "status": status,
        "success": bool(raw.get("success")),
        "message": raw.get("message"),
        "error": raw.get("error"),
        "counts": {"errors": len(errors), "warnings": len(warnings), "infos": len(infos)},
        "diagnostics": limit_entries(diagnostics, output_limit),
        "assessment_policy": "compile_first",
        "strict_does_not_override_red_assessment": True,
    }


def skipped_manage_script_validation(level: str, reason: str, output_limit: int) -> dict[str, object]:
    return {
        "requested_level": level,
        "status": "skipped",
        "success": None,
        "message": None,
        "error": None,
        "counts": {"errors": 0, "warnings": 0, "infos": 0},
        "diagnostics": limit_entries([], output_limit),
        "assessment_policy": "compile_first",
        "strict_does_not_override_red_assessment": True,
        "skip_reason": reason,
    }


def manage_script_command(args: argparse.Namespace) -> dict[str, object]:
    guards = build_guards(args)
    target = single_script_target(args.repo_root, args)
    action = args.action
    payload = {
        "action": action,
        "target": target,
        "resource_guards": guards,
        "failure_recovery": {
            "bounded_wait": True,
            "bounded_console_read": True,
            "bounded_output": True,
            "background_processes_started": False,
            "explicit_exit_on_error": True,
        },
    }
    mcp = Mcp(args.endpoint, int(guards["timeout_sec"]))
    params: dict[str, object] = {"action": action, "name": target["name"], "path": target["path"]}
    if action == "validate":
        params["level"] = validation_level(args)
    raw = mcp.custom("manage_script", params)
    payload["manage_script"] = raw
    if action == "validate":
        summary = manage_script_validation_summary(raw, str(params["level"]), int(guards["output_limit"]))
        payload["validation"] = summary
        return result(
            "manage_script",
            bool(raw.get("success")),
            f"action=validate status={summary['status']} errors={summary['counts']['errors']} warnings={summary['counts']['warnings']}",
            payload,
        )
    sha = (raw.get("data") or {}).get("sha256")
    return result("manage_script", bool(raw.get("success")), f"action=get_sha sha={sha or 'n/a'}", payload)


def unity_red_check_value(args: argparse.Namespace, mcp_error: str | None, waited: dict[str, object] | None) -> str:
    if args.skip_mcp:
        return "live-pending"
    if mcp_error is not None:
        return "blocked"
    if waited is None or not bool(waited.get("ready")):
        return "blocked"
    return "pass"


def mcp_fallback_fields(args: argparse.Namespace, mcp_error: str | None, waited: dict[str, object] | None) -> tuple[str, str]:
    if args.skip_mcp:
        return ("required", "unity_validation_pending")
    if mcp_error is not None:
        return ("required", "baseline_fail")
    if waited is None or not bool(waited.get("ready")):
        return ("required", "blocked")
    return ("not-needed", "none")


def compile_pipeline(args: argparse.Namespace, command: str) -> dict[str, object]:
    guards = build_guards(args)
    paths = resolve_code_paths(args.repo_root, args)
    guard = codeguard(args.repo_root, args.owner_thread, paths)
    guard_counts = codeguard_stats(guard)
    mcp_error, refresh, waited, state, logs = None, None, None, None, None
    manage_script_validation = None
    compat_target = None
    compat_error = None
    if command == "validate_script" and len(paths) == 1:
        try:
            compat_target = single_script_target(args.repo_root, args, paths)
        except Exception as exc:
            compat_error = str(exc)
    if command == "validate_script" and args.skip_mcp and compat_target is not None:
        manage_script_validation = skipped_manage_script_validation(validation_level(args), "skip_mcp", int(guards["output_limit"]))
    if not args.skip_mcp:
        try:
            mcp = Mcp(args.endpoint, int(guards["timeout_sec"]))
            if command == "validate_script" and compat_target is not None:
                raw_validation = mcp.custom(
                    "manage_script",
                    {
                        "action": "validate",
                        "name": compat_target["name"],
                        "path": compat_target["path"],
                        "level": validation_level(args),
                    },
                )
                manage_script_validation = manage_script_validation_summary(raw_validation, validation_level(args), int(guards["output_limit"]))
            refresh = mcp.custom("refresh_unity", {"mode": args.mode, "scope": args.scope, "compile": "request", "wait_for_ready": True})
            waited = wait_ready(mcp, int(guards["wait_sec"]))
            state = waited["final_state"]
            logs = console(mcp, int(guards["console_count"]), True)
        except Exception as exc:
            mcp_error = str(exc)
    classified = classify(args.repo_root, (logs or {}).get("entries") or [], paths, int(guards["output_limit"]))
    owned_errors = int(classified["owned_counts"]["errors"])
    external_errors = int(classified["external_counts"]["errors"])
    unity_pending = args.skip_mcp or mcp_error is not None or waited is None or not bool(waited.get("ready"))

    assessment = "no_red"
    if not bool(guard_counts["code_gate_pass"]) or owned_errors > 0:
        assessment = "own_red"
    elif external_errors > 0:
        assessment = "external_red"
    elif unity_pending:
        assessment = "unity_validation_pending"

    unity_red_check = unity_red_check_value(args, mcp_error, waited)
    mcp_fallback, mcp_fallback_reason = mcp_fallback_fields(args, mcp_error, waited)

    payload = {
        "target_paths": paths,
        "resource_guards": guards,
        "codeguard": guard,
        "codeguard_counts": guard_counts,
        "refresh_unity": refresh,
        "wait_for_ready": waited,
        "editor_state": state,
        "console": console_summary(logs, int(guards["output_limit"])),
        "classification": classified,
        "mcp_error": mcp_error,
        "assessment": assessment,
        "no_red_receipt_v2": {
            "cli_red_check_command": command,
            "cli_red_check_scope": paths,
            "cli_red_check_assessment": assessment,
            "unity_red_check": unity_red_check,
            "mcp_fallback": mcp_fallback,
            "mcp_fallback_reason": mcp_fallback_reason,
            "current_owned_errors": owned_errors,
            "current_external_blockers": external_errors,
            "current_warnings": int(classified["owned_counts"]["warnings"]) + int(classified["external_counts"]["warnings"]),
        },
        "manage_script_compat": {
            "native_tool": "manage_script",
            "native_action": "validate",
            "requested_level": validation_level(args) if command == "validate_script" else None,
            "target": compat_target,
            "resolution_error": compat_error,
            "validation": manage_script_validation,
        }
        if command == "validate_script"
        else None,
        "failure_recovery": {
            "bounded_wait": True,
            "bounded_console_read": True,
            "bounded_output": True,
            "background_processes_started": False,
            "explicit_exit_on_error": True,
        },
    }

    if command == "compile":
        ok = assessment not in {"own_red", "blocked"}
    else:
        ok = assessment == "no_red"
    summary = f"assessment={assessment} owned_errors={owned_errors} external_errors={external_errors}"
    if command == "validate_script" and manage_script_validation is not None:
        summary += f" native_validation={manage_script_validation['status']}"
    return result(command, ok, summary, payload)


def blocked_result(command: str, args: argparse.Namespace, exc: Exception) -> dict[str, object]:
    return result(
        command,
        False,
        "assessment=blocked",
        {
            "assessment": "blocked",
            "message": str(exc),
            "resource_guards": build_guards(args),
            "failure_recovery": {
                "bounded_wait": True,
                "bounded_console_read": True,
                "bounded_output": True,
                "background_processes_started": False,
                "explicit_exit_on_error": True,
            },
        },
        exc.__class__.__name__,
    )


def main() -> None:
    p = argparse.ArgumentParser(
        description="Sunset MCP CLI focused on compile/error workflows.",
        epilog=(
            "examples:\n"
            "  sunset_mcp.py validate_script Assets/YYY_Scripts/Story/UI/SpringDay1PromptOverlay.cs --count 20\n"
            "  sunset_mcp.py validate_script --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level standard\n"
            "  sunset_mcp.py manage_script validate --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI --level strict\n"
            "  sunset_mcp.py manage_script get_sha --name SpringDay1PromptOverlay --path Assets/YYY_Scripts/Story/UI"
        ),
        formatter_class=argparse.RawTextHelpFormatter,
    )
    p.add_argument("--project-root", default=None)
    p.add_argument("--codex-home", default=str(DEFAULT_CODEX_HOME))
    p.add_argument("--endpoint", default=DEFAULT_ENDPOINT)
    p.add_argument("--timeout-sec", type=int, default=20)
    p.add_argument("--wait-sec", type=int, default=30)
    sub = p.add_subparsers(dest="cmd", required=True)
    sub.add_parser("baseline", help="Check MCP baseline: config, 8888 listener, pidfile.")
    sub.add_parser("status", help="Read baseline + editor state + recent console.")
    sub.add_parser("doctor", help="Print current compile-first usage recommendations.")
    err = sub.add_parser("errors", help="Read fresh console entries without compile classification.")
    err.add_argument("--count", type=int, default=10, help="How many console entries to ask Unity for before output clipping.")
    err.add_argument("--include-warnings", action="store_true", help="Also include warnings; default is errors only.")
    err.add_argument("--output-limit", type=int, default=12, help="How many entries to print in JSON output.")
    for name in ("compile", "no-red"):
        cp = sub.add_parser(name, help="Compile-first red check. Prefer explicit script paths to avoid shared-root noise.")
        cp.add_argument("paths", nargs="*", help="Explicit .cs target paths. If omitted, falls back to changed .cs files.")
        cp.add_argument("--owner-thread", default=DEFAULT_OWNER, help="Owner thread label passed into CodexCodeGuard.")
        cp.add_argument("--scope", default="scripts", help="refresh_unity scope; keep default unless MCP side changes.")
        cp.add_argument("--mode", default="force", help="refresh_unity mode; keep default for fresh compile checks.")
        cp.add_argument("--count", type=int, default=20, help="Fresh console sample size before clipping.")
        cp.add_argument("--skip-mcp", action="store_true", help="Only run code-layer checks; result will stay unity_validation_pending.")
        cp.add_argument("--all-changed", action="store_true", help="Allow all changed .cs files instead of enforcing explicit-path guard.")
        cp.add_argument("--output-limit", type=int, default=12, help="How many owned/external entries to print.")
    ms = sub.add_parser(
        "manage_script",
        help="Minimal native-compat surface over Unity manage_script. Only validate/get_sha are exposed on purpose.",
        description="Minimal native-compat surface over Unity manage_script. Only validate/get_sha are exposed on purpose.",
    )
    ms.add_argument("action", choices=("validate", "get_sha"), help="Native manage_script action to proxy.")
    ms.add_argument("paths", nargs="*", help="Optional explicit .cs path. If provided, do not mix with --name/--path.")
    ms.add_argument("--name", help="Script class/file name without .cs, e.g. SpringDay1PromptOverlay.")
    ms.add_argument("--path", help="Script directory under Assets/, e.g. Assets/YYY_Scripts/Story/UI.")
    ms.add_argument("--level", default="standard", choices=VALIDATION_LEVELS, help="Native validation level used by manage_script validate.")
    ms.add_argument("--output-limit", type=int, default=12, help="How many diagnostics to print.")
    val = sub.add_parser(
        "validate_script",
        help="High-frequency Sunset script red check. Supports explicit .cs paths or native-compatible --name/--path.",
        description="High-frequency Sunset script red check. Supports explicit .cs paths or native-compatible --name/--path.",
    )
    val.add_argument("paths", nargs="*", help="Explicit target .cs path. Preferred in dirty shared-root situations.")
    val.add_argument("--name", help="Native-compatible script name without .cs.")
    val.add_argument("--path", help="Native-compatible script directory under Assets/.")
    val.add_argument("--level", default="standard", choices=VALIDATION_LEVELS, help="Native manage_script validation level mirrored into manage_script_compat.")
    val.add_argument("--owner-thread", default=DEFAULT_OWNER, help="Owner thread label passed into CodexCodeGuard.")
    val.add_argument("--scope", default="scripts", help="refresh_unity scope; keep default unless MCP side changes.")
    val.add_argument("--mode", default="force", help="refresh_unity mode; keep default for fresh compile checks.")
    val.add_argument("--count", type=int, default=20, help="Fresh console sample size before clipping.")
    val.add_argument("--skip-mcp", action="store_true", help="Only run code-layer checks; assessment will remain unity_validation_pending.")
    val.add_argument("--all-changed", action="store_true", help="Allow all changed .cs files instead of enforcing explicit-path guard.")
    val.add_argument("--output-limit", type=int, default=12, help="How many console/diagnostic entries to print.")
    rec = sub.add_parser("recover-bridge", help="Try to relaunch MCP bridge if baseline is unhealthy.")
    rec.add_argument("--force", action="store_true", help="Launch even if baseline already looks healthy.")
    args = p.parse_args()
    args.repo_root = Path(args.project_root).resolve() if args.project_root else Path(__file__).resolve().parent.parent
    args.codex_home = Path(args.codex_home).resolve()

    try:
        if args.cmd == "baseline":
            base = baseline(args.repo_root, args.codex_home, args.endpoint)
            payload = result("baseline", bool(base["is_pass"]), f"baseline_status={base['baseline_status']}", base)
        elif args.cmd == "status":
            base = baseline(args.repo_root, args.codex_home, args.endpoint)
            mcp_state, logs, mcp_error = None, None, None
            try:
                mcp = Mcp(args.endpoint, args.timeout_sec)
                mcp_state = editor_state(mcp)
                logs = console(mcp, 10, True)
            except Exception as exc:
                mcp_error = str(exc)
            payload = result("status", bool(base["is_pass"]), "compile_first_status", {"baseline": base, "bridge_status": json.loads((args.repo_root / "Library" / "CodexEditorCommands" / "status.json").read_text(encoding="utf-8", errors="replace")) if (args.repo_root / "Library" / "CodexEditorCommands" / "status.json").exists() else None, "autostart_tail": (args.repo_root / "Library" / "MCPForUnity" / "RunState" / "codex_mcp_http_autostart_status.txt").read_text(encoding="utf-8", errors="replace").splitlines()[-10:] if (args.repo_root / "Library" / "MCPForUnity" / "RunState" / "codex_mcp_http_autostart_status.txt").exists() else [], "editor_state": mcp_state, "console": logs, "mcp_error": mcp_error})
        elif args.cmd == "doctor":
            base = baseline(args.repo_root, args.codex_home, args.endpoint)
            recs = []
            if not base["is_pass"]:
                recs.append("先跑 recover-bridge")
            recs.append("高频顺序应是：compile -> errors/no-red -> 再决定是否继续改代码")
            recs.append("validate_script 已是脚本级单命令入口；需要原生兼容时优先用 --name/--path/--level")
            recs.append("manage_script 只开放 validate|get_sha；不要把 create/update/edit 面重新搬回高频 CLI")
            recs.append("外部 red 仍然只能诚实报 external_red，不能因为 native validate 通过就包装成 pass")
            payload = result(
                "doctor",
                True,
                f"recommendations={len(recs)}",
                {
                    "baseline": base,
                    "recommendations": recs,
                    "no_red_receipt_v2": {
                        "required_fields": [
                            "cli_red_check_command",
                            "cli_red_check_scope",
                            "cli_red_check_assessment",
                            "unity_red_check",
                            "mcp_fallback",
                            "mcp_fallback_reason",
                            "current_owned_errors",
                            "current_external_blockers",
                            "current_warnings",
                        ],
                        "assessment_values": ["no_red", "own_red", "external_red", "unity_validation_pending", "blocked"],
                        "mcp_fallback_reason_values": [
                            "baseline_fail",
                            "unity_validation_pending",
                            "blocked",
                            "scene_live_flow_required",
                            "playmode_required",
                            "inspector_required",
                        ],
                    },
                },
            )
        elif args.cmd == "errors":
            guards = build_guards(args)
            mcp = Mcp(args.endpoint, int(guards["timeout_sec"]))
            logs = console(mcp, int(guards["console_count"]), args.include_warnings)
            payload = result(
                "errors",
                logs["error_count"] == 0,
                f"errors={logs['error_count']} warnings={logs['warning_count']}",
                {"resource_guards": guards, **console_summary(logs, int(guards["output_limit"]))},
            )
        elif args.cmd == "manage_script":
            payload = manage_script_command(args)
        elif args.cmd in {"compile", "no-red", "validate_script"}:
            try:
                payload = compile_pipeline(args, args.cmd)
            except Exception as exc:
                payload = blocked_result(args.cmd, args, exc)
        elif args.cmd == "recover-bridge":
            base = baseline(args.repo_root, args.codex_home, args.endpoint)
            terminal = args.repo_root / "Library" / "MCPForUnity" / "TerminalScripts" / "mcp-terminal.cmd"
            if base["is_pass"] and not args.force:
                payload = result("recover-bridge", True, "Bridge baseline already looks healthy.", {"started": False, "baseline": base})
            elif not terminal.exists():
                payload = result("recover-bridge", False, "terminal_script_missing", {"path": str(terminal)})
            else:
                subprocess.Popen(["cmd.exe", "/c", str(terminal)], cwd=str(args.repo_root), stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, creationflags=getattr(subprocess, "CREATE_NO_WINDOW", 0))
                latest = base
                deadline = time.time() + args.wait_sec
                while time.time() <= deadline:
                    time.sleep(0.5)
                    latest = baseline(args.repo_root, args.codex_home, args.endpoint)
                    if latest["is_pass"]:
                        break
                payload = result("recover-bridge", bool(latest["is_pass"]), "Bridge launch attempted.", {"started": True, "baseline": latest})
        else:
            raise RuntimeError(f"Unsupported command: {args.cmd}")
    except Exception as exc:
        payload = result(args.cmd or "sunset-mcp", False, "sunset-mcp command failed", {"message": str(exc), "repo_root": str(args.repo_root), "endpoint": args.endpoint}, exc.__class__.__name__)

    write(payload, 0 if payload["success"] else 1)


if __name__ == "__main__":
    main()
