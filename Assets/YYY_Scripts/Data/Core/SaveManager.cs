using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sunset.Story;

namespace FarmGame.Data.Core
{
    /// <summary>
    /// 存档管理器 (MVP 版本)
    /// 
    /// 职责：
    /// - 协调存档/读档流程
    /// - 收集全局数据（时间、玩家）
    /// - 序列化/反序列化 JSON
    /// - 文件读写
    /// 
    /// 本阶段简化：
    /// - 只做当前场景内的状态恢复（不换场景）
    /// - 使用 Unity JsonUtility（简单但有限制）
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        private const string NativeFreshSceneName = "Town";
        private const string FreshStartBaselineSlotName = "__fresh_start_baseline__";
        private const string DefaultProgressSlotName = "__default_progress__";
        private const string OrdinarySlotPrefix = "slot";

        #region 单例
        
        private static SaveManager _instance;
        
        public static SaveManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SaveManager>();
                    
                    if (_instance == null)
                    {
                        var go = new GameObject("[SaveManager]");
                        _instance = go.AddComponent<SaveManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
        
        #endregion
        
        #region 配置
        
        [Header("存档配置")]
        [SerializeField] private string saveFileExtension = ".json";
        [SerializeField] private string saveFolder = "Save";
        
        [Header("调试")]
        [SerializeField] private bool showDebugInfo = false;
        [SerializeField] private bool prettyPrintJson = true;

        #endregion
        
        #region 属性
        
        /// <summary>
        /// 存档目录路径（项目根目录下的 Save）
        /// </summary>
        public string SaveFolderPath
        {
            get
            {
                // 打包后必须落到真正可写的持久化目录；编辑器也统一走同一路径，
                // 再通过迁移逻辑兼容旧的项目根目录 Save / Assets/Save。
                return Path.Combine(Application.persistentDataPath, saveFolder);
            }
        }

        private string LegacyProjectRootSaveFolderPath => Path.GetFullPath(Path.Combine(Application.dataPath, "..", saveFolder));
        private string LegacyEditorSaveFolderPath => Path.Combine(Application.dataPath, saveFolder);
        
        /// <summary>
        /// 当前加载的存档数据（用于调试）
        /// </summary>
        public GameSaveData CurrentSaveData { get; private set; }

        public bool HasFreshStartBaseline => File.Exists(GetSaveFilePath(FreshStartBaselineSlotName));
        public bool HasDefaultProgressSlot => true;
        public string DefaultSaveSlotName => DefaultProgressSlotName;

        public event Action SaveSlotsChanged;

        private bool _freshStartBaselinePrepared;
        private bool _freshStartBaselineCaptureScheduled;
        private Coroutine _freshStartBaselineCaptureCoroutine;
        private bool _startupDefaultProgressHandled;
        private bool _nativeFreshRestartInProgress;
        private bool _sceneSwitchLoadInProgress;
        private bool _legacyMigrationAttempted;
        private bool _dynamicObjectFactoryInitialized;

        #endregion

        #region Unity 生命周期

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            if (Instance != null)
            {
                Instance.ScheduleFreshStartBaselineCapture();
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;

            // 🔥 修复：DontDestroyOnLoad 只对根级 GameObject 有效
            // 如果当前对象有父对象，先解除父子关系
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            DontDestroyOnLoad(gameObject);

            // 确保存档目录存在
            EnsureSaveFolderExists();
            if (ShouldRunHeavyStartupBootstrapImmediately())
            {
                EnsureLegacySaveMigrationAttempted();
                EnsureDynamicObjectFactoryInitialized();
            }
            else
            {
                // 打包版启动阶段不主动做旧存档迁移与工厂初始化；
                // 迁移/初始化改为普通槽位或读档路径按需触发。
            }

            ScheduleFreshStartBaselineCapture();

            if (showDebugInfo)
                Debug.Log($"[SaveManager] 初始化完成，存档路径: {SaveFolderPath}");
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                ExecuteQuickSaveHotkey();
            }
            else if (Input.GetKeyDown(KeyCode.F9))
            {
                ExecuteQuickLoadHotkey();
            }
        }

