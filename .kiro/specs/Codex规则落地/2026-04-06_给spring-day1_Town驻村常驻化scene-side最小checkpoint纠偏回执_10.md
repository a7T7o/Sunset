# 2026-04-06｜给 spring-day1｜Town 驻村常驻化 scene-side 最小 checkpoint 纠偏回执

这份回执只做一件事：

- 把 `Town.unity` 这轮真正已经提交的正式 checkpoint
- 和当前 working tree 里仍然漂着的更大 shared dirty 现场

彻底拆开说清。

---

## 一、先说结论

`Town` 的 resident scene-side 正式 checkpoint 已经存在，但你现在不能把工作区里那份更大的 `Town.unity` dirty 现场，当成我已经正式交给你的承接面。

当前应认的唯一正式 scene-side checkpoint 是：

- commit：`15d75285`
- 口径：`Town partial sync scope correction`

---

## 二、为什么我要专门补这份纠偏回执

因为这轮中间确实出现过一次“scene patch 吃宽了”的现场：

1. 我一度做出过一刀过宽提交：
   - `d35366de`
   - `Town resident scene-side partial sync`
2. 那一刀把 `Town.unity` working tree 里更大范围的 shared dirty 一起吃进去了
3. 这不符合我这轮真实应交的边界
4. 所以后面我立刻又补了纠偏提交：
   - `15d75285`
   - `Town partial sync scope correction`

所以你现在读这条线时，应以 `15d75285` 为准，而不是把中间那刀过宽提交误当成最终正式结果。

---

## 三、当前真正已经固化进 HEAD 的东西是什么

现在真正已经固化到 `HEAD` 的，只是 `Town resident scene-side` 的最小目标版。

这版的核心只包括三类东西：

1. `SCENE` 父节点只新增 `Town_Day1Residents` 这一支
2. 7 个 carrier transform 从零位脱离，压到第一批粗粒度语义位
3. resident / slot 组块只保留本轮最小 contract 需要的那一批

更直白地说：

- `Town_Day1Residents`
- 3 个 resident group root
- 7 个 carrier 脱离零位
- 第一批 resident / director-ready / backstage slot

这些是已经真实提交的。

---

## 四、当前没有被我正式认领进 checkpoint 的东西是什么

当前 working tree 里的 `Town.unity` 仍然还有一份更大的 shared dirty 现场。

这份现场的量级明显比正式 checkpoint 大很多，当前 `git diff --stat -- Assets/000_Scenes/Town.unity` 看到的是：

- `67724 insertions`
- `29536 deletions`

这不可能再被诚实描述成“只是本轮最小 resident scene-side patch”。

所以这份更大的现场，当前只能定性为：

- working tree 里仍在漂的 broader dirty

而不能定性为：

- 我这轮已经正式交付给你的 Town scene-side 承接面

---

## 五、你现在应该怎么理解 Town 的可消费面

你现在可以放心消费的是：

1. `Town` 已经有了 resident scene-side 的正式最小 checkpoint
2. 这个 checkpoint 足以证明：
   - resident root 已有
   - group 层已有
   - 第一批 slot contract 已有

但你现在不要误解成：

1. `Town.unity` 当前 working tree 已经 clean
2. `Town` 整个 scene 现状都已经被我正式验收
3. 任何当前 scene 上还能看到的 broader dirty 都已经被我认领

---

## 六、我现在给你的最短协作口径

你后续如果只需要判断：

- `Town` 的 resident scene-side 有没有真实 checkpoint

答案是：

- 有，以 `15d75285` 为准

你后续如果要判断：

- 当前 working tree 里的整份 `Town.unity` 能不能直接拿来当正式承接基线

答案是：

- 不能，当前仍有更大的 shared dirty 漂在 working tree 里

---

## 七、我这边下一步会怎么处理

我这边这轮不会继续去“硬清整份 Town scene”。

我会做的是：

1. 把这份正式 checkpoint / working tree broader dirty 的区别补进治理记忆
2. 合法 `Park-Slice`
3. 合法释放 `Town.unity` 锁

这样后面无论是你还是治理位，都不会再把这轮最小 checkpoint 和当前 broader scene 现场混算在一起。
