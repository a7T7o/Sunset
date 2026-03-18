# 阶段任务：治理工作区归位与彻底清盘

## 阶段目标
- 把 Sunset 的治理正文从 `000_代办/codex` 迁回正式工作区。
- 把 `000_代办/codex` 降级成真正的 TD 镜像层。
- 收掉 `01/02/03/07/09/10/11` 在文档上的剩余尾巴。
- 清理仓库外已退役的证据/备份目录。

## 2026-03-18 迁移说明
- 本阶段保留“治理工作区归位”和“代办区不再充当工作区”的历史结论。
- 但后续 live 现场重新失真后，新的治理重点已不再是工作区迁移本身，而是：
  - shared root 现场回正
  - 物理闸机落地
- 这部分新任务已迁移到：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\20_shared-root现场回正与物理闸机落地`
- `12` 不再继续承接新的 live 修复动作。

## 已完成
- [x] 新建正式工作区 `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地`
- [x] 将原 `000_代办/codex` 的阶段目录与根层 `memory` 迁入正式工作区
- [x] 新建工作区根层设计说明，明确“工作区 vs 代办区”边界
- [x] 新建本阶段目录 `12_治理工作区归位与彻底清盘`
- [x] 为 shared root 新建独立占用状态文档
- [x] 为 `000_代办/codex` 重建 TD-only 镜像结构
- [x] 形成外部 `archives/backups` 的身份与退役说明
- [x] 删除 `D:\Unity\Unity_learning\Sunset_external_archives`
- [x] 删除 `D:\Unity\Unity_learning\Sunset_backups`
- [x] 将 live 入口、AGENTS、skills 路由改指向 `Codex规则落地`
- [x] 把 `000_代办/codex` 从“工作区入口”改写为“代办镜像入口”
- [x] 显式封板 `03/10/11` 的遗留文档尾项
- [x] 对 `01/02/07/09` 的剩余待办做最终裁定并收口

## 不再继续做
- [x] 不再把新的治理正文继续写回 `000_代办/codex`
- [x] 不再把 `11` 当成继续追加新尾巴的阶段
- [x] 不再保留仓库外历史备份目录作为当前有效 carrier

## 完成标准
- 正式治理工作区固定为 `Codex规则落地`
- `000_代办/codex` 只剩 TD 镜像与入口说明
- 当前 live 文档不再把 `000_代办/codex` 误判为工作区
- shared root 占用规则有单独落盘
- 外部 archive / backup 目录彻底退役
- 当前项目进入“shared root `main` + branch-only + TD 镜像”的稳定结构