        private void OnApplicationQuit()
        {
            TryPersistDefaultProgressOnQuit();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        #endregion

        #region 核心 API

        /// <summary>
        /// 保存游戏
        /// </summary>
        /// <param name="slotName">存档槽名称（如 "slot1", "autosave"）</param>
        /// <returns>是否成功</returns>
        public bool SaveGame(string slotName)
        {
            if (IsDefaultSlot(slotName))
            {
                return QuickSaveDefaultSlot();
            }

            return SaveGameInternal(slotName, enforceSaveBlockers: true, raiseSlotChangedEvent: true);
        }

        /// <summary>
        /// 加载游戏
        /// </summary>
        /// <param name="slotName">存档槽名称</param>
        /// <returns>是否成功</returns>
        public bool LoadGame(string slotName)
        {
            if (IsDefaultSlot(slotName))
            {
                return QuickLoadDefaultSlot();
            }

            return LoadGameInternal(slotName, refreshUi: true, raiseSlotChangedEvent: true);
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool SaveExists(string slotName)
        {
            if (IsDefaultSlot(slotName))
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(slotName))
            {
                return false;
            }

            EnsureLegacySaveMigrationAttempted();
            return File.Exists(GetSaveFilePath(slotName));
        }

        /// <summary>
        /// 删除存档
        /// </summary>
        public bool DeleteSave(string slotName)
        {
            if (string.IsNullOrWhiteSpace(slotName) || IsReservedSlot(slotName))
            {
                return false;
            }

            string filePath = GetSaveFilePath(slotName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 删除存档: {filePath}");
                RaiseSaveSlotsChanged();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取所有存档槽名称
        /// </summary>
        public string[] GetAllSaveSlots()
        {
            List<string> slots = new List<string>();
            if (HasDefaultProgressSlot)
            {
                slots.Add(DefaultProgressSlotName);
            }

            slots.AddRange(GetOrdinarySlotNames());
            return slots.ToArray();
        }

        public string[] GetOrdinarySlotNames()
        {
            EnsureLegacySaveMigrationAttempted();

            if (!Directory.Exists(SaveFolderPath))
            {
                return Array.Empty<string>();
            }

            var files = Directory.GetFiles(SaveFolderPath, "*" + saveFileExtension);
            List<string> slots = new List<string>(files.Length);

            for (int i = 0; i < files.Length; i++)
            {
                string slotName = Path.GetFileNameWithoutExtension(files[i]);
                if (IsReservedSlot(slotName) || string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal))
                {
                    continue;
                }

                slots.Add(slotName);
            }

            slots.Sort(CompareOrdinarySlotNames);
            return slots.ToArray();
        }

        /// <summary>
        /// 兼容旧版设置页运行时 UI 的固定入口。
        /// 新版逻辑应改用 GetOrdinarySlotNames()；这里暂时保留，避免重构中途让旧面板直接爆红。
        /// </summary>
        public string[] GetVisibleSlotNames()
        {
            string[] ordinarySlots = GetOrdinarySlotNames();
            if (ordinarySlots.Length > 0)
            {
                return ordinarySlots;
            }

            return new[] { "slot1", "slot2", "slot3" };
        }

        public bool IsDefaultSlot(string slotName)
        {
            return string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal);
        }

        public string GetSlotDisplayName(string slotName)
        {
            if (IsDefaultSlot(slotName))
            {
                return "默认存档";
            }

            if (!string.IsNullOrWhiteSpace(slotName)
                && slotName.Length > OrdinarySlotPrefix.Length
                && slotName.StartsWith(OrdinarySlotPrefix, StringComparison.Ordinal)
                && int.TryParse(slotName.Substring(OrdinarySlotPrefix.Length), out int index)
                && index > 0)
            {
                return $"存档 {index}";
            }

            return string.IsNullOrWhiteSpace(slotName) ? "未知存档" : slotName;
        }

        public bool CanExecutePlayerSaveAction(out string blockerReason)
        {
            StoryProgressPersistenceService.EnsureRuntime();
            return StoryProgressPersistenceService.CanSaveNow(out blockerReason);
        }

        public bool CreateNewOrdinarySlotFromCurrentProgress(out string slotName)
        {
            slotName = GetNextOrdinarySlotName();
            return SaveGame(slotName);
        }

        public bool TryCopySlotData(string sourceSlotName, out GameSaveData copiedData, out string error)
        {
            copiedData = null;
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(sourceSlotName))
            {
                error = "槽位名为空";
                return false;
            }

            if (!TryReadSaveData(sourceSlotName, out GameSaveData saveData))
            {
                error = "当前槽位没有可复制的存档内容";
                return false;
            }

            copiedData = CloneSaveData(saveData);
            return copiedData != null;
        }

        public bool PasteSaveDataToSlot(GameSaveData copiedData, string targetSlotName, out string error)
        {
            error = string.Empty;

            if (copiedData == null)
            {
                error = "请先复制存档内容";
                return false;
            }

            if (string.IsNullOrWhiteSpace(targetSlotName)
                || string.Equals(targetSlotName, FreshStartBaselineSlotName, StringComparison.Ordinal)
                || string.Equals(targetSlotName, DefaultProgressSlotName, StringComparison.Ordinal))
            {
                error = "目标槽位无效";
                return false;
            }

            try
            {
                GameSaveData targetData = CloneSaveData(copiedData);
                if (targetData == null)
                {
                    error = "复制缓存无效";
                    return false;
                }

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (SaveExists(targetSlotName) && TryReadSaveData(targetSlotName, out GameSaveData existingData) && existingData != null)
                {
                    targetData.createdTime = string.IsNullOrWhiteSpace(existingData.createdTime)
                        ? timestamp
                        : existingData.createdTime;
                }
                else if (string.IsNullOrWhiteSpace(targetData.createdTime))
                {
                    targetData.createdTime = timestamp;
                }

                targetData.lastSaveTime = timestamp;
                WriteSaveData(targetSlotName, targetData);
                RaiseSaveSlotsChanged();
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                Debug.LogError($"[SaveManager] 粘贴存档失败: {exception.Message}\n{exception.StackTrace}");
                return false;
            }
        }

        public bool QuickSaveDefaultSlot()
        {
            Debug.LogWarning("[SaveManager] 默认存档已锁定为原生开局入口，本轮不再允许写入。");
            return false;
        }

        public bool QuickLoadDefaultSlot()
        {
            return BeginNativeFreshRestart();
        }

        public bool RestartToFreshGame()
        {
            return BeginNativeFreshRestart();
        }

        public bool TryGetDefaultSlotSummary(out SaveSlotSummary summary)
        {
            summary = CreateEmptySlotSummary(DefaultProgressSlotName);
            summary.exists = true;
            summary.isDefaultSlot = true;
            summary.displayName = GetSlotDisplayName(DefaultProgressSlotName);
            summary.sceneName = NativeFreshSceneName;
            summary.storyPhaseLabel = FormatStoryPhaseLabel((int)StoryPhase.EnterVillage);

            if (HasFreshStartBaseline && TryReadSaveData(FreshStartBaselineSlotName, out GameSaveData baselineData))
            {
                SaveSlotSummary baselineSummary = BuildSlotSummary(DefaultProgressSlotName, baselineData);
                baselineSummary.slotName = DefaultProgressSlotName;
                baselineSummary.displayName = GetSlotDisplayName(DefaultProgressSlotName);
                baselineSummary.isDefaultSlot = true;
                summary = baselineSummary;
            }

            return true;
        }

