# 线程续工统一尾巴：`thread-state` 补登记 / 收口提醒（2026-04-01）

适用范围：

- 任何会继续真实施工的 Sunset 旧线程
- 任何刚要开始真实施工的新线程
- 任何还没接进 `thread-state`，但已经在跑业务切片的线程

用途：

- 这不是替代业务 prompt 的正文
- 这是你可以直接追加在业务 prompt 末尾的一段统一尾巴
- 它的作用是把旧线程补接到当前 live 并发规则上

推荐直接追加下面这段：

```text
[thread-state 接线要求｜本轮强制]
从现在起，这条线程按 Sunset 当前 live 规范接入 `thread-state`：

- 如果你这轮会继续真实施工，先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Begin-Slice.ps1`
- 旧线程不用废弃重开；但最晚必须在“下一次真实继续施工前”补这次 `Begin-Slice`
- 第一次准备 `sync` 前，必须先跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Ready-To-Sync.ps1`
- 如果这轮先停、卡住、让位或不准备收口，改跑：
  `D:\Unity\Unity_learning\Sunset\.kiro\scripts\thread-state\Park-Slice.ps1`

回执里额外补 3 件事：
- 这轮是否已跑 `Begin-Slice / Ready-To-Sync / Park-Slice`
- 如果还没跑，原因是什么
- 当前是 `ACTIVE / READY / PARKED`，还是被 blocker 卡住

这不是建议，是当前 live 规则的一部分。除非这轮始终停留在只读分析，否则不要跳过。
```

补充说明：

- 老线程：用这段尾巴补接即可，不需要推倒重开
- 新线程：从第一轮真实施工 prompt 开始，默认就该带这段要求
- 治理位：如果本轮 prompt 会让线程继续写 tracked 内容，就默认追加这段尾巴
