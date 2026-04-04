using UnityEngine;
using UnityEngine.SceneManagement;
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
[DefaultExecutionOrder(-1000)]
public class PersistentManagers : MonoBehaviour
{
    private const string RootObjectName = "PersistentManagers";
    private const string TimeManagerObjectName = "TimeManager";
    private const string SeasonManagerObjectName = "SeasonManager";
    private const string WeatherSystemObjectName = "WeatherSystem";
    private const string PersistentRegistryObjectName = "[PersistentObjectRegistry]";
    private const string PrefabDatabaseResourcesPath = "Data/Database/PrefabDatabase";
#if UNITY_EDITOR
    private const string PrefabDatabaseEditorAssetPath = "Assets/111_Data/Database/PrefabDatabase.asset";
#endif

    private static PersistentManagers instance;
    private static bool isBootstrapping;
    private bool isInitialized;

    public static PersistentManagers Instance => EnsureRuntime();
    
    [Header("预制体数据库")]
    [Tooltip("预制体数据库资产（用于动态对象重建）")]
    [SerializeField] private PrefabDatabase prefabDatabase;
    
    [Header("调试")]
    [SerializeField] private bool showDebugInfo = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void BootstrapRuntime()
    {
        EnsureRuntime();
    }

    public static PersistentManagers EnsureRuntime()
    {
        if (instance != null)
        {
            return instance;
        }

        if (isBootstrapping)
        {
            return instance;
        }

        isBootstrapping = true;
        try
        {
            instance = FindFirstObjectByType<PersistentManagers>(FindObjectsInactive.Include);
            if (instance == null)
            {
                GameObject rootObject = new GameObject(RootObjectName);
                instance = rootObject.AddComponent<PersistentManagers>();
            }

            instance.InitializeIfNeeded();
            return instance;
        }
        finally
        {
            isBootstrapping = false;
        }
    }

    public static T EnsureManagedComponent<T>(string objectName) where T : Component
    {
        PersistentManagers root = EnsureRuntime();
        return root.EnsureManagedChild<T>(objectName);
    }

    internal static Transform GetRuntimeRootTransform(bool createIfMissing = true)
    {
        if (!createIfMissing)
        {
            return instance != null ? instance.transform : null;
        }

        return EnsureRuntime().transform;
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeIfNeeded();
        }
        else
        {
            MergeChildrenInto(instance.transform);
            if (HasMeaningfulStateToMerge())
            {
                Debug.LogWarning("<color=yellow>[PersistentManagers] 检测到重复实例，已合并有效状态后销毁</color>");
            }
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void InitializeIfNeeded()
    {
        if (isInitialized)
        {
            return;
        }

        isInitialized = true;
        gameObject.name = RootObjectName;
        DontDestroyOnLoad(gameObject);

        // 🔥 3.7.5：初始化 DynamicObjectFactory
        InitializeDynamicObjectFactory();
        EnsureRuntimeGraph();

        if (showDebugInfo)
            Debug.Log("<color=cyan>[PersistentManagers] 初始化完成，管理器将在场景切换时保持</color>");
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        EnsureRuntimeGraph();
    }

    private void EnsureRuntimeGraph()
    {
        TimeManager timeManager = EnsureManagedChild<TimeManager>(TimeManagerObjectName);
        EnsureTimeManagerDebugger(timeManager);
        EnsureManagedChild<SeasonManager>(SeasonManagerObjectName);
        EnsureManagedChild<WeatherSystem>(WeatherSystemObjectName);

        if (PersistentObjectRegistry.Instance != null)
        {
            AdoptIntoRoot(PersistentObjectRegistry.Instance.transform, PersistentRegistryObjectName);
        }
    }

    private T EnsureManagedChild<T>(string objectName) where T : Component
    {
        T existing = FindFirstObjectByType<T>(FindObjectsInactive.Include);
        if (existing != null)
        {
            AdoptIntoRoot(existing.transform, objectName);
            return existing;
        }

        GameObject childObject = new GameObject(objectName);
        childObject.transform.SetParent(transform, false);
        return childObject.AddComponent<T>();
    }

    private void EnsureTimeManagerDebugger(TimeManager timeManager)
    {
        if (timeManager == null)
        {
            return;
        }

        TimeManagerDebugger debugger = timeManager.GetComponent<TimeManagerDebugger>();
        if (debugger == null)
        {
            debugger = timeManager.gameObject.AddComponent<TimeManagerDebugger>();
        }

        debugger.enableDebugKeys = true;
        debugger.enableScreenClock = true;
        debugger.showDebugInfo = false;
    }

    private void AdoptIntoRoot(Transform target, string objectName)
    {
        if (target == null || target == transform)
        {
            return;
        }

        if (target.gameObject.name != objectName)
        {
            target.gameObject.name = objectName;
        }

        if (target.parent != transform)
        {
            target.SetParent(transform, false);
        }
    }

    private void MergeChildrenInto(Transform targetRoot)
    {
        if (targetRoot == null || targetRoot == transform)
        {
            return;
        }

        while (transform.childCount > 0)
        {
            transform.GetChild(0).SetParent(targetRoot, false);
        }
    }

    private bool HasMeaningfulStateToMerge()
    {
        return transform.childCount > 0 || prefabDatabase != null;
    }
    
    /// <summary>
    /// 🔥 3.7.5：初始化 DynamicObjectFactory
    /// </summary>
    private void InitializeDynamicObjectFactory()
    {
        if (TryResolvePrefabDatabase())
        {
            DynamicObjectFactory.Initialize(prefabDatabase);
            if (showDebugInfo)
                Debug.Log($"<color=cyan>[PersistentManagers] DynamicObjectFactory 已初始化，使用 PrefabDatabase ({prefabDatabase.EntryCount} 个预制体)</color>");
        }
        else
        {
            Debug.LogWarning("<color=yellow>[PersistentManagers] PrefabDatabase 未配置，DynamicObjectFactory 未初始化</color>");
        }
    }

    private bool TryResolvePrefabDatabase()
    {
        if (prefabDatabase != null)
        {
            return true;
        }

        prefabDatabase = Resources.Load<PrefabDatabase>(PrefabDatabaseResourcesPath);
        if (prefabDatabase != null)
        {
            return true;
        }

        prefabDatabase = Resources.Load<PrefabDatabase>("PrefabDatabase");
        if (prefabDatabase != null)
        {
            return true;
        }

#if UNITY_EDITOR
        prefabDatabase = UnityEditor.AssetDatabase.LoadAssetAtPath<PrefabDatabase>(PrefabDatabaseEditorAssetPath);
#endif

        return prefabDatabase != null;
    }
}
