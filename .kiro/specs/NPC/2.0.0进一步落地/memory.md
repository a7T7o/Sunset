# NPC 2.0.0 进一步落地 memory

## 2026-03-25

- 当前主线目标：
  - 把 NPC 从“功能验证对象”正式推进到“可持续扩展的角色系统设计阶段”，并把本轮用户新增的大需求收成稳定的 2.0.0 工作区入口。
- 本轮子任务：
  - 新建 `2.0.0进一步落地` 的工作区记忆。
  - 新建 `需求拆分.md`，完整记录用户本轮详细 prompt 与系统级拆分。
  - 将 `NPC-导航接入契约与联调验收规范` 迁移到 2.0.0，并把它确认为后续唯一维护版本。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\需求拆分.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC-导航接入契约与联调验收规范.md`
  - 将 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\1.0.0初步规划\NPC-导航接入契约与联调验收规范.md` 收口为迁移说明，避免继续双份正文并存
- 本轮锁定的核心需求：
  - 文档治理上：NPC 同类设计文档要少而精，尤其导航契约必须只维护一个最终版
  - 业务设计上：开始进入 NPC 场景化真实落点、人设化气泡与相遇对话、受击/工具命中/反应兼容、好感度与玩家/NPC 双气泡体系
  - 阶段边界上：导航契约仍是独立协作文档，但 2.0.0 需要开始承接更大的 NPC 产品化设计
- 当前恢复点：
  - 后续 NPC 设计型工作优先进入 `2.0.0进一步落地`
  - `1.0.0初步规划` 仍保留历史救援与早期收口记录，但导航契约的当前唯一维护版本已经迁到 2.0.0

## 2026-03-25｜2.0.0 第一刀实现：角色化 profile 与生成器默认映射

- 当前主线目标：
  - 在用户批准 2.0.0 文档结构后，先把现在就能安全落地、又不撞导航主战场的 NPC 内容真正落进 `main`。
- 本轮子任务：
  - 补齐 `2.0.0进一步落地` 的 4 份主文件。
  - 为 `001 / 002 / 003` 建立独立角色化 `NPCRoamProfile`。
  - 让 prefab 与生成器默认映射到这套角色化 profile，而不再共享一份通用环境话术。
- 本轮完成：
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC场景化真实落点与角色日常设计.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC交互反应与关系成长设计.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\.kiro\specs\NPC\2.0.0进一步落地\NPC系统实施主表.md`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_001_VillageChiefRoamProfile.asset`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_002_VillageDaughterRoamProfile.asset`
  - 新增 `D:\Unity\Unity_learning\Sunset\Assets\111_Data\NPC\NPC_003_ResearchReviewProfile.asset`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\Editor\NPCPrefabGeneratorTool.cs`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\001.prefab`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\002.prefab`
  - 修改 `D:\Unity\Unity_learning\Sunset\Assets\222_Prefabs\NPC\003.prefab`
- 本轮关键实现：
  - `001` 当前默认走村长 profile，日常自言自语和对聊口吻更偏“稳、看局面、管节奏”。
  - `002` 当前默认走村长女儿 profile，口吻更生活化、更柔和、更带环境感受。
  - `003` 当前默认走研究型 review profile，同时 `NPCBubbleStressTalker.testLines` 也被切成研究记录风格，不再是纯测试句。
  - 生成器现在会按 NPC 名称自动选 profile：`001 -> VillageChief`、`002 -> VillageDaughter`、`003 + BubbleReview -> ResearchReview`。
- 本轮验证：
  - `git diff --check` 已通过本轮 NPC 文档、asset、prefab 与生成器脚本。
  - prefab 当前引用的新 profile GUID 已与对应 `.meta` 闭合：
    - `001 -> 0c11f4f44e5a4dbd93de8c2fd8c06001`
    - `002 -> 0c22a5d55f6b4ecd84af9d3ea7d27002`
    - `003 -> 0c33b6e6607c4fed95b0ae4fb8e38003`
  - 本轮没有进入 Unity / MCP live 写，也没有触碰 `Primary.unity`、`NPCAutoRoamController.cs`、`NPCMotionController.cs`、导航核心或玩家导航核心。
- 当前恢复点：
  - `2.0.0进一步落地` 现在已经不是纯文档工作区，而是开始承接角色化数据与 prefab 基线。
  - 下一刀若继续安全推进，应优先考虑真实场景落点、双气泡规范或关系成长入口；导航运动语义仍保持不越界。
