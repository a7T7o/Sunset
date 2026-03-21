# 2026.03.21 main-only 极简并发开发 01

## 这是当前现行入口
- 这一批次已经和旧的 `2026.03.21_恢复开发总控_01` 分离。
- 旧目录从现在开始只保留历史记录，不再继续承载当前 prompt。

## 当前目录结构
- `可分发Prompt/`
  - 当前直接开发 prompt
  - 当前回执 / 冻结 / 交接 prompt
- `治理线程_当前执行Prompt.md`
  - 给治理线程自己看的最短执行口径
- `治理线程_职责与更新规则.md`
  - 给后续续办时看的最小维护规则

## 当前一句话口径
- 普通开发默认直接在 `D:\Unity\Unity_learning\Sunset @ main` 并发推进。
- 治理线程只在高危撞车、Unity / MCP 单写冲突、或现场已写坏时介入。
- `scene-build` 是当前唯一的过渡例外：
  - 现工作现场：`D:\Unity\Unity_learning\Sunset_worktrees\scene-build-5.0.0-001`
  - 目标迁移路径：`D:\Unity\Unity_learning\SceneBuild_Standalone\scene-build-5.0.0-001`
