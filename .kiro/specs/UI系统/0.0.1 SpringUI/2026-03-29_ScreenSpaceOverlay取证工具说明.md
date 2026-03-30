# SpringUI ScreenSpaceOverlay 取证工具说明

## 1. 为什么要补这把工具

- `SpringDay1PromptOverlay` 的 runtime 现场已经确认是 `ScreenSpaceOverlay`。
- 这类 UI 不经过主相机合成，所以 `manage_camera` / Main Camera 截图不能直接证明它有没有显示、位置对不对、尺寸漂没漂。
- 之前我们缺的不是“分析再多一点”，而是**抓最终合成屏幕**的能力。

## 2. 这轮落地了什么

- runtime 抓屏器：`D:\Unity\Unity_learning\Sunset\Assets\YYY_Scripts\Story\UI\SpringUiEvidenceCaptureRuntime.cs`
- Unity 菜单入口：`D:\Unity\Unity_learning\Sunset\Assets\Editor\Story\SpringUiEvidenceMenu.cs`
- 仓库外证据脚本：`D:\Unity\Unity_learning\Sunset\scripts\SpringUiEvidence.ps1`
- 仓库内证据说明：`D:\Unity\Unity_learning\Sunset\.codex\artifacts\README.md`

## 3. 正确使用方式

- 第一步：进入 `PlayMode`
- 第二步：如需最小 Day1 现场，先执行 `Sunset/Story/Debug/Bootstrap Spring Day1 Validation`
- 第三步：执行：
  - `Sunset/Story/Debug/Capture Spring UI Evidence`
  - 或 `Sunset/Story/Debug/Bootstrap + Capture Spring UI Evidence`
- 第四步：检查：
  - `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending`
  - 是否同时生成一对 `.png + .json`
- 第五步：如果这张图可以作为后续验收基线，再执行：
  - `Sunset/Story/Debug/Promote Latest Spring UI Evidence`
  - 或 `powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action promote-latest`

## 4. 证据目录约定

- `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\pending`
  - 临时证据
  - 默认 `14` 天后可清理
- `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\accepted`
  - 人工确认保留的基线证据
- `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\latest.json`
  - 当前最新一张证据的指针
- `D:\Unity\Unity_learning\Sunset\.codex\artifacts\ui-captures\spring-ui\manifest.jsonl`
  - `capture / promote / prune` 事件流水

## 5. JSON 侧车里有什么

- 抓图时间、场景名、帧号、屏幕尺寸
- 取证来源：`WaitForEndOfFrame + ScreenCapture.CaptureScreenshotAsTexture`
- `SpringDay1LiveValidation` 快照字符串
- Prompt 关键节点：
  - `Canvas.renderMode`
  - `CanvasGroup.alpha`
  - `TaskCardRoot / Page / BackPage`
  - `Title / Subtitle / Focus / Footer`
- Workbench 关键节点：
  - `PanelRoot`
  - `Recipe Viewport / Content`
  - `Materials Viewport / Content`
  - `ProgressBackground / ProgressFill`
  - `CraftButton`
  - `FloatingProgressRoot`

## 6. 什么才算有效证据

- 有效：
  - `pending` 或 `accepted` 里的最终合成屏 `.png`
  - 同名 `.json` 里同时记录了 Prompt / Workbench 的 runtime 几何和文本状态
- 无效：
  - 只看 Main Camera 截图就下结论
  - 只看 Console 日志，不看最终屏幕
  - 只看代码链和测试通过，不看 live 图像

## 7. 清理与维护

- 查看最新指针：
  - `powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action latest`
- dry-run 清理：
  - `powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action prune -RetentionDays 14 -DryRun`
- 实际清理：
  - `powershell -ExecutionPolicy Bypass -File .\scripts\SpringUiEvidence.ps1 -Action prune -RetentionDays 14`
- 规则：
  - `pending` 可以定期清
  - `accepted` 不自动删
  - 不把实际证据图提交进 Git

## 8. 后续线程统一口径

- 以后只要任务涉及 `ScreenSpaceOverlay`、Prompt 最终观感、工作台最终排版、GameView 真正看到的 UI，都应优先用这套工具。
- 如果线程回执仍拿 Main Camera 截图当 Prompt 几何证据，默认视为证据口径不合格。
