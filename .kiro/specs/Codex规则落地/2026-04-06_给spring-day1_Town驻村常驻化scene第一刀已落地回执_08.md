# 2026-04-06｜给 spring-day1｜Town 驻村常驻化 scene 第一刀已落地回执

这份回执只报 `Town.unity` 的 resident scene-side 第一刀真实落点，不回到泛方向说明。

## 一句话结论

- `Town` 这边的 **resident scene-side 第一刀已经真实落下**：
  - `SCENE/Town_Day1Residents`
  - `Resident_DefaultPresent`
  - `Resident_DirectorTakeoverReady`
  - `Resident_BackstagePresent`
  - 以及 7 个 `Town_Day1Carriers/*` 已全部脱离 `(0,0,0)` 空壳

## 这轮真实做成了什么

### 1. resident 容器层已存在

`Town.unity` 现在已经有：

- `SCENE/Town_Day1Residents`
  - `Resident_DefaultPresent`
  - `Resident_DirectorTakeoverReady`
  - `Resident_BackstagePresent`

这意味着：

- `Town` 不再只有 director carrier 壳
- 已经第一次拥有“驻村常驻化”的 scene-side 容器层

### 2. 7 个 carrier 已全部脱零位

当前大致空间语义已压成下面这版：

| carrier | 当前粗粒度空间语义 |
| --- | --- |
| `EnterVillageCrowdRoot` | 村口入口内侧的第一拍围观/让位位 |
| `KidLook_01` | 入口旁、稍偏通路边的单点探头位 |
| `DinnerBackgroundRoot` | 村内中段、偏饭馆/生活层的背景位 |
| `NightWitness_01` | 村边偏上、夜路/坡边感更强的见闻位 |
| `DailyStand_01` | 生活层靠中左的次日常驻位 |
| `DailyStand_02` | 生活层更靠中段的次日常驻位 |
| `DailyStand_03` | 生活层靠中右的次日常驻位 |

## 这轮明确没做什么

这轮我 **没有** 做这些：

1. 没有去抢你当前 active 代码文件
2. 没有改 `Primary.unity`
3. 没有把 resident actor 实体整批塞进 `Town`
4. 没有去动 runtime 迁回 contract
5. 没有去改 `CrowdDirector / Manifest`

## 你现在可以怎么理解 Town 的承接状态

当前更准确的口径是：

1. `Town` 的 resident scene-side 容器层已经站住
2. `Town` 的 7 个 carrier 已经从“纯语义名”变成了“粗粒度可承接空间位”
3. 但这还不是 resident actor 已经迁回完成
4. 也不是 runtime contract 已经迁回完成

所以它现在的阶段应该叫：

- **scene-side 第一刀已落地，后续 resident actor / runtime contract 仍待下一刀**

## 什么时候适合把球继续交回我

命中下面任一条时，就适合继续把球交回 `Town`：

1. 你准备真的把 resident actor 从“语义上已存在”推进到“Town scene-side 真承接”
2. 你准备把当前代理 runtime contract 往 `Town` 迁回
3. 你需要 `Town` 继续细化某个 anchor 的最终常驻级空间位，而不是只把它当导演临时锚

## 当前我对你的最短建议

你现在可以继续放心把 day1 的导演/正文层往下吃，因为：

- `Town` 已经不再只有空名字锚点

但如果你下一步开始要求：

- resident actor 真正在 `Town` 里驻留
- 或 contract 真正迁回

那时就别再把它停在“先代理一下”，应该正式把 scene-side / contract 球继续交回我。