        public bool TryGetSlotSummary(string slotName, out SaveSlotSummary summary)
        {
            if (IsDefaultSlot(slotName))
            {
                return TryGetDefaultSlotSummary(out summary);
            }

            summary = CreateEmptySlotSummary(slotName);

            if (string.IsNullOrWhiteSpace(slotName))
            {
                summary.loadError = "槽位名为空";
                return false;
            }

            if (!SaveExists(slotName))
            {
                return true;
            }

            if (!TryReadSaveData(slotName, out GameSaveData saveData))
            {
                summary.exists = true;
                summary.loadError = "存档文件损坏或无法解析";
                return false;
            }

            summary = BuildSlotSummary(slotName, saveData);
            summary.isDefaultSlot = IsDefaultSlot(slotName);
            summary.displayName = GetSlotDisplayName(slotName);
            return true;
        }

        #endregion

        #region 槽位管理

        private bool SaveGameInternal(string slotName, bool enforceSaveBlockers, bool raiseSlotChangedEvent)
        {
            if (string.IsNullOrWhiteSpace(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                return false;
            }

            if (!string.Equals(slotName, FreshStartBaselineSlotName, StringComparison.Ordinal))
            {
                EnsureLegacySaveMigrationAttempted();
            }

            if (string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal))
            {
                Debug.LogWarning("[SaveManager] 默认存档当前固定为原生开局入口，本轮不再允许覆盖。");
                return false;
            }

            StoryProgressPersistenceService.EnsureRuntime();
            if (enforceSaveBlockers && !StoryProgressPersistenceService.CanSaveNow(out string saveBlockerReason))
            {
                Debug.LogWarning($"[SaveManager] 当前不可存档：{saveBlockerReason}");
                return false;
            }

            try
            {
                GameSaveData saveData = CollectFullSaveData();
                WriteSaveData(slotName, saveData);
                CurrentSaveData = saveData;

                if (raiseSlotChangedEvent)
                {
                    RaiseSaveSlotsChanged();
                }

                if (string.Equals(slotName, FreshStartBaselineSlotName, StringComparison.Ordinal))
                {
                    _freshStartBaselinePrepared = true;
                    RaiseSaveSlotsChanged();
                }

                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 保存成功: {GetSaveFilePath(slotName)}, 世界对象: {saveData.worldObjects?.Count ?? 0}");
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 保存失败: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        private bool LoadGameInternal(string slotName, bool refreshUi, bool raiseSlotChangedEvent)
        {
            if (string.IsNullOrWhiteSpace(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                return false;
            }

            if (!TryReadSaveData(slotName, out GameSaveData saveData))
            {
                Debug.LogWarning($"[SaveManager] 存档文件不存在或解析失败: {GetSaveFilePath(slotName)}");
                return false;
            }

            string targetSceneName = ResolveTargetLoadSceneName(saveData);
            if (ShouldSceneSwitchBeforeLoad(targetSceneName))
            {
                return BeginSceneSwitchLoad(saveData, targetSceneName, refreshUi, raiseSlotChangedEvent);
            }

            return ApplyLoadedSaveData(saveData, refreshUi, raiseSlotChangedEvent);
        }

        private bool ApplyLoadedSaveData(GameSaveData saveData, bool refreshUi, bool raiseSlotChangedEvent)
        {
            try
            {
                EnsureDynamicObjectFactoryInitialized();
                StoryProgressPersistenceService.EnsureRuntime();

                if (PersistentObjectRegistry.Instance != null)
                {
                    PersistentObjectRegistry.Instance.PruneStaleRecords();
                    if (showDebugInfo)
                    {
                        Debug.Log("[SaveManager] 已清理 PersistentObjectRegistry 中的空引用（保留活着的对象）");
                    }
                }

                RestoreGameTimeData(saveData.gameTime);
                RestorePlayerData(saveData.player);
                RestoreInventoryData(saveData.inventory);
                CloudShadowManager.ImportPersistentSaveData(saveData.cloudShadowScenes);

                if (PersistentObjectRegistry.Instance != null && saveData.worldObjects != null)
                {
                    PersistentObjectRegistry.Instance.RestoreAllFromSaveData(saveData.worldObjects);
                }

                StoryProgressPersistenceService.FinalizeLoadedSave(saveData.worldObjects);

                CurrentSaveData = saveData;

                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 加载成功: scene={ResolveTargetLoadSceneName(saveData)}, 世界对象: {saveData.worldObjects?.Count ?? 0}");
                }

                if (refreshUi)
                {
                    RefreshAllUI();
                }

                if (raiseSlotChangedEvent)
                {
                    RaiseSaveSlotsChanged();
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 加载失败: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        private bool BeginSceneSwitchLoad(GameSaveData saveData, string targetSceneName, bool refreshUi, bool raiseSlotChangedEvent)
        {
            if (_sceneSwitchLoadInProgress)
            {
                Debug.LogWarning("[SaveManager] 当前已有读档切场流程在进行，已忽略新的读档请求。");
                return false;
            }

            if (!Application.isPlaying)
            {
                return false;
            }

            StartCoroutine(LoadAfterSceneSwitchRoutine(saveData, targetSceneName, refreshUi, raiseSlotChangedEvent));
            return true;
        }

        private IEnumerator LoadAfterSceneSwitchRoutine(GameSaveData saveData, string targetSceneName, bool refreshUi, bool raiseSlotChangedEvent)
        {
            _sceneSwitchLoadInProgress = true;

            AsyncOperation loadOperation = null;
            try
            {
                loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SaveManager] 读档切场失败，无法载入场景 {targetSceneName}：{exception.Message}");
            }

            if (loadOperation == null)
            {
                _sceneSwitchLoadInProgress = false;
                yield break;
            }

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            ApplyLoadedSaveData(saveData, refreshUi, raiseSlotChangedEvent);
            _sceneSwitchLoadInProgress = false;
        }

        private static string ResolveTargetLoadSceneName(GameSaveData saveData)
        {
            return NormalizeSceneName(saveData?.player?.sceneName);
        }

        private static bool ShouldSceneSwitchBeforeLoad(string targetSceneName)
        {
            return !string.IsNullOrWhiteSpace(targetSceneName)
                && !string.Equals(SceneManager.GetActiveScene().name, targetSceneName, StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizeSceneName(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                return string.Empty;
            }

            string trimmed = sceneName.Trim();
            return string.Equals(trimmed, "DontDestroyOnLoad", StringComparison.OrdinalIgnoreCase)
                ? string.Empty
                : trimmed;
        }

        private GameSaveData CollectFullSaveData()
        {
            var saveData = new GameSaveData
            {
                lastSaveTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                gameTime = CollectGameTimeData(),
                player = CollectPlayerData(),
                inventory = CollectInventoryData()
            };

            if (PersistentObjectRegistry.Instance != null)
            {
                saveData.worldObjects = PersistentObjectRegistry.Instance.CollectAllSaveData();
            }

            saveData.cloudShadowScenes = CloudShadowManager.ExportPersistentSaveData();

            return saveData;
        }

        private void WriteSaveData(string slotName, GameSaveData saveData)
        {
            string json = prettyPrintJson
                ? JsonUtility.ToJson(saveData, true)
                : JsonUtility.ToJson(saveData);

            File.WriteAllText(GetSaveFilePath(slotName), json);
        }

        private bool TryReadSaveData(string slotName, out GameSaveData saveData)
        {
            saveData = null;
            string readableSlotName = string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal)
                ? FreshStartBaselineSlotName
                : slotName;

            if (!string.Equals(readableSlotName, FreshStartBaselineSlotName, StringComparison.Ordinal))
            {
                EnsureLegacySaveMigrationAttempted();
            }

            string filePath = GetSaveFilePath(readableSlotName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                saveData = JsonUtility.FromJson<GameSaveData>(json);
                if (saveData != null && saveData.cloudShadowScenes == null)
                {
                    saveData.cloudShadowScenes = new List<CloudShadowSceneSaveData>();
                }
                return saveData != null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[SaveManager] 读取存档 {slotName} 失败：{exception.Message}");
                return false;
            }
        }

        private void ScheduleFreshStartBaselineCapture()
        {
            if (_freshStartBaselinePrepared || _freshStartBaselineCaptureScheduled || !isActiveAndEnabled)
            {
                return;
            }

            _freshStartBaselineCaptureScheduled = true;
            if (_freshStartBaselineCaptureCoroutine != null)
            {
                StopCoroutine(_freshStartBaselineCaptureCoroutine);
            }

            _freshStartBaselineCaptureCoroutine = StartCoroutine(CaptureFreshStartBaselineRoutine());
        }

        private IEnumerator CaptureFreshStartBaselineRoutine()
        {
            yield return null;
            yield return new WaitForEndOfFrame();

            _freshStartBaselineCaptureScheduled = false;
            _freshStartBaselineCaptureCoroutine = null;

            if (_freshStartBaselinePrepared)
            {
                yield break;
            }

            SaveGameInternal(FreshStartBaselineSlotName, enforceSaveBlockers: false, raiseSlotChangedEvent: false);
            RaiseSaveSlotsChanged();
        }

        private void RaiseSaveSlotsChanged()
        {
            SaveSlotsChanged?.Invoke();
        }

        private static SaveSlotSummary CreateEmptySlotSummary(string slotName)
        {
            return new SaveSlotSummary
            {
                slotName = slotName,
                displayName = string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal) ? "默认存档" : slotName,
                isDefaultSlot = string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal),
                day = 1,
                season = 0,
                year = 1,
                hour = 6,
                minute = 0,
                storyPhaseLabel = "开局"
            };
        }

        private static SaveSlotSummary BuildSlotSummary(string slotName, GameSaveData saveData)
        {
            SaveSlotSummary summary = CreateEmptySlotSummary(slotName);
            summary.exists = true;
            summary.createdTime = saveData?.createdTime;
            summary.lastSaveTime = saveData?.lastSaveTime;
            summary.sceneName = saveData?.player?.sceneName ?? string.Empty;

            if (saveData?.gameTime != null)
            {
                summary.day = saveData.gameTime.day;
                summary.season = saveData.gameTime.season;
                summary.year = saveData.gameTime.year;
                summary.hour = saveData.gameTime.hour;
                summary.minute = saveData.gameTime.minute;
            }

            if (saveData?.inventory?.slots != null)
            {
                int filledCount = 0;
                for (int index = 0; index < saveData.inventory.slots.Count; index++)
                {
                    InventorySlotSaveData slot = saveData.inventory.slots[index];
                    if (slot != null && !slot.IsEmpty)
                    {
                        filledCount++;
                    }
                }

                summary.filledInventorySlots = filledCount;
            }

            if (saveData?.worldObjects != null)
            {
                for (int index = 0; index < saveData.worldObjects.Count; index++)
                {
                    WorldObjectSaveData data = saveData.worldObjects[index];
                    if (data == null || !string.Equals(data.objectType, "StoryProgressState", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(data.genericData))
                    {
                        try
                        {
                            StoryPreviewSnapshot snapshot = JsonUtility.FromJson<StoryPreviewSnapshot>(data.genericData);
                            if (snapshot != null)
                            {
                                summary.storyPhaseLabel = FormatStoryPhaseLabel(snapshot.storyPhase);
                                summary.isLanguageDecoded = snapshot.isLanguageDecoded;
                                if (snapshot.health != null)
                                {
                                    summary.healthCurrent = snapshot.health.current;
                                    summary.healthMax = snapshot.health.max;
                                }

                                if (snapshot.energy != null)
                                {
                                    summary.energyCurrent = snapshot.energy.current;
                                    summary.energyMax = snapshot.energy.max;
                                }
                            }
                        }
                        catch (Exception)
                        {
                            summary.loadError = "剧情摘要解析失败";
                        }
                    }

                    break;
                }
            }

            return summary;
        }

        private void EnsureDefaultProgressPreparedForRuntime()
        {
            if (_startupDefaultProgressHandled)
            {
                return;
            }

            _startupDefaultProgressHandled = true;

            RaiseSaveSlotsChanged();
        }

        private static bool IsValidRuntimeDefaultSave(GameSaveData saveData)
        {
            return saveData != null
                && saveData.worldObjects != null
                && saveData.worldObjects.Count > 0;
        }

        private void TryRepairDefaultProgressFromBaseline()
        {
            _startupDefaultProgressHandled = true;
        }

        private void TryPersistDefaultProgressOnQuit()
        {
            _startupDefaultProgressHandled = true;
        }

        private void ExecuteQuickSaveHotkey()
        {
            SaveActionToastOverlay.Show("F5 已停用：默认存档当前固定为原生开局，请使用普通存档槽。");
        }

        private void ExecuteQuickLoadHotkey()
        {
            bool success = QuickLoadDefaultSlot();
            SaveActionToastOverlay.Show(success ? "已读档" : "读档失败");
        }

        private bool BeginNativeFreshRestart()
        {
            if (!Application.isPlaying)
            {
                return false;
            }

            if (_nativeFreshRestartInProgress)
            {
                return false;
            }

            StartCoroutine(NativeFreshRestartRoutine());
            return true;
        }

        private IEnumerator NativeFreshRestartRoutine()
        {
            _nativeFreshRestartInProgress = true;

            AsyncOperation loadOperation = null;
            try
            {
                loadOperation = SceneManager.LoadSceneAsync(NativeFreshSceneName, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SaveManager] 原生重开失败，无法载入场景 {NativeFreshSceneName}：{exception.Message}");
            }

            if (loadOperation == null)
            {
                _nativeFreshRestartInProgress = false;
                yield break;
            }

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            ApplyNativeFreshRuntimeDefaults();
            CurrentSaveData = null;
            RefreshAllUI();
            RaiseSaveSlotsChanged();

            _nativeFreshRestartInProgress = false;
        }

        private void ApplyNativeFreshRuntimeDefaults()
        {
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTime(1, SeasonManager.Season.Spring, 1, 16, 0);
            }

            StoryProgressPersistenceService.ResetToTownOpeningRuntimeState();

            if (showDebugInfo)
            {
                Debug.Log("[SaveManager] 已按 Town 进村开局状态重建当前运行时。");
            }
        }

        private static GameSaveData CloneSaveData(GameSaveData source)
        {
            if (source == null)
            {
                return null;
            }

            string json = JsonUtility.ToJson(source);
            return JsonUtility.FromJson<GameSaveData>(json);
        }

        private static int CompareOrdinarySlotNames(string left, string right)
        {
            int leftIndex = ExtractOrdinarySlotIndex(left);
            int rightIndex = ExtractOrdinarySlotIndex(right);
            int indexCompare = leftIndex.CompareTo(rightIndex);
            return indexCompare != 0
                ? indexCompare
                : string.Compare(left, right, StringComparison.Ordinal);
        }

        private static int ExtractOrdinarySlotIndex(string slotName)
        {
            if (!string.IsNullOrWhiteSpace(slotName)
                && slotName.StartsWith(OrdinarySlotPrefix, StringComparison.Ordinal)
                && int.TryParse(slotName.Substring(OrdinarySlotPrefix.Length), out int index)
                && index > 0)
            {
                return index;
            }

            return int.MaxValue;
        }

        private string GetNextOrdinarySlotName()
        {
            HashSet<string> usedSlots = new HashSet<string>(GetOrdinarySlotNames(), StringComparer.Ordinal);
            int index = 1;
            while (usedSlots.Contains($"{OrdinarySlotPrefix}{index}"))
            {
                index++;
            }

            return $"{OrdinarySlotPrefix}{index}";
        }

        private static bool IsReservedSlot(string slotName)
        {
            return string.Equals(slotName, FreshStartBaselineSlotName, StringComparison.Ordinal)
                || string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal);
        }

        private static string FormatStoryPhaseLabel(int storyPhase)
        {
            return storyPhase switch
            {
                (int)StoryPhase.CrashAndMeet => "坠落与相遇",
                (int)StoryPhase.EnterVillage => "进入村口",
                (int)StoryPhase.HealingAndHP => "疗伤教学",
                (int)StoryPhase.WorkbenchFlashback => "工作台回想",
                (int)StoryPhase.FarmingTutorial => "农田教学",
                (int)StoryPhase.DinnerConflict => "晚餐冲突",
                (int)StoryPhase.ReturnAndReminder => "返程提醒",
                (int)StoryPhase.FreeTime => "自由行动",
                (int)StoryPhase.DayEnd => "日终",
                _ => "开局"
            };
        }

        [Serializable]
        private sealed class StoryPreviewSnapshot
        {
            public int storyPhase;
            public bool isLanguageDecoded;
            public PreviewGaugeState health;
            public PreviewGaugeState energy;
        }

        [Serializable]
        private sealed class PreviewGaugeState
        {
            public int current = -1;
            public int max = -1;
        }

        #endregion

        #region 数据收集
        
        /// <summary>
        /// 收集游戏时间数据
        /// Rule: P1-2 时间恢复 - 从 TimeManager 获取实际时间
        /// </summary>
        private GameTimeSaveData CollectGameTimeData()
        {
            var data = new GameTimeSaveData();
            
            // 从 TimeManager 获取数据
            if (TimeManager.Instance != null)
            {
                data.day = TimeManager.Instance.GetDay();
                data.season = (int)TimeManager.Instance.GetSeason();
                data.year = TimeManager.Instance.GetYear();
                data.hour = TimeManager.Instance.GetHour();
                data.minute = TimeManager.Instance.GetMinute();
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 收集时间数据: Year {data.year} Season {data.season} Day {data.day} {data.hour}:{data.minute:D2}");
            }
            else
            {
                // 回退到默认值
                data.day = 1;
                data.season = 0;
                data.year = 1;
                data.hour = 6;
                data.minute = 0;
                
                Debug.LogWarning("[SaveManager] TimeManager 未找到，使用默认时间");
            }
            
            return data;
        }
        
        /// <summary>
        /// 收集玩家数据
        /// 注意：Tool 子物体不需要排除，因为：
        /// 1. PlayerSaveData 只保存位置、场景等基础数据
        /// 2. Tool 没有实现 IPersistentObject，不会被 Registry 收集
        /// 🔥 锐评013 修复：使用 FindPlayerRoot() 确保找到真正的 Player
        /// </summary>
        private PlayerSaveData CollectPlayerData()
        {
            var data = new PlayerSaveData();
            
            // 🔥 使用 FindPlayerRoot() 而不是 FindGameObjectWithTag
            var player = FindPlayerRoot();
            if (player != null)
            {
                data.positionX = player.transform.position.x;
                data.positionY = player.transform.position.y;
                data.sceneName = SceneManager.GetActiveScene().name;
                
                // Tool 子物体不需要特殊处理：
                // - 当前只保存玩家位置，不收集子物体数据
                // - Tool 是运行时动态控制的，不需要持久化
            }
            
            return data;
        }
        
        /// <summary>
        /// 收集背包数据
        /// 注意：InventoryService 现在实现了 IPersistentObject，
        /// 会通过 PersistentObjectRegistry 自动收集
        /// 这里保留方法用于兼容性，但实际数据由 Registry 收集
        /// </summary>
        private InventorySaveData CollectInventoryData()
        {
            var data = new InventorySaveData();
            
            // InventoryService 现在通过 IPersistentObject 接口保存
            // 这里只返回空数据，实际数据在 worldObjects 中
            // 保留此方法是为了兼容旧存档格式
            
            return data;
        }
        
        #endregion
        
        #region 数据恢复
        
        /// <summary>
        /// 恢复游戏时间数据
        /// Rule: P1-2 时间恢复 - 调用 TimeManager.SetTime()
        /// </summary>
        private void RestoreGameTimeData(GameTimeSaveData data)
        {
            if (data == null) return;
            
            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTime(
                    data.year,
                    (SeasonManager.Season)data.season,
                    data.day,
                    data.hour,
                    data.minute
                );
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 恢复时间: Year {data.year} Season {data.season} Day {data.day} {data.hour}:{data.minute:D2}");
            }
            else
            {
                Debug.LogWarning("[SaveManager] TimeManager 未找到，无法恢复时间");
            }
        }
        
