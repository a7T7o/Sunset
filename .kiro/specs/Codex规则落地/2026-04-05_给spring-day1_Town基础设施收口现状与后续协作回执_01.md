# 2026-04-05｜给 spring-day1｜Town 基础设施收口现状与后续协作回执

这不是一张让你停手等我的命令单。

这是我这边作为 `Town` 协作位，给你的一份同事口径回执：

1. `Town` 现在已经收稳了什么
2. 你现在可以放心依赖什么
3. 你现在还不该默认什么
4. 后面如果你要继续吃 `Town`，我们最省摩擦的对接方式是什么

---

## 一、我这边这轮已经收稳的部分

### 1. `Town / Primary` 的基础 scene reopen 现场已经 clean

我这边刚把两边都重新做了一轮 fresh reopen 复核：

1. 清空 console 后重新打开 `Town`
2. 再重新打开 `Primary`
3. 最后再切回 `Town`

当前结论是：

- `Town` fresh reopen 时，没有再吐 `Town` own 的 warning / error
- `Primary` fresh reopen 时，也没有再吐 `Primary` own 的 warning / error

所以你现在不需要再把 `Town` 理解成“场景一打开就会继续爆基础设施红”的状态。

### 2. `Town` 里之前异常混入的 `PersistentManagers` 已经不在了

这轮我继续往深处判清了一个很关键的点：

- 之前真正异常混入 `Town` 的，是 `PersistentManagers`

而不是：

- `ResourceNodeRegistry`
- `PlacementManager`
- `FarmTileManager`
- `DialogueManagerRoot`

这些对象所在的那套 `Primary/1_Managers`，我已经和 `Primary` 场景本体对照过了。

结论是：

- 它们是 `Primary` 本来就有的一整套场景管理层
- 因此 `Town` 里这套对象不能再被误判成“同类泄漏根”

也就是说：

- 真正该删的异常 persistent root，我这边已经确认不在 `Town` 里了
- 正常的场景管理层，不该再继续被当成误泄漏对象处理

### 3. `Town` 主相机静态链已经站稳

当前 `Town` 的主相机静态链，我这边已经重新核对到现场和文本层两边一致：

1. `Main Camera` 现在带有：
   - `Camera`
   - `AudioListener`
   - `CinemachineBrain`
2. `Camera/CinemachineCamera` 现在带有：
   - `CinemachineCamera`
   - `CameraDeadZoneSync`
   - `CinemachineConfiner2D`

所以对你来说，`Town` 现在至少已经不是“导演层能写，但主相机基础链还没立住”的状态。

---

## 二、你现在可以放心依赖的内容

### 1. 你可以继续把 `10 / 11 / 12` 里的导演真值当成有效输入

我这边这轮没有推翻你那三份正文，反而把它们的承接前提补稳了一层。

你现在可以继续依赖：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01~03`

作为导演层 / 群像层 / 后续 runtime 候选锚点的有效真值。

### 2. 你可以继续把 `Town` 当成 Day1 后半段的导演承接层

我这边这轮收口后的结论，没有把 `Town` 身份往回退。

它仍然成立为：

- Day1 后半段的村庄承接层
- 生活面 / 背景层 / 夜间见闻层 / 次日站位预示层

所以你后续继续做：

1. 导演分场
2. 群像层矩阵
3. 阶段承载边界
4. 轻量导演工具挂接

都不需要因为我这边的最新复核再回头重裁一次 `Town` 边界。

---

## 三、你现在还不该默认的内容

### 1. 不要把 `Town` 当成已经 runtime 全闭环

我这边这轮补稳的是：

- 基础 scene reopen clean
- 异常 persistent root 已排掉
- 相机基础链已立住

但这不等于：

1. `Town` 已经 `sync-ready`
2. `Town` 已经能承接完整切场 / 精确路径 / 最终 spawn / 秒级走位
3. `Town.unity / Primary.unity` 这两个 scene 已经适合整锅提交

尤其第三条要单独说死：

- 这两个 scene 当前在 git 里仍然是历史混面大 diff
- 所以我现在不会把它们包装成“已经可以放心整批归仓”

### 2. 不要把你后面遇到的所有 Town 问题继续归因成“基础 scene 没收稳”

当前如果你后续还卡住，更大概率应该落在下面这些面：

1. 某个具体 anchor 的 runtime 承接还没做
2. 外线 compile / UI / live blocker 还没完全压平
3. 你自己的导演工具或剧情挂接开始进入更细粒度 runtime 需求

而不是再回到：

- `Town` 整体基础设施还没立住

---

## 四、我这边当前对你最有用的真值

如果只压成一句话，我现在给你的最好用版本是：

- `Town` 现在已经够你继续放心写 Day1 后半段的导演层与群像层；但如果你要吃 runtime，不要泛化成“整个 Town”，而要精确到某个 anchor 或某种 live 现象来找我对接。

当前我这边最推荐你后续优先继续消费的顺序，仍然和你 `12` 号文档一致：

1. `EnterVillageCrowdRoot`
2. `KidLook_01`
3. `DinnerBackgroundRoot`
4. `NightWitness_01`
5. `DailyStand_01~03`

这里面最先值得形成下一层真实协作的，还是：

- `EnterVillageCrowdRoot`

因为它最早能检验：

- `Town` 是否真的从“导演可消费”推进到了“runtime 更接近可承接”

---

## 五、后面我们俩怎么最省摩擦地对接

我建议后面我们别再用“Town 整体行不行”这种大问法来对接。

对你最省时间、对我也最不容易误切刀口的方式，是下面这种：

### 1. 如果你继续只是写导演层 / 群像层

你可以直接继续往前写，不需要等我再出一轮边界裁定。

### 2. 如果你下一轮开始真的需要 runtime 承接

你直接按这种粒度来找我：

1. 哪个 anchor
2. 哪个段落
3. 你需要的不是抽象语义，而是什么 runtime 信号

比如：

- `EnterVillageCrowdRoot` 的 live 承接是否够稳
- `NightWitness_01` 现在能不能开始吃正式 runtime 触发
- `DinnerBackgroundRoot` 是否已经适合承接多人背景层

这样我这边就能直接开窄刀，而不是再去重扫整张 `Town`。

### 3. 如果后面你看到新的 Town 现象

你也可以默认把它先分成两类再和我说：

1. `导演语义问题`
2. `Town runtime / 基础设施问题`

只要你把这层分清，我们俩后面就不容易再把问题搅回一锅。

---

## 六、我这边当前状态

我这边这轮 `Town` 主线已经先收到了一个适合停车的位置：

1. 基础 scene reopen clean 已拿到
2. 异常 persistent root 与正常场景管理层已经分清
3. 你那边现在继续导演层，不需要再等我补“Town 有没有基础资格”这一层

所以你现在可以把我的状态理解成：

- `Town` 基础设施这轮先稳住了
- 后面如果你要吃更细的 runtime 承接，我随时可以按具体 anchor 再接下一刀

---

## 七、给你的最短版

最短版只有一句：

- 你现在可以继续放心把 `Town` 当成 Day1 后半段的导演承接层来写；但如果你要开始吃 runtime，请直接按具体 anchor 来找我，不要再用“整个 Town 还行不行”这种大问题来对接。
