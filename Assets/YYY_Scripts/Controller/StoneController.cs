using UnityEngine;
using System.Collections.Generic;
using FarmGame.Combat;
using FarmGame.Data;
using FarmGame.Utils;

/// <summary>
/// 石头/矿石控制器
/// 
/// 核心特性：
/// - 4阶段系统（M1-M4），只能被挖掘变小，不会生长
/// - 溢出伤害机制：伤害可以跨阶段传递
/// - 差值掉落：每个阶段掉落当前与下一阶段的矿物/石料差值
/// - 材料等级限制：不同镐子能获取不同矿物，但所有镐子都能获得石料
/// 
/// Sprite命名规范：Stone_{OreType}_{Stage}_{OreIndex}
/// 例如：Stone_C1_M1_4（铜矿，M1阶段，含量4）
/// </summary>
public class StoneController : MonoBehaviour, IResourceNode
{
    #region 序列化字段 - 阶段配置
    [Header("━━━━ 阶段配置 ━━━━")]
    [Tooltip("4个阶段的配置")]
    [SerializeField] private StoneStageConfig[] stageConfigs;
    
    [Tooltip("当前阶段")]
    [SerializeField] private StoneStage currentStage = StoneStage.M1;
    
    [Tooltip("矿物类型")]
    [SerializeField] private OreType oreType = OreType.None;
    
    [Tooltip("矿物含量指数（0-4）")]
    [Range(0, 4)]
    [SerializeField] private int oreIndex = 0;
    #endregion
    
    #region 序列化字段 - 血量
    [Header("━━━━ 血量状态 ━━━━")]
    [Tooltip("当前血量")]
    [SerializeField] private int currentHealth = 36;
    #endregion
    
    #region 序列化字段 - Sprite配置
    [Header("━━━━ Sprite配置 ━━━━")]
    [Tooltip("SpriteRenderer组件")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    [Tooltip("Sprite资源路径前缀（用于动态加载）")]
    [SerializeField] private string spritePathPrefix = "Sprites/Stone/";
    #endregion
    
    #region 序列化字段 - 掉落配置
    [Header("━━━━ 掉落配置 ━━━━")]
    [Tooltip("铜矿掉落物品")]
    [SerializeField] private ItemData copperOreItem;
    
    [Tooltip("铁矿掉落物品")]
    [SerializeField] private ItemData ironOreItem;
    
    [Tooltip("金矿掉落物品")]
    [SerializeField] private ItemData goldOreItem;
    
    [Tooltip("石料掉落物品")]
    [SerializeField] private ItemData stoneItem;
    
    [Tooltip("掉落物散布半径")]
    [Range(0.5f, 2f)]
    [SerializeField] private float dropSpreadRadius = 1f;
    #endregion
    
    #region 序列化字段 - 音效
    [Header("━━━━ 音效设置 ━━━━")]
    [Tooltip("挖掘音效")]
    [SerializeField] private AudioClip mineHitSound;
    
    [Tooltip("破碎音效")]
    [SerializeField] private AudioClip breakSound;
    
    [Tooltip("等级不足音效")]
    [SerializeField] private AudioClip tierInsufficientSound;
    
    [Tooltip("音效音量")]
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.8f;
    #endregion
    
    #region 序列化字段 - 调试
    [Header("━━━━ 调试 ━━━━")]
    [SerializeField] private bool showDebugInfo = false;
    #endregion
    
    #region 私有字段
    private bool isDepleted = false;
    private int lastHitPickaxeTier = 0;
    private bool lastHitCanGetOre = false;
    #endregion
    
    #region 属性
    /// <summary>当前阶段配置</summary>
    public StoneStageConfig CurrentStageConfig => GetStageConfig(currentStage);
    
    /// <summary>是否为最终阶段</summary>
    public bool IsFinalStage => CurrentStageConfig?.isFinalStage ?? true;
    #endregion
    
    #region Unity生命周期
    private void Awake()
    {
        // 初始化阶段配置
        if (stageConfigs == null || stageConfigs.Length != 4)
        {
            stageConfigs = StoneStageConfigFactory.CreateDefaultConfigs();
        }
    }
    
    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        if (spriteRenderer == null)
        {
            Debug.LogError($"[StoneController] {gameObject.name} 缺少SpriteRenderer组件！");
            enabled = false;
            return;
        }
        
        // 初始化血量
        InitializeHealth();
        
        // 更新显示
        UpdateSprite();
        
        // 注册到资源节点注册表
        if (ResourceNodeRegistry.Instance != null)
        {
            ResourceNodeRegistry.Instance.Register(this, gameObject.GetInstanceID());
        }
    }
    
