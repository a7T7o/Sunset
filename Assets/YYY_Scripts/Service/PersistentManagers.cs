using UnityEngine;
using FarmGame.Data.Core;

/// <summary>
/// 持久化管理器容器
/// 确保所有子管理器在场景切换时不被销毁
/// 
/// 使用方法：
/// 1. 在场景中创建一个根物体，命名为 "PersistentManagers"
/// 2. 添加此组件
/// 3. 将 TimeManager、SeasonManager、WeatherSystem 等管理器作为子物体
/// 4. 这些管理器不需要再调用 DontDestroyOnLoad
/// 
/// 🔥 3.7.5：添加 PrefabDatabase 初始化
/// </summary>
public class PersistentManagers : MonoBehaviour
{
    private static PersistentManagers instance;
    
    [Header("预制体数据库")]
    [Tooltip("预制体数据库资产（用于动态对象重建）")]
    [SerializeField] private PrefabDatabase prefabDatabase;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 🔥 3.7.5：初始化 DynamicObjectFactory
            InitializeDynamicObjectFactory();
            
            if (showDebugInfo)
                Debug.Log("<color=cyan>[PersistentManagers] 初始化完成，管理器将在场景切换时保持</color>");
        }
        else
        {
            Debug.LogWarning("<color=yellow>[PersistentManagers] 检测到重复实例，销毁</color>");
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 🔥 3.7.5：初始化 DynamicObjectFactory
    /// </summary>
    private void InitializeDynamicObjectFactory()
    {
        if (prefabDatabase != null)
        {
            DynamicObjectFactory.Initialize(prefabDatabase);
            if (showDebugInfo)
                Debug.Log($"<color=cyan>[PersistentManagers] DynamicObjectFactory 已初始化，使用 PrefabDatabase ({prefabDatabase.EntryCount} 个预制体)</color>");
        }
        else
        {
            // 尝试从 Resources 加载
            prefabDatabase = Resources.Load<PrefabDatabase>("PrefabDatabase");
            if (prefabDatabase != null)
            {
                DynamicObjectFactory.Initialize(prefabDatabase);
                if (showDebugInfo)
                    Debug.Log($"<color=cyan>[PersistentManagers] DynamicObjectFactory 已初始化（从 Resources 加载）</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>[PersistentManagers] PrefabDatabase 未配置，DynamicObjectFactory 未初始化</color>");
            }
        }
    }
}
