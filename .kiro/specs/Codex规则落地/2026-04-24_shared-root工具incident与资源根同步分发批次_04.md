这轮不再继续群发 shared-root 业务上传 prompt。

第三波回执已经把盘面压成了两条真正的下一步：
1. `spring-day1 / UI / 存档系统 / 导航检查` 合并成 `1` 条统一工具 incident 线
2. `NPC` 单开 `Assets/Resources/Story` 目录级同步 blocker 线

当前唯一改判如下：
1. 不再给 `spring-day1 / UI / 存档系统 / 导航检查` 原业务线程继续发“再试一次上传”的 prompt。
2. 这 `4` 条线当前都应保持 `PARKED`，不要再让它们重复撞同一个 `CodexCodeGuard / pre-sync` blocker。
3. `NPC` 也不再继续改 `npcId:104` 内容本身；内容一致性已经补平，下一刀只处理目录级同步 blocker。

本波定向线程只有 `2` 条：

## 1. `只读工具链分身`
- 领取文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给只读工具链分身_统一CodexCodeGuard预同步incident排查prompt_04.md`
- 当前唯一目标：
  - 把 `spring-day1 / UI / 存档系统 / 导航检查` 这 `4` 条线已经拿到的 incident 证据收成一份统一结论
  - 明确这到底是 `1` 个共因 incident，还是 `2+` 个不同 incident
  - 把下一刀真正该修的文件/函数边界说死
- 这条线默认只读，不替业务线程继续上传

## 2. `NPC`
- 领取文件：
  - `D:\Unity\Unity_learning\Sunset\.kiro\specs\Codex规则落地\2026-04-24_给NPC_AssetsResourcesStory目录同步blocker处理prompt_04.md`
- 当前唯一目标：
  - 不再改 `104` 内容
  - 只处理 `Assets/Resources/Story` 这条目录级同步 blocker 的 exact 边界
  - 如果 blocker 已自然消失，才允许对白名单这 `3` 个文件做 `1` 次真实上传尝试

本波明确停发，不在这一轮继续转发：
1. `spring-day1`
2. `UI`
3. `存档系统`
4. `导航检查`
5. `农田交互修复V3`
6. `019d4d18-bb5d-7a71-b621-5d1e2319d778`
7. `云朵与光影`
8. `树石修复`

本波最核心的治理判断：
1. `spring-day1 / UI / 存档系统 / 导航检查` 的问题已经不再是“业务切片还不完整”，而是工具链已经先挂。
2. `NPC` 的问题也不再是“内容没修完”，而是同步口被 `Assets/Resources/Story` 根下他线脏改挡住。
3. 如果现在还继续拿旧模板催业务线程往下跑，只会制造重复噪音，不会新增有效证据。

统一要求：
1. 这波所有新回执，仍然优先按：
   - `A1 保底六点卡`
   - `A2 用户补充层（可选）`
   - `B 技术审计层`
2. 这波不准把工具 incident 包装成“业务线程只差再试一次”。
3. 这波不准把 `NPC` 目录 blocker 又讲回“104 内容还没修完”。