    private void OnDestroy()
    {
        if (ResourceNodeRegistry.Instance != null)
        {
            ResourceNodeRegistry.Instance.Unregister(gameObject.GetInstanceID());
        }
    }
    #endregion
    
    #region 初始化
    /// <summary>
    /// 初始化血量（根据当前阶段）
    /// </summary>
    private void InitializeHealth()
    {
        var config = CurrentStageConfig;
        if (config != null)
        {
            currentHealth = config.health;
        }
    }
    
    /// <summary>
    /// 获取指定阶段的配置
    /// </summary>
    private StoneStageConfig GetStageConfig(StoneStage stage)
    {
        int index = (int)stage;
        if (stageConfigs == null || index < 0 || index >= stageConfigs.Length)
        {
            return null;
        }
        return stageConfigs[index];
    }
    #endregion
    
    #region IResourceNode 接口实现
    public string ResourceTag => "Rock";
    
    public bool IsDepleted => isDepleted;
    
    /// <summary>
    /// 检查是否接受此工具类型（只有镐子能有效挖掘）
    /// </summary>
    public bool CanAccept(ToolHitContext ctx)
    {
        if (isDepleted) return false;
        return ctx.toolType == ToolType.Pickaxe;
    }
    
    /// <summary>
    /// 处理命中效果
    /// </summary>
    public void OnHit(ToolHitContext ctx)
    {
        if (isDepleted) return;
        
        // 只有镐子能有效挖掘
        if (ctx.toolType != ToolType.Pickaxe)
        {
            PlayShakeEffect();
            if (showDebugInfo)
                Debug.Log($"<color=gray>[StoneController] {gameObject.name} 被非镐子工具击中，只抖动</color>");
            return;
        }
        
        // 获取镐子材料等级
        int pickaxeTier = GetPickaxeTier(ctx);
        lastHitPickaxeTier = pickaxeTier;
        
        // 检查是否能获取矿物
        lastHitCanGetOre = MaterialTierHelper.CanMineOre(pickaxeTier, oreType);
        
        // 尝试消耗精力
        float energyCost = GetEnergyCost(ctx);
        if (!TryConsumeEnergy(energyCost))
        {
            PlayShakeEffect();
            if (showDebugInfo)
                Debug.Log($"<color=yellow>[StoneController] {gameObject.name} 精力不足</color>");
            return;
        }
        
        // 计算伤害
        int damage = Mathf.Max(1, Mathf.RoundToInt(ctx.baseDamage));
        
        // 播放挖掘音效
        PlayMineHitSound();
        
        // 扣血
        TakeDamage(damage);
        
        if (showDebugInfo)
        {
            string oreStatus = lastHitCanGetOre ? "可获取矿物" : "只能获取石料";
            Debug.Log($"<color=yellow>[StoneController] {gameObject.name} 受到 {damage} 点伤害，剩余血量 {currentHealth}，{oreStatus}</color>");
        }
    }
    
