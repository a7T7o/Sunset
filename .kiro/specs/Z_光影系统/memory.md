# Z_光影系统 工作区记忆

## 工作区信息
- 创建时间：2026-02-19
- 目标：为游戏添加全局光影特效，首要目标是昼夜更替系统

---

## 会话1（2026-02-19）

### 背景
用户反馈游戏场景单薄，无论几点钟都是同样的颜色和光线，希望实现昼夜更替特效。

### 讨论内容
- 调研了项目现状：使用内置渲染管线（非 URP），无任何 Light2D 或光照代码
- 已有 TimeManager 支持 OnHourChanged 事件、IsDaytime()/IsNighttime()、GetDayProgress()
- 已有 CloudShadowManager 云朵阴影系统，Sorting Layer 预留了 Effects 和 CloudShadow 层

### 方案分析
- 路线 A：URP + Light2D — 需迁移渲染管线，风险极高，不推荐
- 路线 B：全屏颜色叠加（Overlay 方案）— 零迁移风险，星露谷同款，推荐 ⭐

### 推荐方案（路线 B）要点
1. DayNightManager 订阅 TimeManager 时间事件
2. Gradient 渐变色定义 24 小时颜色曲线
3. 全屏 Overlay（Multiply 混合模式）实现色调变化
4. 季节联动：不同季节颜色曲线不同
5. 可选：夜晚建筑窗户发光精灵

### 状态
- 等待用户确认方案后创建正式 spec 文档

### 修改文件
- 无（纯讨论）
