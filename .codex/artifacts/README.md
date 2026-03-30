# UI 证据产物

- 运行时 `ScreenSpaceOverlay` / GameView 证据统一落在 `.codex/artifacts/ui-captures/`。
- 默认只保留目录结构与说明文件，不把实际截图和 JSON 侧车提交进仓库。
- 当前 `SpringUI` 约定结构：
  - `spring-ui/pending/`：待清理 / 待挑选的临时证据
  - `spring-ui/accepted/`：人工确认需要长期保留的基线证据
  - `spring-ui/latest.json`：最新一次证据指针
  - `spring-ui/manifest.jsonl`：capture / promote / prune 事件流水
- 常用入口：
  - Unity 菜单：`Sunset/Story/Debug/Capture Spring UI Evidence`
  - Unity 菜单：`Sunset/Story/Debug/Promote Latest Spring UI Evidence`
  - PowerShell：`powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action latest`
  - PowerShell：`powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action promote-latest`
  - PowerShell：`powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action prune -RetentionDays 14`
- `pending` 默认按 `14` 天保留；`accepted` 不自动删除。
- 不要再把 `manage_camera` 的 Main Camera 截图，当成 `ScreenSpaceOverlay` UI 的最终证据。