    public Bounds GetBounds()
    {
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            return spriteRenderer.bounds;
        }
        return new Bounds(GetPosition(), Vector3.one * 0.5f);
    }
    
    /// <summary>
    /// 获取碰撞体边界（用于精确命中检测）
    /// 返回 Collider bounds，无 Collider 时回退到 Sprite bounds
    /// </summary>
    public Bounds GetColliderBounds()
    {
        // 优先使用 Collider2D 的 bounds
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && collider.enabled)
        {
            return collider.bounds;
        }
        
        // 检查父物体的 CompositeCollider2D
        if (transform.parent != null)
        {
            var compositeCollider = transform.parent.GetComponent<CompositeCollider2D>();
            if (compositeCollider != null && compositeCollider.enabled)
            {
                return compositeCollider.bounds;
            }
        }
        
        // 回退到 Sprite bounds
        return GetBounds();
    }
    
    public Vector3 GetPosition()
    {
        return transform.parent != null ? transform.parent.position : transform.position;
    }
    #endregion
    
    #region 伤害系统
    /// <summary>
    /// 处理伤害（含溢出）
    /// </summary>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            int overflow = -currentHealth;
            HandleStageTransition(overflow);
        }
        else
        {
            PlayShakeEffect();
        }
    }
    
    /// <summary>
    /// 处理阶段转换（含溢出伤害）
    /// </summary>
    private void HandleStageTransition(int overflowDamage)
    {
        var config = CurrentStageConfig;
        if (config == null)
        {
            DestroyStone();
            return;
        }
        
        // 最终阶段：直接销毁
        if (config.isFinalStage)
        {
            SpawnFinalDrops();
            GrantExperience(true);
            DestroyStone();
            return;
        }
        
        // 计算新的含量指数
        int newOreIndex = config.decreaseOreIndexOnTransition 
            ? Mathf.Max(0, oreIndex - 1) 
            : oreIndex;
        
        // 计算并掉落差值矿物（如果镐子等级足够）
        if (lastHitCanGetOre)
        {
            int oreDrop = StoneDropConfig.CalculateOreDropAmount(
                currentStage, oreIndex, 
                config.nextStage, newOreIndex
            );
            
            if (oreDrop > 0)
            {
                SpawnOreDrops(oreDrop);
            }
        }
        
        // 计算并掉落差值石料（所有镐子都能获得）
        int stoneDrop = StoneDropConfig.CalculateStoneDropAmount(currentStage, config.nextStage);
        if (stoneDrop > 0)
        {
            SpawnStoneDrops(stoneDrop);
        }
        
        // 给予经验
        GrantExperience(false);
        
        // 播放破碎音效
        PlayBreakSound();
        
        // 转换到下一阶段
        StoneStage previousStage = currentStage;
        currentStage = config.nextStage;
        oreIndex = newOreIndex;
        
        // 初始化新阶段血量
        InitializeHealth();
        
        // 更新 Sprite
        UpdateSprite();
        
        if (showDebugInfo)
            Debug.Log($"<color=orange>[StoneController] {gameObject.name} 从 {previousStage} 转换到 {currentStage}，新含量指数 {oreIndex}</color>");
        
        // 应用溢出伤害
        if (overflowDamage > 0)
        {
            TakeDamage(overflowDamage);
        }
    }
    #endregion
    
    #region 掉落系统
    /// <summary>
    /// 生成矿物掉落
    /// </summary>
    private void SpawnOreDrops(int amount)
    {
        if (amount <= 0) return;
        
        ItemData oreItem = GetOreItem();
        if (oreItem == null) return;
        
        Vector3 dropOrigin = GetPosition();
        
        if (WorldSpawnService.Instance != null)
        {
            WorldSpawnService.Instance.SpawnMultiple(
                oreItem,
                0, // 品质
                amount,
                dropOrigin,
                dropSpreadRadius
            );
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=lime>[StoneController] {gameObject.name} 掉落 {amount} 个 {oreItem.itemName}</color>");
    }
    
    /// <summary>
    /// 生成石料掉落
    /// </summary>
    private void SpawnStoneDrops(int amount)
    {
        if (amount <= 0 || stoneItem == null) return;
        
        Vector3 dropOrigin = GetPosition();
        
        if (WorldSpawnService.Instance != null)
        {
            WorldSpawnService.Instance.SpawnMultiple(
                stoneItem,
                0, // 品质
                amount,
                dropOrigin,
                dropSpreadRadius
            );
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=lime>[StoneController] {gameObject.name} 掉落 {amount} 个石料</color>");
    }
    
    /// <summary>
    /// 生成最终阶段掉落（全部掉落）
    /// </summary>
    private void SpawnFinalDrops()
    {
        // 掉落矿物（如果镐子等级足够）
        if (lastHitCanGetOre)
        {
            int oreDrop = StoneDropConfig.CalculateFinalOreDropAmount(currentStage, oreIndex);
            if (oreDrop > 0)
            {
                SpawnOreDrops(oreDrop);
            }
        }
        
        // 掉落石料（所有镐子都能获得）
        int stoneDrop = StoneDropConfig.CalculateFinalStoneDropAmount(currentStage);
        if (stoneDrop > 0)
        {
            SpawnStoneDrops(stoneDrop);
        }
        
        // 播放破碎音效
        PlayBreakSound();
    }
    
    /// <summary>
    /// 获取对应矿物类型的掉落物品
    /// </summary>
    private ItemData GetOreItem()
    {
        return oreType switch
        {
            OreType.C1 => copperOreItem,
            OreType.C2 => ironOreItem,
            OreType.C3 => goldOreItem,
            _ => null
        };
    }
    #endregion
    
    #region 经验系统
    /// <summary>
    /// 给予采集经验
    /// </summary>
    /// <param name="isFinal">是否为最终阶段</param>
    private void GrantExperience(bool isFinal)
    {
        int oreCount = 0;
        int stoneCount = 0;
        
        if (isFinal)
        {
            // 最终阶段：计算全部掉落
            if (lastHitCanGetOre)
            {
                oreCount = StoneDropConfig.CalculateFinalOreDropAmount(currentStage, oreIndex);
            }
            stoneCount = StoneDropConfig.CalculateFinalStoneDropAmount(currentStage);
        }
        else
        {
            // 阶段转换：计算差值掉落
            var config = CurrentStageConfig;
            if (config != null)
            {
                int newOreIndex = config.decreaseOreIndexOnTransition 
                    ? Mathf.Max(0, oreIndex - 1) 
                    : oreIndex;
                
                if (lastHitCanGetOre)
                {
                    oreCount = StoneDropConfig.CalculateOreDropAmount(
                        currentStage, oreIndex, 
                        config.nextStage, newOreIndex
                    );
                }
                stoneCount = StoneDropConfig.CalculateStoneDropAmount(currentStage, config.nextStage);
            }
        }
        
        int totalXP = StoneDropConfig.CalculateExperience(oreCount, stoneCount);
        
        if (totalXP > 0 && SkillLevelService.Instance != null)
        {
            SkillLevelService.Instance.AddExperience(SkillType.Gathering, totalXP);
            
            if (showDebugInfo)
            {
                Debug.Log($"<color=lime>[StoneController] {gameObject.name} 获得 {totalXP} 点采集经验（矿物{oreCount}×2 + 石料{stoneCount}×1）</color>");
            }
        }
    }
    #endregion
    
    #region 工具辅助方法
    /// <summary>
    /// 获取镐子材料等级
    /// </summary>
    private int GetPickaxeTier(ToolHitContext ctx)
    {
        if (ctx.attacker != null)
        {
            var toolController = ctx.attacker.GetComponent<PlayerToolController>();
            if (toolController != null && toolController.CurrentToolData != null)
            {
                var toolData = toolController.CurrentToolData as ToolData;
                if (toolData != null)
                {
                    return toolData.GetMaterialTierValue();
                }
            }
        }
        return 0; // 默认木质
    }
    
    /// <summary>
    /// 获取精力消耗
    /// </summary>
    private float GetEnergyCost(ToolHitContext ctx)
    {
        float energyCost = 2f; // 默认
        
        if (ctx.attacker != null)
        {
            var toolController = ctx.attacker.GetComponent<PlayerToolController>();
            if (toolController != null && toolController.CurrentToolData != null)
            {
                var toolData = toolController.CurrentToolData as ToolData;
                if (toolData != null)
                {
                    energyCost = toolData.energyCost;
                }
            }
        }
        
        return energyCost;
    }
    
    /// <summary>
    /// 尝试消耗精力
    /// </summary>
    private bool TryConsumeEnergy(float energyCost)
    {
        if (EnergySystem.Instance != null)
        {
            return EnergySystem.Instance.TryConsumeEnergy(Mathf.RoundToInt(energyCost));
        }
        return true; // 如果没有精力系统，默认允许
    }
    #endregion
    
    #region Sprite系统
    /// <summary>
    /// 更新Sprite显示
    /// </summary>
    public void UpdateSprite()
    {
        if (spriteRenderer == null) return;
        
        string spriteName = GetSpriteName();
        
        // 尝试从Resources加载
        string fullPath = spritePathPrefix + spriteName;
        Sprite sprite = Resources.Load<Sprite>(fullPath);
        
        if (sprite != null)
        {
            spriteRenderer.sprite = sprite;
        }
        else if (showDebugInfo)
        {
            Debug.LogWarning($"[StoneController] 找不到Sprite: {fullPath}");
        }
    }
    
    /// <summary>
    /// 获取当前状态的Sprite名称
    /// 格式：Stone_{OreType}_{Stage}_{OreIndex}
    /// </summary>
    private string GetSpriteName()
    {
        string oreTypeStr = oreType == OreType.None ? "C0" : oreType.ToString();
        string stageStr = currentStage.ToString();
        return $"Stone_{oreTypeStr}_{stageStr}_{oreIndex}";
    }
    
    /// <summary>
    /// 从Sprite名称解析状态
    /// </summary>
    public static bool TryParseSpriteName(string spriteName, out OreType oreType, out StoneStage stage, out int oreIndex)
    {
        oreType = OreType.None;
        stage = StoneStage.M1;
        oreIndex = 0;
        
        if (string.IsNullOrEmpty(spriteName)) return false;
        
        // 格式：Stone_{OreType}_{Stage}_{OreIndex}
        string[] parts = spriteName.Split('_');
        if (parts.Length < 4 || parts[0] != "Stone") return false;
        
        // 解析矿物类型
        string oreStr = parts[1];
        if (oreStr == "C0" || oreStr == "None")
            oreType = OreType.None;
        else if (System.Enum.TryParse(oreStr, out OreType parsedOre))
            oreType = parsedOre;
        else
            return false;
        
        // 解析阶段
        if (!System.Enum.TryParse(parts[2], out stage))
            return false;
        
        // 解析含量指数
        if (!int.TryParse(parts[3], out oreIndex))
            return false;
        
        return true;
    }
    #endregion
    
    #region 音效系统
    private void PlayMineHitSound()
    {
        if (mineHitSound != null)
        {
            AudioSource.PlayClipAtPoint(mineHitSound, GetPosition(), soundVolume);
        }
    }
    
    private void PlayBreakSound()
    {
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, GetPosition(), soundVolume);
        }
    }
    
    private void PlayTierInsufficientSound()
    {
        if (tierInsufficientSound != null)
        {
            AudioSource.PlayClipAtPoint(tierInsufficientSound, GetPosition(), soundVolume);
        }
    }
    #endregion
    
    #region 视觉效果
    /// <summary>
    /// 播放抖动效果
    /// </summary>
    private void PlayShakeEffect()
    {
        StartCoroutine(ShakeCoroutine());
    }
    
    private System.Collections.IEnumerator ShakeCoroutine()
    {
        if (spriteRenderer == null) yield break;
        
        Vector3 originalPos = spriteRenderer.transform.localPosition;
        float shakeDuration = 0.15f;
        float shakeAmount = 0.05f;
        float elapsed = 0f;
        
        while (elapsed < shakeDuration)
        {
            float progress = elapsed / shakeDuration;
            float damping = 1f - progress;
            float x = Mathf.Sin(progress * Mathf.PI * 4) * shakeAmount * damping;
            spriteRenderer.transform.localPosition = originalPos + new Vector3(x, 0, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        spriteRenderer.transform.localPosition = originalPos;
    }
    #endregion
    
    #region 销毁
    /// <summary>
    /// 销毁石头
    /// </summary>
    private void DestroyStone()
    {
        isDepleted = true;
        
        // 从注册表注销
        if (ResourceNodeRegistry.Instance != null)
        {
            ResourceNodeRegistry.Instance.Unregister(gameObject.GetInstanceID());
        }
        
        // 销毁父物体（整个石头）
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (showDebugInfo)
            Debug.Log($"<color=orange>[StoneController] {gameObject.name} 被完全挖掘！</color>");
    }
    #endregion
    
    #region 公共接口
    public StoneStage GetCurrentStage() => currentStage;
    public OreType GetOreType() => oreType;
    public int GetOreIndex() => oreIndex;
    public int GetCurrentHealth() => currentHealth;
    
    /// <summary>
    /// 设置阶段（用于调试或初始化）
    /// </summary>
    public void SetStage(StoneStage stage, OreType type, int index)
    {
        currentStage = stage;
        oreType = type;
        oreIndex = Mathf.Clamp(index, 0, 4);
        InitializeHealth();
        UpdateSprite();
    }
    #endregion
    
    #region 编辑器
    #if UNITY_EDITOR
    [ContextMenu("调试 - 设置为M1_C1_4（大铜矿）")]
    private void DEBUG_SetM1C1()
    {
        SetStage(StoneStage.M1, OreType.C1, 4);
    }
    
    [ContextMenu("调试 - 设置为M1_C2_3（大铁矿）")]
    private void DEBUG_SetM1C2()
    {
        SetStage(StoneStage.M1, OreType.C2, 3);
    }
    
    [ContextMenu("调试 - 设置为M1_C3_2（大金矿）")]
    private void DEBUG_SetM1C3()
    {
        SetStage(StoneStage.M1, OreType.C3, 2);
    }
    
    [ContextMenu("调试 - 设置为M4（装饰石头）")]
    private void DEBUG_SetM4()
    {
        SetStage(StoneStage.M4, OreType.None, 0);
    }
    
    [ContextMenu("调试 - 造成10点伤害")]
    private void DEBUG_TakeDamage10()
    {
        lastHitCanGetOre = true;
        lastHitPickaxeTier = 5;
        TakeDamage(10);
    }
    
    [ContextMenu("调试 - 造成50点伤害（测试溢出）")]
    private void DEBUG_TakeDamage50()
    {
        lastHitCanGetOre = true;
        lastHitPickaxeTier = 5;
        TakeDamage(50);
    }
    #endif
    #endregion
}