        /// <summary>
        /// 恢复玩家数据
        /// 优先走 PersistentPlayerSceneBridge 的稳定复位链，
        /// 避免和自动导航/跨场景持久玩家打架。
        /// </summary>
        private void RestorePlayerData(PlayerSaveData data)
        {
            if (data == null)
            {
                return;
            }

            Vector2 targetPosition = new Vector2(data.positionX, data.positionY);
            if (PersistentPlayerSceneBridge.TryApplyLoadedPlayerState(targetPosition))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 通过 bridge 恢复玩家位置: ({data.positionX}, {data.positionY})");
                }

                return;
            }

            var player = FindPlayerRoot();
            if (player == null)
            {
                Debug.LogWarning("[SaveManager] 未找到玩家根节点，无法恢复玩家位置。");
                return;
            }

            PlayerAutoNavigator autoNavigator = player.GetComponent<PlayerAutoNavigator>();
            if (autoNavigator != null)
            {
                autoNavigator.ForceCancel();
            }

            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.SetMovementInput(Vector2.zero, false);
            }

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.position = targetPosition;
            }

            player.transform.position = new Vector3(data.positionX, data.positionY, player.transform.position.z);

            if (showDebugInfo)
            {
                Debug.Log($"[SaveManager] 回退链恢复玩家位置: ({data.positionX}, {data.positionY})");
            }
        }
        
        /// <summary>
        /// 递归重置所有子物体的 localPosition
        /// 🔥 锐评012 指令：确保所有层级的子物体都归零
        /// </summary>
        private void RecursiveResetChildPositions(Transform parent)
        {
            foreach (Transform child in parent)
            {
                // Tool 必须在 (0,0,0)
                if (child.name == "Tool" || child.name.Contains("Tool"))
                {
                    if (child.localPosition != Vector3.zero)
                    {
                        Debug.Log($"[SaveManager] 重置 {child.name} localPosition: {child.localPosition} -> (0,0,0)");
                        child.localPosition = Vector3.zero;
                    }
                }
                
                // 递归处理子物体的子物体
                if (child.childCount > 0)
                {
                    RecursiveResetChildPositions(child);
                }
            }
        }
        
        /// <summary>
        /// 协程：检测下一帧玩家位置是否被"内鬼"脚本修改
        /// 🔥 锐评012 指令：如果位置被改回去，说明有脚本在 Update/LateUpdate 里强制修改位置
        /// </summary>
        private System.Collections.IEnumerator CheckPositionNextFrame(GameObject player, Vector3 targetPos)
        {
            yield return null; // 等一帧
            
            if (player == null) yield break;
            
            Vector3 currentPos = player.transform.position;
            float distance = Vector3.Distance(currentPos, targetPos);
            
            if (distance > 0.1f)
            {
                Debug.LogError($"[SaveManager] ⚠️ 异常！刚移动完一帧后，玩家位置被改回了！\n" +
                    $"  目标位置: {targetPos}\n" +
                    $"  当前位置: {currentPos}\n" +
                    $"  偏移距离: {distance}\n" +
                    $"  一定有脚本在 Update/LateUpdate 里强制修改位置！");
                
                // 检查 Tool 位置
                var tool = player.transform.Find("Tool");
                if (tool != null)
                {
                    Debug.LogError($"[SaveManager] Tool 当前状态:\n" +
                        $"  世界坐标: {tool.position}\n" +
                        $"  本地坐标: {tool.localPosition}");
                }
                
                // 检查 Rigidbody2D 位置
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    Debug.LogError($"[SaveManager] Rigidbody2D 当前位置: {rb.position}");
                }
            }
            else
            {
                Debug.Log($"[SaveManager] ✓ 位置验证通过，玩家位置稳定在: {currentPos}");
            }
        }
        
        /// <summary>
        /// 恢复背包数据
        /// 注意：InventoryService 现在实现了 IPersistentObject，
        /// 会通过 PersistentObjectRegistry 自动恢复
        /// 这里保留方法用于兼容旧存档
        /// </summary>
        private void RestoreInventoryData(InventorySaveData data)
        {
            // InventoryService 现在通过 IPersistentObject 接口恢复
            // 这里只处理旧存档格式的兼容性
            
            if (data == null || data.slots == null || data.slots.Count == 0) return;
            
            // 如果旧存档有数据，尝试迁移到新系统
            var inventory = FindFirstObjectByType<InventoryService>();
            if (inventory != null)
            {
                foreach (var slotData in data.slots)
                {
                    if (slotData.slotIndex >= 0 && slotData.slotIndex < inventory.Size && !slotData.IsEmpty)
                    {
                        // 使用新的 InventoryItem API
                        var item = SaveDataHelper.FromSaveData(slotData);
                        inventory.SetInventoryItem(slotData.slotIndex, item);
                    }
                }
                
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] 已从旧存档格式迁移背包数据");
            }
        }
        
        #endregion
        
        #region 辅助方法

        private static bool ShouldRunHeavyStartupBootstrapImmediately()
        {
            // 编辑器下保持原习惯，方便频繁调试；打包版改成懒启动，避开首屏 IO/初始化峰值。
            return Application.isEditor;
        }

        private void EnsureLegacySaveMigrationAttempted()
        {
            if (_legacyMigrationAttempted)
            {
                return;
            }

            _legacyMigrationAttempted = true;
            TryMigrateLegacySaveFolders();
        }

        private void EnsureDynamicObjectFactoryInitialized()
        {
            if (_dynamicObjectFactoryInitialized)
            {
                return;
            }

            InitializeDynamicObjectFactory();
            _dynamicObjectFactoryInitialized = true;
        }
        
        /// <summary>
        /// 初始化 DynamicObjectFactory（动态对象重建系统）
        /// 加载 PrefabRegistry 并初始化工厂
        /// </summary>
        private void InitializeDynamicObjectFactory()
        {
            // 尝试从 Resources 加载 PrefabDatabase（新版）
            var database = AssetLocator.LoadPrefabDatabase();
            
            if (database != null)
            {
                DynamicObjectFactory.Initialize(database);
                if (showDebugInfo)
                    Debug.Log("[SaveManager] DynamicObjectFactory 初始化成功（使用 PrefabDatabase）");
            }
            else
            {
                // 回退到旧版 PrefabRegistry
                #pragma warning disable 0618
                var registry = Resources.Load<PrefabRegistry>("Data/Database/PrefabRegistry");
                
                if (registry == null)
                {
                    registry = Resources.Load<PrefabRegistry>("PrefabRegistry");
                }
                
#if UNITY_EDITOR
                if (registry == null)
                {
                    var guids = UnityEditor.AssetDatabase.FindAssets("t:PrefabRegistry");
                    if (guids != null && guids.Length > 0)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                        registry = UnityEditor.AssetDatabase.LoadAssetAtPath<PrefabRegistry>(path);
                        if (showDebugInfo && registry != null)
                            Debug.Log($"[SaveManager] 从 AssetDatabase 加载 PrefabRegistry（旧版）: {path}");
                    }
                }
#endif
                
                if (registry != null)
                {
                    DynamicObjectFactory.Initialize(registry);
                    if (showDebugInfo)
                        Debug.Log("[SaveManager] DynamicObjectFactory 初始化成功（使用旧版 PrefabRegistry）");
                }
                else
                {
                    Debug.LogWarning("[SaveManager] 未找到 PrefabDatabase 或 PrefabRegistry，动态对象重建功能将不可用。" +
                        "请在 Assets/111_Data/Database/ 下创建 PrefabDatabase.asset");
                }
                #pragma warning restore 0618
            }
        }

        /// <summary>
        /// 查找真正的 Player 根节点
        /// 🔥 锐评013 修复：场景中 Tool 子物体也有 "Player" 标签，
        /// FindGameObjectWithTag 可能返回 Tool 而不是 Player 根节点
        /// 必须通过 PlayerMovement 组件来确认是真正的 Player
        /// </summary>
        private GameObject FindPlayerRoot()
        {
            // 方法 1：通过 PlayerMovement 组件查找（最可靠）
            var playerMovement = FindFirstObjectByType<PlayerMovement>();
            if (playerMovement != null)
            {
                if (showDebugInfo)
                    Debug.Log($"[SaveManager] FindPlayerRoot: 通过 PlayerMovement 找到 Player: {playerMovement.gameObject.name}");
                return playerMovement.gameObject;
            }

            // 方法 2：遍历所有 Player 标签的对象，找到有 Rigidbody2D 的那个
            var allPlayers = GameObject.FindGameObjectsWithTag("Player");
            foreach (var obj in allPlayers)
            {
                // 真正的 Player 根节点应该有 Rigidbody2D
                if (obj.GetComponent<Rigidbody2D>() != null)
                {
                    if (showDebugInfo)
                        Debug.Log($"[SaveManager] FindPlayerRoot: 通过 Rigidbody2D 找到 Player: {obj.name}");
                    return obj;
                }
            }

            // 方法 3：回退到原来的方法（不推荐，但作为最后手段）
            var fallback = GameObject.FindGameObjectWithTag("Player");
            if (fallback != null)
            {
                Debug.LogWarning($"[SaveManager] FindPlayerRoot: 使用回退方法找到: {fallback.name}，可能不是真正的 Player 根节点！");
            }
            
            return fallback;
        }
        
        /// <summary>
        /// 获取存档文件路径
        /// </summary>
        private string GetSaveFilePath(string slotName)
        {
            return Path.Combine(SaveFolderPath, slotName + saveFileExtension);
        }
        
        /// <summary>
        /// 确保存档目录存在
        /// </summary>
        private void EnsureSaveFolderExists()
        {
            if (!Directory.Exists(SaveFolderPath))
            {
                Directory.CreateDirectory(SaveFolderPath);
            }
        }

        private void TryMigrateLegacySaveFolders()
        {
            TryCopyLegacySaveFolder(LegacyProjectRootSaveFolderPath, deleteMetaArtifacts: false);
            TryCopyLegacySaveFolder(LegacyEditorSaveFolderPath, deleteMetaArtifacts: true);
        }

        private void TryCopyLegacySaveFolder(string legacyPath, bool deleteMetaArtifacts)
        {
            if (string.IsNullOrWhiteSpace(legacyPath)
                || string.Equals(Path.GetFullPath(legacyPath), Path.GetFullPath(SaveFolderPath), StringComparison.OrdinalIgnoreCase)
                || !Directory.Exists(legacyPath))
            {
                return;
            }

            try
            {
                string[] legacyFiles = Directory.GetFiles(legacyPath, "*" + saveFileExtension);
                for (int index = 0; index < legacyFiles.Length; index++)
                {
                    string legacyFile = legacyFiles[index];
                    string targetFile = Path.Combine(SaveFolderPath, Path.GetFileName(legacyFile));

                    bool shouldCopy = !File.Exists(targetFile)
                        || File.GetLastWriteTimeUtc(legacyFile) > File.GetLastWriteTimeUtc(targetFile);
                    if (shouldCopy)
                    {
                        File.Copy(legacyFile, targetFile, true);
                    }

                    if (deleteMetaArtifacts)
                    {
                        TryDeleteFileSilently(legacyFile);
                        TryDeleteFileSilently(legacyFile + ".meta");
                    }
                }

                if (deleteMetaArtifacts && Directory.Exists(legacyPath) && Directory.GetFiles(legacyPath).Length == 0)
                {
                    Directory.Delete(legacyPath, false);
                    string metaPath = legacyPath + ".meta";
                    TryDeleteFileSilently(metaPath);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[SaveManager] 迁移旧存档目录失败（{legacyPath}）：{exception.Message}");
            }
        }

        private static void TryDeleteFileSilently(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
            }
        }
        
        /// <summary>
        /// 刷新所有 UI（读档后调用）
        /// Rule: P1-1 背包刷新 - 读档后立即刷新 UI
        /// </summary>
        private void RefreshAllUI()
        {
            // 刷新背包面板
            var inventoryPanel = FindFirstObjectByType<InventoryPanelUI>();
            if (inventoryPanel != null)
            {
                inventoryPanel.RefreshAll();
            }
            
            // 刷新工具栏
            var toolbar = FindFirstObjectByType<ToolbarUI>();
            if (toolbar != null)
            {
                toolbar.ForceRefresh();
            }
            
            if (showDebugInfo)
                Debug.Log("[SaveManager] UI 已刷新");
        }
        
        #endregion
        
    }
}
