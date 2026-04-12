export type BlueprintRef = {
  sourceId: string;
  section: string;
};

export type BlueprintGroup = {
  semanticKey: string;
  title: string;
  kind: "project" | "module" | "system" | "scene" | "flow" | "rule" | "toolchain" | "governance" | "milestone" | "risk" | "source";
  status: "established" | "in_progress" | "partial" | "planned" | "parked" | "deprecated";
  stability: "canonical" | "stable" | "active" | "volatile" | "historical";
  summary: string;
  refs: BlueprintRef[];
};

export type MapBlueprint = {
  id: string;
  title: string;
  fileName: string;
  root: {
    semanticKey: string;
    title: string;
    kind: BlueprintGroup["kind"];
    status: BlueprintGroup["status"];
    stability: BlueprintGroup["stability"];
    summary: string;
    refs: BlueprintRef[];
  };
  groups: BlueprintGroup[];
};

const ref = (sourceId: string, section: string): BlueprintRef => ({ sourceId, section });

export const mapBlueprints: MapBlueprint[] = [
  {
    id: "sunset-project-map",
    title: "Sunset_项目总图",
    fileName: "Sunset_项目总图.xmind",
    root: {
      semanticKey: "sunset-project-root",
      title: "Sunset 项目总图",
      kind: "project",
      status: "established",
      stability: "canonical",
      summary: "Sunset 当前已经形成由长期主文驱动的项目母结构，主总图负责把世界观、经营成长、交互系统、剧情 NPC、表现层、工具链、AI 治理和进度优先级收成同一张长期可更新总图。",
      refs: [ref("plan-index", "## 1. 文档定位"), ref("plan-overview", "## 2. 项目是什么")]
    },
    groups: [
      {
        semanticKey: "overview-branch",
        title: "项目总览",
        kind: "module",
        status: "established",
        stability: "canonical",
        summary: "项目总览分支收项目定位、当前阶段、核心玩法闭环和跨场景主线，不再把短期治理现场直接冒充长期主文。",
        refs: [
          ref("plan-overview", "## 2. 项目是什么"),
          ref("plan-overview", "## 3. 项目阶段线"),
          ref("plan-overview", "## 5. 项目核心玩法闭环"),
          ref("workspace-weekly-receipt", "### 3.1 Day1 已从“开篇孤立剧情”推进到跨场景承接主线")
        ]
      },
      {
        semanticKey: "economy-branch",
        title: "经营成长",
        kind: "module",
        status: "established",
        stability: "canonical",
        summary: "经营成长分支收时间季节、资源层级、农田循环和工作台生产链，把资源博弈统一到聚落复兴主循环里。",
        refs: [
          ref("plan-economy", "## 2. 经营成长总闭环"),
          ref("plan-economy", "## 3. 时间、季节与世界节奏"),
          ref("plan-economy", "## 4. 资源节点、采集门槛与材料层级"),
          ref("plan-economy", "## 5. 农田、作物与农业循环"),
          ref("plan-economy", "## 6. 制作、工作台与资源转化")
        ]
      },
      {
        semanticKey: "interaction-branch",
        title: "交互系统",
        kind: "module",
        status: "established",
        stability: "canonical",
        summary: "交互系统分支收输入分发、状态锁、背包 Toolbar、放置、农田和箱子合同，当前真正收口的是边界语言。",
        refs: [
          ref("plan-interaction", "## 3. 输入分发与全局状态锁"),
          ref("plan-interaction", "## 4. 背包、Toolbar 与物品操作"),
          ref("plan-interaction", "## 5. Toolbar、手持工具与动作触发"),
          ref("plan-interaction", "## 6. 放置系统"),
          ref("plan-interaction", "## 8. 箱子、存储与面板互斥"),
          ref("plan-interaction", "## 10. 全局优先级与打断规则")
        ]
      },
      {
        semanticKey: "story-npc-branch",
        title: "剧情与 NPC",
        kind: "module",
        status: "in_progress",
        stability: "stable",
        summary: "剧情与 NPC 分支收 spring-day1、正式对白消费、resident fallback 和常驻居民化，是当前跨场景主线的重要承接面。",
        refs: [
          ref("plan-story-npc", "## 3. 为什么 `spring-day1` 是当前叙事主入口"),
          ref("plan-story-npc", "## 4. `spring-day1` 当前正式流程"),
          ref("plan-story-npc", "## 5. Day1 的关键叙事机制"),
          ref("plan-story-npc", "## 6. NPC 系统的当前正式结构"),
          ref("plan-story-npc", "## 7. NPC 的场景化、轻交互与关系成长")
        ]
      },
      {
        semanticKey: "presentation-branch",
        title: "表现层",
        kind: "module",
        status: "in_progress",
        stability: "stable",
        summary: "表现层分支收场景、UI、动画、导航、遮挡和昼夜光影，目标是把玩家真实看到的世界感和可读性统一起来。",
        refs: [
          ref("plan-presentation", "## 3. 场景与 Tilemap 表达"),
          ref("plan-presentation", "## 4. UI 与世界提示"),
          ref("plan-presentation", "## 5. 玩家、工具与 NPC 动画"),
          ref("plan-presentation", "## 6. 导航、避让与遮挡联动"),
          ref("plan-presentation", "## 7. 昼夜光影与世界提示"),
          ref("plan-presentation", "## 8. 当前统一视觉语言")
        ]
      },
      {
        semanticKey: "toolchain-branch",
        title: "工具链",
        kind: "toolchain",
        status: "established",
        stability: "canonical",
        summary: "工具链分支收数据资产、Editor 工具、场景验证和 Story/UI 支撑工具，强调工具已经进入真实生产链。",
        refs: [
          ref("plan-toolchain", "## 2. 当前策划侧落地链路"),
          ref("plan-toolchain", "## 3. 数据资产结构"),
          ref("plan-toolchain", "## 4. SO 规范、ID 体系与数据治理"),
          ref("plan-toolchain", "## 5. 当前 Editor 工具链"),
          ref("plan-toolchain", "## 7. 当前工具链的正式使用方式")
        ]
      },
      {
        semanticKey: "governance-branch",
        title: "AI 治理",
        kind: "governance",
        status: "in_progress",
        stability: "stable",
        summary: "AI 治理分支收规则栈、工作区栈、memory 审计、thread-state 和 CLI/MCP 边界，强调治理已经是正式工程层。",
        refs: [
          ref("plan-ai-governance", "## 3. 当前正式治理架构"),
          ref("plan-ai-governance", "## 4. 当前角色与输出口径分工"),
          ref("plan-ai-governance", "## 5. 当前标准推进流程"),
          ref("plan-ai-governance", "## 6. Skills、本地化适配与 Unity MCP"),
          ref("workspace-three-dimensions", "### 5.6 验证体系：CLI 优先，MCP 兜底")
        ]
      },
      {
        semanticKey: "progress-branch",
        title: "进度与优先级",
        kind: "milestone",
        status: "in_progress",
        stability: "stable",
        summary: "进度与优先级分支收模块总表、优先扩展顺序和本周推进主轴，让系统推进始终围绕最值钱的一刀排优先级。",
        refs: [
          ref("plan-progress", "## 2. 状态口径"),
          ref("plan-progress", "## 3. 模块总表"),
          ref("plan-progress", "## 4. 当前优先扩展顺序"),
          ref("workspace-weekly-receipt", "## 3. 本周最重要的项目级成果，不是单点功能，而是 6 条新基线")
        ]
      }
    ]
  },
  {
    id: "sunset-system-landscape-map",
    title: "Sunset_系统版图",
    fileName: "Sunset_系统版图.xmind",
    root: {
      semanticKey: "systems-landscape-root",
      title: "Sunset 系统版图",
      kind: "project",
      status: "established",
      stability: "canonical",
      summary: "系统版图从系统策划视角展示 Sunset：生活经营、资源成长、剧情交互、表现层、工具链与治理怎样挂在同一项目总循环里。",
      refs: [ref("plan-overview", "## 6. 三大系统版图"), ref("workspace-three-dimensions", "## 6. 三个维度之间的共同主线")]
    },
    groups: [
      {
        semanticKey: "landscape-life-economy",
        title: "生活经营与资源成长",
        kind: "module",
        status: "established",
        stability: "canonical",
        summary: "经营成长和资源成长共同定义 Sunset 的生产闭环，时间季节、资源层级、农田和工作台是这一层的正式承载。",
        refs: [
          ref("plan-overview", "### 6.1 生活经营模块"),
          ref("plan-overview", "### 6.2 资源成长模块"),
          ref("plan-economy", "## 2. 经营成长总闭环")
        ]
      },
      {
        semanticKey: "landscape-story-interaction",
        title: "剧情交互与跨场景推进",
        kind: "flow",
        status: "in_progress",
        stability: "stable",
        summary: "剧情交互模块当前不是单看对白资产，而是围绕 spring-day1、跨场景承接、resident 化和状态边界共同推进。",
        refs: [
          ref("plan-overview", "### 6.3 剧情交互模块"),
          ref("workspace-weekly-receipt", "### 3.1 Day1 已从“开篇孤立剧情”推进到跨场景承接主线"),
          ref("workspace-weekly-receipt", "### 3.3 NPC 从“先多做几个人”变成“formal consumed -> resident fallback”的长期结构")
        ]
      },
      {
        semanticKey: "landscape-presentation",
        title: "表现层与玩家面",
        kind: "system",
        status: "in_progress",
        stability: "stable",
        summary: "场景、UI、动画、导航和光影共同组成玩家实际感知到的表现层，这一层已经进入正式验收口径。",
        refs: [ref("plan-presentation", "## 2. 表现层总结构"), ref("plan-presentation", "## 9. 当前正式验收基线")]
      },
      {
        semanticKey: "landscape-tooling",
        title: "工具链与数据生产",
        kind: "toolchain",
        status: "established",
        stability: "stable",
        summary: "数据资产、Editor 工具和验证工具已经形成 Sunset 的策划侧工程生产链，不再只是辅助脚本集合。",
        refs: [ref("plan-toolchain", "## 2. 当前策划侧落地链路"), ref("plan-toolchain", "## 5. 当前 Editor 工具链")]
      },
      {
        semanticKey: "landscape-governance",
        title: "AI 治理与验证秩序",
        kind: "governance",
        status: "in_progress",
        stability: "stable",
        summary: "AI 治理层负责把线程推进、验证顺序和验收口径工程化，已经开始真实改变开发节奏。",
        refs: [
          ref("workspace-weekly-receipt", "### 3.6 AI 治理与工具链从“能用”推进到“开始反过来改变开发节奏”"),
          ref("plan-ai-governance", "## 5. 当前标准推进流程")
        ]
      }
    ]
  },
  {
    id: "sunset-story-npc-map",
    title: "Sunset_剧情与NPC",
    fileName: "Sunset_剧情与NPC.xmind",
    root: {
      semanticKey: "story-npc-map-root",
      title: "Sunset 剧情与 NPC",
      kind: "module",
      status: "in_progress",
      stability: "stable",
      summary: "剧情与 NPC 主题图聚焦 spring-day1、正式对白消费、resident fallback 和常驻居民场景化，强调这条线当前是结构站住但体验仍待继续验证。",
      refs: [ref("plan-story-npc", "## 10. 当前正式收口")]
    },
    groups: [
      { semanticKey: "story-map-positioning", title: "定位", kind: "flow", status: "established", stability: "stable", summary: "当前叙事主入口仍是 spring-day1，它承担开篇认识世界、引导玩家进入聚落，并把后续系统绑定到一条正式可验证切片里。", refs: [ref("plan-story-npc", "## 2. 当前叙事定位"), ref("plan-story-npc", "## 3. 为什么 `spring-day1` 是当前叙事主入口")] },
      { semanticKey: "story-map-formal-structure", title: "当前正式结构", kind: "system", status: "in_progress", stability: "stable", summary: "正式结构由 Day1 阶段流、关键叙事机制、四条 NPC 主线和常驻居民阵容共同构成，已经不只是对话节点集合。", refs: [ref("plan-story-npc", "## 4. `spring-day1` 当前正式流程"), ref("plan-story-npc", "## 6. NPC 系统的当前正式结构")] },
      { semanticKey: "story-map-rules", title: "关键规则", kind: "rule", status: "in_progress", stability: "stable", summary: "当前最关键规则是乱码恢复、首次状态条、工作台闪回，以及 formal consumed -> resident fallback 这条长期 NPC 语义合同。", refs: [ref("plan-story-npc", "## 5. Day1 的关键叙事机制"), ref("workspace-weekly-receipt", "### 5.7 Day1 的 NPC 方向已改判为“驻村常驻化”")] },
      { semanticKey: "story-map-carriers", title: "场景/系统/对象承载", kind: "scene", status: "in_progress", stability: "stable", summary: "剧情与 NPC 当前由 Town、Primary、Home 跨场景承接、NPC 常驻居民阵容和场景化轻交互一起承载。", refs: [ref("plan-story-npc", "### 4.5 当前阶段补记：Day1 的跨场景承载关系"), ref("plan-story-npc", "## 7. NPC 的场景化、轻交互与关系成长")] },
      { semanticKey: "story-map-closure", title: "当前收口", kind: "milestone", status: "in_progress", stability: "stable", summary: "这条线当前已经从“能讲 Day1”推进到“把 NPC 常驻化和正式对白消费结构压成正式合同”，但还没进入玩家真实视面的完整过线状态。", refs: [ref("plan-story-npc", "## 10. 当前正式收口"), ref("workspace-weekly-receipt", "## 8. 一句话收束")] },
      { semanticKey: "story-map-risks", title: "风险与未收口项", kind: "risk", status: "partial", stability: "active", summary: "当前仍未收口的重点在于真实场景写入、玩家面体验验证和 resident 结构的后半链承接。", refs: [ref("plan-progress", "## 4. 当前优先扩展顺序"), ref("workspace-weekly-receipt", "### 6.1 从“线程自己说完成”改成“必须过用户真实体验反馈”")] },
      { semanticKey: "story-map-sources", title: "主要依据", kind: "source", status: "established", stability: "stable", summary: "剧情与 NPC 主题图主要依据长期主文、项目总览周回执和三维度母卷，不直接吸收一次性线程回执原话。", refs: [ref("plan-story-npc", "## 11. 当前主要依据"), ref("workspace-three-dimensions", "### 3.5 NPC 常驻居民化与剧情消费结构")] }
    ]
  },
  {
    id: "sunset-interaction-map",
    title: "Sunset_交互与状态边界",
    fileName: "Sunset_交互与状态边界.xmind",
    root: {
      semanticKey: "interaction-map-root",
      title: "Sunset 交互与状态边界",
      kind: "module",
      status: "established",
      stability: "canonical",
      summary: "交互与状态边界主题图聚焦输入归属、状态锁、背包/Toolbar、放置、农田和箱子互斥，强调当前真正收口的是跨系统边界语言。",
      refs: [ref("plan-interaction", "## 11. 当前正式收口")]
    },
    groups: [
      { semanticKey: "interaction-map-positioning", title: "定位", kind: "flow", status: "established", stability: "canonical", summary: "交互层的目标不是把所有功能拼在一起，而是用统一状态边界保护玩家手感、输入归属和跨系统一致性。", refs: [ref("plan-interaction", "## 2. 交互层总原则")] },
      { semanticKey: "interaction-map-formal-structure", title: "当前正式结构", kind: "system", status: "established", stability: "canonical", summary: "当前正式结构由输入分发与全局状态锁、背包与 Toolbar、工具动作、放置、农田和箱子状态机共同组成。", refs: [ref("plan-interaction", "## 3. 输入分发与全局状态锁"), ref("plan-interaction", "## 4. 背包、Toolbar 与物品操作"), ref("plan-interaction", "## 6. 放置系统"), ref("plan-interaction", "## 8. 箱子、存储与面板互斥")] },
      { semanticKey: "interaction-map-rules", title: "关键规则", kind: "rule", status: "established", stability: "stable", summary: "关键规则包括输入优先级、动作锁、状态互斥、取消/暂停/恢复矩阵，以及放置合法性和箱子面板互斥合同。", refs: [ref("plan-interaction", "## 9. 交互层的跨系统统一口径"), ref("plan-interaction", "## 10. 全局优先级与打断规则")] },
      { semanticKey: "interaction-map-carriers", title: "场景/系统/对象承载", kind: "scene", status: "in_progress", stability: "stable", summary: "交互边界当前由 Town/Home 场景合同、玩家唯一实例、背包面板、箱子对象和放置/农田运行时一起承载。", refs: [ref("plan-interaction", "### 7.4 当前 `Town / Home` 场景合同"), ref("workspace-weekly-receipt", "### 5.3 玩家走向跨场景唯一实例")] },
      { semanticKey: "interaction-map-closure", title: "当前收口", kind: "milestone", status: "established", stability: "stable", summary: "交互层已经把“修某个功能 bug”推进成“先把边界语言说清，再做功能扩张”的正式口径。", refs: [ref("plan-interaction", "## 11. 当前正式收口"), ref("workspace-weekly-receipt", "### 6.5 从“讲边界”改成“边界说清后立刻落地”")] },
      { semanticKey: "interaction-map-risks", title: "风险与未收口项", kind: "risk", status: "partial", stability: "active", summary: "当前未收口重点不在边界是否存在，而在不同场景和运行态持续变化时，这些边界能否继续稳定不回退。", refs: [ref("plan-progress", "## 4. 当前优先扩展顺序"), ref("workspace-weekly-receipt", "### 3.5 存档、云影、持久化玩家三条链开始汇入“运行态延续”这一个主题")] },
      { semanticKey: "interaction-map-sources", title: "主要依据", kind: "source", status: "established", stability: "stable", summary: "本图主要依据交互系统主文、周回执和三维度母卷中的交互收口事实池。", refs: [ref("plan-interaction", "## 12. 当前主要依据"), ref("workspace-three-dimensions", "### 3.3 交互系统与状态边界收口")] }
    ]
  },
  {
    id: "sunset-presentation-map",
    title: "Sunset_表现层与场景链",
    fileName: "Sunset_表现层与场景链.xmind",
    root: {
      semanticKey: "presentation-map-root",
      title: "Sunset 表现层与场景链",
      kind: "module",
      status: "in_progress",
      stability: "stable",
      summary: "表现层与场景链主题图聚焦场景、UI、动画、导航、遮挡和光影如何共同构成玩家面真实体验；这条线结构已清，但最终观感仍须持续终验。",
      refs: [ref("plan-presentation", "## 10. 当前正式收口")]
    },
    groups: [
      { semanticKey: "presentation-map-positioning", title: "定位", kind: "scene", status: "established", stability: "stable", summary: "表现层的目标不是单点渲染效果，而是保证场景、UI、动画和光影共同服务于玩家理解与世界连续性。", refs: [ref("plan-presentation", "## 2. 表现层总结构")] },
      { semanticKey: "presentation-map-formal-structure", title: "当前正式结构", kind: "system", status: "in_progress", stability: "stable", summary: "当前正式结构包括场景与 Tilemap 表达、UI 与世界提示、玩家与 NPC 动画、导航遮挡联动以及昼夜光影系统。", refs: [ref("plan-presentation", "## 3. 场景与 Tilemap 表达"), ref("plan-presentation", "## 4. UI 与世界提示"), ref("plan-presentation", "## 5. 玩家、工具与 NPC 动画"), ref("plan-presentation", "## 6. 导航、避让与遮挡联动"), ref("plan-presentation", "## 7. 昼夜光影与世界提示")] },
      { semanticKey: "presentation-map-rules", title: "关键规则", kind: "rule", status: "in_progress", stability: "stable", summary: "关键规则是跨场景表现合同、玩家面可读性、导航与遮挡必须联动，以及视觉语言必须尊重空间、焦点和动作反馈。", refs: [ref("plan-presentation", "### 2.4 当前跨场景表现合同"), ref("plan-presentation", "## 8. 当前统一视觉语言")] },
      { semanticKey: "presentation-map-carriers", title: "场景/系统/对象承载", kind: "scene", status: "in_progress", stability: "stable", summary: "Primary、Town、Home 与对应 UI/提示层、动画控制器、导航网格和昼夜系统共同承载了当前表现层合同。", refs: [ref("plan-presentation", "### 2.2 当前直接场景入口"), ref("plan-presentation", "### 7.1 当前承载体")] },
      { semanticKey: "presentation-map-closure", title: "当前收口", kind: "milestone", status: "in_progress", stability: "stable", summary: "表现层当前已经进入正式验收基线阶段，玩家面 UI、导航遮挡和世界表现的合同已明确，但真实好不好看还必须继续靠终验说话。", refs: [ref("plan-presentation", "## 9. 当前正式验收基线"), ref("plan-presentation", "## 10. 当前正式收口")] },
      { semanticKey: "presentation-map-risks", title: "风险与未收口项", kind: "risk", status: "partial", stability: "active", summary: "最大风险仍是玩家真实视面和跨场景连续性：结构成立并不等于最终观感已经足够稳定。", refs: [ref("plan-progress", "## 4. 当前优先扩展顺序"), ref("workspace-weekly-receipt", "### 3.4 Workbench / Prompt / Tooltip / 气泡进入了真正的“玩家面终验”阶段")] },
      { semanticKey: "presentation-map-sources", title: "主要依据", kind: "source", status: "established", stability: "stable", summary: "本图主要依据表现层主文、周回执和三维度母卷中关于场景、Tilemap、动画和玩家面的正式事实池。", refs: [ref("plan-presentation", "## 11. 当前主要依据"), ref("workspace-three-dimensions", "### 4.5 动画、表现层与玩家面验收")] }
    ]
  },
  {
    id: "sunset-toolchain-governance-map",
    title: "Sunset_工具链与AI治理",
    fileName: "Sunset_工具链与AI治理.xmind",
    root: {
      semanticKey: "toolchain-governance-map-root",
      title: "Sunset 工具链与 AI 治理",
      kind: "toolchain",
      status: "in_progress",
      stability: "stable",
      summary: "工具链与 AI 治理主题图展示 Sunset 已形成的策划侧生产链、验证链和多线程治理秩序，强调工具必须真实改变开发行为。",
      refs: [ref("plan-toolchain", "## 8. 当前正式收口"), ref("plan-ai-governance", "## 7. 当前正式收口")]
    },
    groups: [
      { semanticKey: "toolchain-governance-positioning", title: "定位", kind: "toolchain", status: "established", stability: "stable", summary: "这条线的目标不是堆工具数量，而是让数据、Editor、验证、CLI/MCP 和治理流程真正服务于项目推进吞吐。", refs: [ref("plan-toolchain", "## 2. 当前策划侧落地链路"), ref("workspace-weekly-receipt", "### 6.4 从“工具能跑就行”改成“工具必须改变真实开发行为”")] },
      { semanticKey: "toolchain-governance-formal-structure", title: "当前正式结构", kind: "governance", status: "in_progress", stability: "stable", summary: "当前正式结构由数据资产与 SO 治理、Editor 工具链、场景验证工具、规则栈、工作区栈、thread-state 和验证顺序共同组成。", refs: [ref("plan-toolchain", "## 3. 数据资产结构"), ref("plan-toolchain", "## 5. 当前 Editor 工具链"), ref("plan-ai-governance", "## 3. 当前正式治理架构")] },
      { semanticKey: "toolchain-governance-rules", title: "关键规则", kind: "rule", status: "established", stability: "stable", summary: "关键规则包括 CLI first、direct MCP last-resort、保底六点卡、No-Red 证据卡和典狱长/看守长分工口径。", refs: [ref("plan-ai-governance", "## 4. 当前角色与输出口径分工"), ref("plan-ai-governance", "## 6. Skills、本地化适配与 Unity MCP")] },
      { semanticKey: "toolchain-governance-carriers", title: "场景/系统/对象承载", kind: "toolchain", status: "in_progress", stability: "stable", summary: "当前由数据资产目录、Editor 菜单工具、CLI 入口、skills、本地 rules 和工作区文档共同承载这条工具/治理链。", refs: [ref("plan-toolchain", "## 7. 当前工具链的正式使用方式"), ref("workspace-material-pack", "## 6. Unity、工具链与实际落地事实池")] },
      { semanticKey: "toolchain-governance-closure", title: "当前收口", kind: "milestone", status: "in_progress", stability: "stable", summary: "工具链与治理已经从“可用”推进到“开始反过来改变开发节奏”，这是一条正式成立的生产能力。", refs: [ref("workspace-weekly-receipt", "### 3.6 AI 治理与工具链从“能用”推进到“开始反过来改变开发节奏”"), ref("plan-ai-governance", "## 7. 当前正式收口")] },
      { semanticKey: "toolchain-governance-risks", title: "风险与未收口项", kind: "risk", status: "partial", stability: "active", summary: "当前风险不在于有没有工具，而在于哪些工具已经进入生产主链、哪些仍是实验线，以及规则变更能否同步到所有真实入口。", refs: [ref("workspace-engineering-prompt", "### 2.1 规则失真是第一优先级"), ref("workspace-engineering-prompt", "### 2.2 不要再把“最近一周的治理快照”直接冒充长期主文")] },
      { semanticKey: "toolchain-governance-sources", title: "主要依据", kind: "source", status: "established", stability: "stable", summary: "本图主要依据工具链主文、AI 治理主文、项目素材总包和三维度母卷中的正式工具/治理事实池。", refs: [ref("plan-toolchain", "## 9. 当前主要依据"), ref("plan-ai-governance", "## 8. 当前主要依据"), ref("workspace-three-dimensions", "### 5.7 AI 治理已经真实改变了项目推进方式")] }
    ]
  },
  {
    id: "sunset-progress-map",
    title: "Sunset_进度与优先级",
    fileName: "Sunset_进度与优先级.xmind",
    root: {
      semanticKey: "progress-map-root",
      title: "Sunset 进度与优先级",
      kind: "milestone",
      status: "in_progress",
      stability: "stable",
      summary: "进度与优先级主题图把模块状态、当前优先顺序、本周推进主轴和方法论变化集中展示，避免项目推进失去主次。",
      refs: [ref("plan-progress", "## 3. 模块总表")]
    },
    groups: [
      { semanticKey: "progress-map-positioning", title: "定位", kind: "milestone", status: "established", stability: "stable", summary: "当前进度图不只是记录模块是否存在，而是帮助持续裁定什么该先压深、什么暂时停在 targeted probe，什么仍待真实体验终验。", refs: [ref("plan-progress", "## 1. 文档目标"), ref("plan-progress", "## 2. 状态口径")] },
      { semanticKey: "progress-map-formal-structure", title: "当前正式结构", kind: "milestone", status: "established", stability: "canonical", summary: "当前正式结构由模块总表、优先扩展顺序和维护要求组成，所有模块的完成语言都要回到这张表上统一。", refs: [ref("plan-progress", "## 3. 模块总表"), ref("plan-progress", "## 5. 当前维护要求")] },
      { semanticKey: "progress-map-rules", title: "关键规则", kind: "rule", status: "established", stability: "stable", summary: "关键规则是优先扩展顺序必须围绕跨场景主线、玩家面终验和结构收口来排，而不是平均给所有子系统分配注意力。", refs: [ref("plan-progress", "## 4. 当前优先扩展顺序"), ref("workspace-weekly-receipt", "### 6.3 从“功能面堆更多”改成“先把最值钱的一刀压深”")] },
      { semanticKey: "progress-map-carriers", title: "场景/系统/对象承载", kind: "flow", status: "in_progress", stability: "stable", summary: "当前最关键的承载面已经集中到跨场景主线、resident 化、玩家面 UI、存档/持久化与表现层连续性这些系统联动面上。", refs: [ref("workspace-weekly-receipt", "## 3. 本周最重要的项目级成果，不是单点功能，而是 6 条新基线")] },
      { semanticKey: "progress-map-closure", title: "当前收口", kind: "milestone", status: "in_progress", stability: "stable", summary: "当前项目的真实收口方式已经是多条系统线围绕同一主体验继续汇流，而不是各自完成就算交差。", refs: [ref("workspace-weekly-receipt", "## 8. 一句话收束"), ref("plan-overview", "### 7.4 当前阶段的收口条件补记")] },
      { semanticKey: "progress-map-risks", title: "风险与未收口项", kind: "risk", status: "partial", stability: "active", summary: "当前最大不确定性仍是结构成立和真实体验过线之间的距离，很多线已经有正式合同，但还没完成玩家视面的终验闭环。", refs: [ref("plan-progress", "## 4. 当前优先扩展顺序"), ref("workspace-weekly-receipt", "### 6.1 从“线程自己说完成”改成“必须过用户真实体验反馈”")] },
      { semanticKey: "progress-map-sources", title: "主要依据", kind: "source", status: "established", stability: "stable", summary: "本图主要依据进度总表、总览主文和用户本周项目级回执。", refs: [ref("plan-progress", "## 3. 模块总表"), ref("workspace-weekly-receipt", "## 1. 这份回执要解决什么")] }
    ]
  }
];
