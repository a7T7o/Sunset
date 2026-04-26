using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using FarmGame.UI;
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
        private const string LegacyFreshStartBaselineSlotName = "__fresh_start_baseline__";
        private const string DefaultProgressSlotName = "__default_progress__";
        private const string OrdinarySlotPrefix = "slot";
        private const string DefaultProgressDisplayName = "默认存档";
        private const string StoryProgressPersistenceServiceTypeName = "Sunset.Story.StoryProgressPersistenceService";
        private const string SaveActionToastOverlayTypeName = "SaveActionToastOverlay";
        private const string DialoguePauseSource = "Dialogue";
        private const string StoryTimePauseSource = "SpringDay1Director";

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

        public bool HasDefaultProgressSlot => true;
        public string DefaultSaveSlotName => DefaultProgressSlotName;

        public event Action SaveSlotsChanged;

        private bool _nativeFreshRestartInProgress;
        private bool _sceneSwitchLoadInProgress;
        private bool _legacyMigrationAttempted;
        private bool _dynamicObjectFactoryInitialized;

        #endregion

        #region Unity 生命周期

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            _ = Instance;
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
        public bool LoadGame(string slotName, Action<bool> onCompleted = null)
        {
            if (!CanExecutePlayerLoadAction(out string blockerReason))
            {
                onCompleted?.Invoke(false);
                return false;
            }

            if (IsDefaultSlot(slotName))
            {
                return QuickLoadDefaultSlot(onCompleted);
            }

            return LoadGameInternal(slotName, refreshUi: true, raiseSlotChangedEvent: true, onCompleted);
        }

        /// <summary>
        /// 检查存档是否存在
        /// </summary>
        public bool SaveExists(string slotName)
        {
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
            if (string.IsNullOrWhiteSpace(slotName) || IsProtectedSlot(slotName))
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
                if (IsProtectedSlot(slotName))
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
                return DefaultProgressDisplayName;
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
            if (_nativeFreshRestartInProgress || _sceneSwitchLoadInProgress)
            {
                blockerReason = "当前仍在切场或重建运行态，请稍候再保存。";
                return false;
            }

            if (PersistentPlayerSceneBridge.IsSceneWorldRestoreInProgress())
            {
                blockerReason = "当前场景仍在恢复世界状态，请稍候再保存。";
                return false;
            }

            EnsureStoryProgressPersistenceRuntime();
            return CanSaveStoryProgressNow(out blockerReason);
        }

        public bool CanExecutePlayerLoadAction(out string blockerReason)
        {
            if (_nativeFreshRestartInProgress || _sceneSwitchLoadInProgress)
            {
                blockerReason = "当前仍在切场或重建运行态，请稍候再读取存档。";
                return false;
            }

            if (PersistentPlayerSceneBridge.IsSceneWorldRestoreInProgress())
            {
                blockerReason = "当前场景仍在恢复世界状态，请稍候再读取存档。";
                return false;
            }

            EnsureStoryProgressPersistenceRuntime();
            return CanLoadStoryProgressNow(out blockerReason);
        }

        public bool CanExecutePlayerRestartAction(out string blockerReason)
        {
            if (_nativeFreshRestartInProgress || _sceneSwitchLoadInProgress)
            {
                blockerReason = "当前仍在切场或重建运行态，请稍候再重新开始。";
                return false;
            }

            if (PersistentPlayerSceneBridge.IsSceneWorldRestoreInProgress())
            {
                blockerReason = "当前场景仍在恢复世界状态，请稍候再重新开始。";
                return false;
            }

            EnsureStoryProgressPersistenceRuntime();
            if (CanLoadStoryProgressNow(out blockerReason))
            {
                return true;
            }

            blockerReason = RewriteLoadBlockerForRestart(blockerReason);
            return false;
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
                || IsProtectedSlot(targetSlotName))
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
            return SaveGameInternal(DefaultProgressSlotName, enforceSaveBlockers: true, raiseSlotChangedEvent: true, allowProtectedDefaultSlotWrite: true);
        }

        public bool QuickLoadDefaultSlot(Action<bool> onCompleted = null)
        {
            if (!SaveExists(DefaultProgressSlotName))
            {
                onCompleted?.Invoke(false);
                return false;
            }

            if (!CanExecutePlayerLoadAction(out string blockerReason))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 默认存档读取被拦截：{blockerReason}");
                }

                onCompleted?.Invoke(false);
                return false;
            }

            return LoadGameInternal(DefaultProgressSlotName, refreshUi: true, raiseSlotChangedEvent: true, onCompleted);
        }

        public bool RestartToFreshGame(Action<bool> onCompleted = null)
        {
            if (!CanExecutePlayerRestartAction(out string blockerReason))
            {
                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 重新开始被拦截：{blockerReason}");
                }

                onCompleted?.Invoke(false);
                return false;
            }

            return BeginNativeFreshRestart(onCompleted);
        }

        public bool TryGetDefaultSlotSummary(out SaveSlotSummary summary)
        {
            summary = CreateEmptyDefaultSlotSummary();

            string filePath = GetSaveFilePath(DefaultProgressSlotName);
            if (!File.Exists(filePath))
            {
                return true;
            }

            if (!TryReadSaveData(DefaultProgressSlotName, out GameSaveData saveData))
            {
                summary.exists = true;
                summary.loadError = "默认存档文件损坏或无法解析";
                return false;
            }

            summary = BuildSlotSummary(DefaultProgressSlotName, saveData);
            summary.isDefaultSlot = true;
            summary.displayName = DefaultProgressDisplayName;
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

        private bool SaveGameInternal(string slotName, bool enforceSaveBlockers, bool raiseSlotChangedEvent, bool allowProtectedDefaultSlotWrite = false)
        {
            if (string.IsNullOrWhiteSpace(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                return false;
            }

            if (IsInternalReservedSlot(slotName))
            {
                Debug.LogWarning("[SaveManager] 旧版默认基线槽已退役，不再允许写入。");
                return false;
            }

            if (IsDefaultSlot(slotName) && !allowProtectedDefaultSlotWrite)
            {
                Debug.LogWarning("[SaveManager] 默认存档不允许通过普通覆盖入口改写，请使用 F5 或默认槽快速保存。");
                return false;
            }

            EnsureLegacySaveMigrationAttempted();
            EnsureStoryProgressPersistenceRuntime();
            if (enforceSaveBlockers && !CanSaveStoryProgressNow(out string saveBlockerReason))
            {
                Debug.LogWarning($"[SaveManager] 当前不可存档：{saveBlockerReason}");
                return false;
            }

            try
            {
                EnsureCriticalPersistentRuntimeServicesRegistered();
                GameSaveData saveData = CollectFullSaveData();
                if (!ValidateRequiredSavePayloads(saveData, out string payloadError))
                {
                    Debug.LogWarning($"[SaveManager] 当前存档快照不完整，已拒绝写盘：{payloadError}");
                    return false;
                }

                WriteSaveData(slotName, saveData);
                CurrentSaveData = saveData;

                if (raiseSlotChangedEvent)
                {
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

        private bool LoadGameInternal(string slotName, bool refreshUi, bool raiseSlotChangedEvent, Action<bool> onCompleted = null)
        {
            if (string.IsNullOrWhiteSpace(slotName))
            {
                Debug.LogError("[SaveManager] 存档名称不能为空");
                onCompleted?.Invoke(false);
                return false;
            }

            if (!TryReadSaveData(slotName, out GameSaveData saveData))
            {
                Debug.LogWarning($"[SaveManager] 存档文件不存在或解析失败: {GetSaveFilePath(slotName)}");
                onCompleted?.Invoke(false);
                return false;
            }

            string targetSceneName = ResolveTargetLoadSceneName(saveData);
            if (ShouldSceneSwitchBeforeLoad(targetSceneName))
            {
                return BeginSceneSwitchLoad(saveData, targetSceneName, refreshUi, raiseSlotChangedEvent, onCompleted);
            }

            return ApplyLoadedSaveData(saveData, refreshUi, raiseSlotChangedEvent, onCompleted);
        }

        private bool ApplyLoadedSaveData(GameSaveData saveData, bool refreshUi, bool raiseSlotChangedEvent, Action<bool> onCompleted = null)
        {
            try
            {
                EnsureDynamicObjectFactoryInitialized();
                EnsureStoryProgressPersistenceRuntime();
                ResetTransientRuntimeForRestore("读档前恢复入口清理");

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
                ImportCloudShadowPersistentSaveData(saveData.cloudShadowScenes);

                List<WorldObjectSaveData> orderedWorldObjects = SortWorldObjectsForRestore(saveData.worldObjects);

                EnsureCriticalPersistentRuntimeServicesRegistered();
                if (PersistentObjectRegistry.Instance != null && orderedWorldObjects != null)
                {
                    PersistentObjectRegistry.Instance.RestoreAllFromSaveData(orderedWorldObjects);
                }

                PersistentPlayerSceneBridge.ImportOffSceneWorldSnapshotsFromSave(saveData.offSceneWorldSnapshots);

                FinalizeStoryProgressLoaded(orderedWorldObjects);

                CurrentSaveData = saveData;
                PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot();
                PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings();

                if (showDebugInfo)
                {
                    Debug.Log($"[SaveManager] 加载成功: scene={ResolveTargetLoadSceneName(saveData)}, 世界对象: {saveData.worldObjects?.Count ?? 0}");
                }

                if (refreshUi)
                {
                    RefreshAllUI();
                }

                PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot();

                if (raiseSlotChangedEvent)
                {
                    RaiseSaveSlotsChanged();
                }

                onCompleted?.Invoke(true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveManager] 加载失败: {e.Message}\n{e.StackTrace}");
                onCompleted?.Invoke(false);
                return false;
            }
        }

        private bool BeginSceneSwitchLoad(GameSaveData saveData, string targetSceneName, bool refreshUi, bool raiseSlotChangedEvent, Action<bool> onCompleted = null)
        {
            if (_sceneSwitchLoadInProgress)
            {
                Debug.LogWarning("[SaveManager] 当前已有读档切场流程在进行，已忽略新的读档请求。");
                onCompleted?.Invoke(false);
                return false;
            }

            if (!Application.isPlaying)
            {
                onCompleted?.Invoke(false);
                return false;
            }

            PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(targetSceneName);
            StartCoroutine(LoadAfterSceneSwitchRoutine(saveData, targetSceneName, refreshUi, raiseSlotChangedEvent, onCompleted));
            return true;
        }

        private IEnumerator LoadAfterSceneSwitchRoutine(GameSaveData saveData, string targetSceneName, bool refreshUi, bool raiseSlotChangedEvent, Action<bool> onCompleted)
        {
            _sceneSwitchLoadInProgress = true;

            AsyncOperation loadOperation = null;
            try
            {
                loadOperation = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(targetSceneName);
                Debug.LogError($"[SaveManager] 读档切场失败，无法载入场景 {targetSceneName}：{exception.Message}");
            }

            if (loadOperation == null)
            {
                PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(targetSceneName);
                _sceneSwitchLoadInProgress = false;
                onCompleted?.Invoke(false);
                yield break;
            }

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            ApplyLoadedSaveData(saveData, refreshUi, raiseSlotChangedEvent, onCompleted);
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

        private static List<WorldObjectSaveData> SortWorldObjectsForRestore(List<WorldObjectSaveData> worldObjects)
        {
            if (worldObjects == null || worldObjects.Count <= 0)
            {
                return worldObjects;
            }

            List<WorldObjectSaveData> ordered = new List<WorldObjectSaveData>(worldObjects);
            ordered.Sort((left, right) =>
            {
                int priorityCompare = ResolveWorldObjectRestorePriority(left?.objectType)
                    .CompareTo(ResolveWorldObjectRestorePriority(right?.objectType));
                if (priorityCompare != 0)
                {
                    return priorityCompare;
                }

                return string.Compare(left?.guid, right?.guid, StringComparison.OrdinalIgnoreCase);
            });
            return ordered;
        }

        private static int ResolveWorldObjectRestorePriority(string objectType)
        {
            switch (objectType)
            {
                case "PlayerInventory":
                    return 0;
                case "FarmTileManager":
                    return 10;
                case "Crop":
                    return 20;
                case "Chest":
                    return 30;
                case "StoryProgressState":
                    return 40;
                case "Drop":
                    return 50;
                case "Tree":
                    return 60;
                case "Stone":
                    return 70;
                default:
                    return 100;
            }
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
                // 正式存档当前只收“当前已加载 scene 内”的持久对象。
                // 已离场 scene 的 runtime continuity 仍由 PersistentPlayerSceneBridge 维护；
                // 在不补 bridge 消费合同前，不能把 off-scene world state 粗暴并进 worldObjects。
                saveData.worldObjects = PersistentObjectRegistry.Instance.CollectAllSaveData();
            }

            saveData.cloudShadowScenes = ExportCloudShadowPersistentSaveData();
            saveData.offSceneWorldSnapshots = PersistentPlayerSceneBridge.ExportOffSceneWorldSnapshotsForSave();

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
            // 旧 baseline 文件名只保留隐藏兼容，
            // 防止历史文件被误当成普通存档重新露出来。
            if (IsInternalReservedSlot(slotName))
            {
                return false;
            }

            EnsureLegacySaveMigrationAttempted();

            string filePath = GetSaveFilePath(slotName);
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                string json = File.ReadAllText(filePath);
                saveData = JsonUtility.FromJson<GameSaveData>(json);
                NormalizeLoadedSaveData(saveData);
                return saveData != null;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[SaveManager] 读取存档 {slotName} 失败：{exception.Message}");
                return false;
            }
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
                displayName = string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal) ? DefaultProgressDisplayName : slotName,
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

        private static SaveSlotSummary CreateEmptyDefaultSlotSummary()
        {
            return new SaveSlotSummary
            {
                slotName = DefaultProgressSlotName,
                displayName = DefaultProgressDisplayName,
                exists = false,
                isDefaultSlot = true,
                storyPhaseLabel = "未保存"
            };
        }

        private void ExecuteQuickSaveHotkey()
        {
            if (QuickSaveDefaultSlot())
            {
                ShowSaveActionToast("已快速保存到默认存档");
                return;
            }

            ShowSaveActionToast(CanExecutePlayerSaveAction(out string blockerReason)
                ? "默认存档保存失败"
                : blockerReason);
        }

        private void ExecuteQuickLoadHotkey()
        {
            if (!SaveExists(DefaultProgressSlotName))
            {
                ShowSaveActionToast("默认存档为空，请先按 F5 快速保存");
                return;
            }

            if (!CanExecutePlayerLoadAction(out string blockerReason))
            {
                ShowSaveActionToast(blockerReason);
                return;
            }

            QuickLoadDefaultSlot(success => ShowSaveActionToast(success ? "已读取默认存档" : "默认存档读取失败"));
        }

        private bool BeginNativeFreshRestart(Action<bool> onCompleted = null)
        {
            if (!Application.isPlaying)
            {
                onCompleted?.Invoke(false);
                return false;
            }

            if (_nativeFreshRestartInProgress)
            {
                onCompleted?.Invoke(false);
                return false;
            }

            StartCoroutine(NativeFreshRestartRoutine(onCompleted));
            return true;
        }

        private IEnumerator NativeFreshRestartRoutine(Action<bool> onCompleted)
        {
            _nativeFreshRestartInProgress = true;

            AsyncOperation loadOperation = null;
            try
            {
                PersistentPlayerSceneBridge.SuppressSceneWorldRestoreForScene(NativeFreshSceneName);
                loadOperation = SceneManager.LoadSceneAsync(NativeFreshSceneName, LoadSceneMode.Single);
            }
            catch (Exception exception)
            {
                PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(NativeFreshSceneName);
                Debug.LogError($"[SaveManager] 原生重开失败，无法载入场景 {NativeFreshSceneName}：{exception.Message}");
            }

            if (loadOperation == null)
            {
                PersistentPlayerSceneBridge.CancelSuppressedSceneWorldRestore(NativeFreshSceneName);
                _nativeFreshRestartInProgress = false;
                onCompleted?.Invoke(false);
                yield break;
            }

            while (!loadOperation.isDone)
            {
                yield return null;
            }

            yield return null;
            yield return new WaitForEndOfFrame();

            bool success = false;
            try
            {
                ApplyNativeFreshRuntimeDefaults();
                CurrentSaveData = null;
                PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot();
                PersistentPlayerSceneBridge.RefreshActiveSceneRuntimeBindings();
                RefreshAllUI();
                PersistentPlayerSceneBridge.SyncActiveSceneInventorySnapshot();
                RaiseSaveSlotsChanged();
                success = true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"[SaveManager] 原生重开应用默认运行态失败：{exception.Message}\n{exception.StackTrace}");
            }

            _nativeFreshRestartInProgress = false;
            onCompleted?.Invoke(success);
        }

        private void ApplyNativeFreshRuntimeDefaults()
        {
            PersistentPlayerSceneBridge.ResetPersistentRuntimeForFreshStart();
            ResetTransientRuntimeForRestore("新开局重建默认运行态前恢复入口清理");

            if (TimeManager.Instance != null)
            {
                TimeManager.Instance.SetTime(1, SeasonManager.Season.Spring, 1, 9, 0);
            }

            ResetStoryProgressToTownOpeningRuntimeState();

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

        private static bool IsInternalReservedSlot(string slotName)
        {
            return string.Equals(slotName, LegacyFreshStartBaselineSlotName, StringComparison.Ordinal);
        }

        private static bool IsProtectedSlot(string slotName)
        {
            return IsInternalReservedSlot(slotName)
                || string.Equals(slotName, DefaultProgressSlotName, StringComparison.Ordinal);
        }

        private static Type ResolveStoryProgressPersistenceServiceType()
        {
            return typeof(SaveManager).Assembly.GetType(StoryProgressPersistenceServiceTypeName);
        }

        private static bool TryInvokeStoryProgressPersistenceMethod(string methodName, object[] parameters, out object returnValue)
        {
            returnValue = null;

            Type serviceType = ResolveStoryProgressPersistenceServiceType();
            if (serviceType == null)
            {
                return false;
            }

            MethodInfo method = serviceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
            if (method == null)
            {
                return false;
            }

            returnValue = method.Invoke(null, parameters);
            return true;
        }

        private static void EnsureStoryProgressPersistenceRuntime()
        {
            TryInvokeStoryProgressPersistenceMethod("EnsureRuntime", Array.Empty<object>(), out _);
        }

        private static void EnsureCriticalPersistentRuntimeServicesRegistered()
        {
            EnsureStoryProgressPersistenceRuntime();

            PersistentObjectRegistry registry = PersistentObjectRegistry.Instance;
            if (registry == null)
            {
                return;
            }

            InventoryService runtimeInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
            TryRegisterCriticalPersistentObject(registry, runtimeInventory, "PlayerInventory");

            EquipmentService runtimeEquipment = PersistentPlayerSceneBridge.GetPreferredRuntimeEquipmentService()
                ?? FindFirstObjectByType<EquipmentService>(FindObjectsInactive.Include);
            TryRegisterCriticalPersistentObject(registry, runtimeEquipment, "EquipmentService");
        }

        private static void TryRegisterCriticalPersistentObject(
            PersistentObjectRegistry registry,
            IPersistentObject persistentObject,
            string objectType)
        {
            if (registry == null || persistentObject == null)
            {
                return;
            }

            if (registry.TryRegister(persistentObject))
            {
                return;
            }

            if (!ReferenceEquals(registry.FindByGuid(persistentObject.PersistentId), persistentObject))
            {
                Debug.LogWarning($"[SaveManager] 关键持久对象注册未就位：{objectType}, GUID={persistentObject.PersistentId}");
            }
        }

        private static bool ValidateRequiredSavePayloads(GameSaveData saveData, out string error)
        {
            if (saveData == null)
            {
                error = "根存档数据为空";
                return false;
            }

            if (saveData.gameTime == null)
            {
                error = "时间数据缺失";
                return false;
            }

            if (saveData.player == null)
            {
                error = "玩家基础数据缺失";
                return false;
            }

            if (!ContainsWorldObjectType(saveData.worldObjects, "StoryProgressState"))
            {
                error = "剧情长期态未进入正式存档";
                return false;
            }

            InventoryService runtimeInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
            if (runtimeInventory != null && !ContainsWorldObjectType(saveData.worldObjects, "PlayerInventory"))
            {
                error = "玩家背包未进入正式存档";
                return false;
            }

            EquipmentService runtimeEquipment = PersistentPlayerSceneBridge.GetPreferredRuntimeEquipmentService()
                ?? FindFirstObjectByType<EquipmentService>(FindObjectsInactive.Include);
            if (runtimeEquipment != null && !ContainsWorldObjectType(saveData.worldObjects, "EquipmentService"))
            {
                error = "装备栏未进入正式存档";
                return false;
            }

            error = null;
            return true;
        }

        private static bool ContainsWorldObjectType(List<WorldObjectSaveData> worldObjects, string objectType)
        {
            if (worldObjects == null || string.IsNullOrWhiteSpace(objectType))
            {
                return false;
            }

            for (int index = 0; index < worldObjects.Count; index++)
            {
                WorldObjectSaveData worldObject = worldObjects[index];
                if (worldObject != null
                    && string.Equals(worldObject.objectType, objectType, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool CanSaveStoryProgressNow(out string blockerReason)
        {
            object[] parameters = { null };
            if (TryInvokeStoryProgressPersistenceMethod("CanSaveNow", parameters, out object result)
                && result is bool canSave)
            {
                blockerReason = parameters[0] as string;
                return canSave;
            }

            blockerReason = null;
            return true;
        }

        private static bool CanLoadStoryProgressNow(out string blockerReason)
        {
            object[] parameters = { null };
            if (TryInvokeStoryProgressPersistenceMethod("CanLoadNow", parameters, out object result)
                && result is bool canLoad)
            {
                blockerReason = parameters[0] as string;
                return canLoad;
            }

            blockerReason = null;
            return true;
        }

        private static string RewriteLoadBlockerForRestart(string blockerReason)
        {
            if (string.IsNullOrWhiteSpace(blockerReason))
            {
                return "当前还在剧情或场景接管中，请稍候再重新开始。";
            }

            return blockerReason
                .Replace("读取存档", "重新开始")
                .Replace("操作存档", "重新开始");
        }

        private static void FinalizeStoryProgressLoaded(List<WorldObjectSaveData> worldObjects)
        {
            object[] parameters = { worldObjects };
            TryInvokeStoryProgressPersistenceMethod("FinalizeLoadedSave", parameters, out _);
        }

        private static void ResetStoryProgressToTownOpeningRuntimeState()
        {
            TryInvokeStoryProgressPersistenceMethod("ResetToTownOpeningRuntimeState", Array.Empty<object>(), out _);
        }

        private static void ResetTransientRuntimeForRestore(string reason)
        {
            StopActiveDialogueForRestore();
            ClosePackageAndBoxUiForRestore();
            CloseWorkbenchOverlayForRestore();
            ResetInventoryInteractionForRestore();
            HideTransientOverlayUiForRestore();
            HideTransientBubbleUiForRestore();
            ResetKnownTimePauseSourcesForRestore();
            GameInputManager.ForceResetPlacementRuntime(reason);
        }

        private static void StopActiveDialogueForRestore()
        {
            DialogueManager dialogueManager = DialogueManager.Instance
                ?? FindFirstObjectByType<DialogueManager>(FindObjectsInactive.Include);
            dialogueManager?.StopDialogue();
        }

        private static void ClosePackageAndBoxUiForRestore()
        {
            PackagePanelTabsUI[] packagePanels = FindObjectsByType<PackagePanelTabsUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < packagePanels.Length; index++)
            {
                packagePanels[index]?.ClosePanelForExternalAction();
            }

            BoxPanelUI activeBox = BoxPanelUI.ActiveInstance;
            if (activeBox != null && activeBox.IsOpen)
            {
                activeBox.Close();
            }
        }

        private static void CloseWorkbenchOverlayForRestore()
        {
            SpringDay1WorkbenchCraftingOverlay[] overlays = FindObjectsByType<SpringDay1WorkbenchCraftingOverlay>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < overlays.Length; index++)
            {
                overlays[index]?.Hide();
            }
        }

        private static void ResetInventoryInteractionForRestore()
        {
            InventoryInteractionManager interactionManager = InventoryInteractionManager.Instance
                ?? FindFirstObjectByType<InventoryInteractionManager>(FindObjectsInactive.Include);
            interactionManager?.Cancel();

            InventorySlotInteraction.ResetActiveChestHeldState();
            ItemTooltip.Instance?.Hide();
            ItemUseConfirmDialog.Instance?.Hide();
        }

        private static void HideTransientOverlayUiForRestore()
        {
            SpringDay1PromptOverlay promptOverlay = FindFirstObjectByType<SpringDay1PromptOverlay>(FindObjectsInactive.Include);
            if (promptOverlay != null)
            {
                promptOverlay.SetExternalVisibilityBlock(false);
                promptOverlay.Hide();
            }

            SpringDay1Director director = SpringDay1Director.Instance
                ?? FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            director?.HideTaskListBridgePrompt();

            InteractionHintOverlay.HideIfExists();
            NpcWorldHintBubble.HideIfExists();
            SpringDay1WorldHintBubble.HideIfExists();
        }

        private static void HideTransientBubbleUiForRestore()
        {
            PlayerThoughtBubblePresenter[] playerThoughtBubbles = FindObjectsByType<PlayerThoughtBubblePresenter>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < playerThoughtBubbles.Length; index++)
            {
                playerThoughtBubbles[index]?.HideImmediate();
            }

            NPCBubblePresenter[] npcBubbles = FindObjectsByType<NPCBubblePresenter>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < npcBubbles.Length; index++)
            {
                npcBubbles[index]?.HideImmediateBubble();
            }
        }

        private static void ResetKnownTimePauseSourcesForRestore()
        {
            TimeManager timeManager = TimeManager.Instance
                ?? FindFirstObjectByType<TimeManager>(FindObjectsInactive.Include);
            if (timeManager == null)
            {
                return;
            }

            timeManager.ResumeTime(DialoguePauseSource);
            timeManager.ResumeTime(StoryTimePauseSource);
            timeManager.SetPaused(false);
        }

        private static List<CloudShadowSceneSaveData> ExportCloudShadowPersistentSaveData()
        {
            MethodInfo exportMethod = typeof(CloudShadowManager).GetMethod("ExportPersistentSaveData", BindingFlags.Public | BindingFlags.Static);
            if (exportMethod == null)
            {
                return new List<CloudShadowSceneSaveData>();
            }

            return exportMethod.Invoke(null, null) as List<CloudShadowSceneSaveData> ?? new List<CloudShadowSceneSaveData>();
        }

        private static void ImportCloudShadowPersistentSaveData(List<CloudShadowSceneSaveData> serializedStates)
        {
            MethodInfo importMethod = typeof(CloudShadowManager).GetMethod("ImportPersistentSaveData", BindingFlags.Public | BindingFlags.Static);
            if (importMethod == null)
            {
                return;
            }

            importMethod.Invoke(null, new object[] { serializedStates });
        }

        private static void ShowSaveActionToast(string message)
        {
            Type toastType = typeof(SaveManager).Assembly.GetType(SaveActionToastOverlayTypeName);
            MethodInfo showMethod = toastType?.GetMethod("Show", BindingFlags.Public | BindingFlags.Static);
            if (showMethod == null)
            {
                return;
            }

            showMethod.Invoke(null, new object[] { message });
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
            public int storyPhase = 0;
            public bool isLanguageDecoded = false;
            public PreviewGaugeState health = null;
            public PreviewGaugeState energy = null;
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

            HotbarSelectionService hotbarSelection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>(FindObjectsInactive.Include);
            if (hotbarSelection != null)
            {
                data.selectedHotbarSlot = hotbarSelection.selectedIndex;
                data.selectedInventoryIndex = hotbarSelection.selectedInventoryIndex;
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
            }
            else
            {
                var player = FindPlayerRoot();
                if (player == null)
                {
                    Debug.LogWarning("[SaveManager] 未找到玩家根节点，无法恢复玩家位置。");
                }
                else
                {
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
            }

            RestoreHotbarSelectionForLoadedPlayer(data);
        }

        private static void RestoreHotbarSelectionForLoadedPlayer(PlayerSaveData data)
        {
            if (data == null)
            {
                return;
            }

            HotbarSelectionService hotbarSelection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>(FindObjectsInactive.Include);
            if (hotbarSelection == null)
            {
                return;
            }

            int restoredHotbarIndex = Mathf.Clamp(data.selectedHotbarSlot, 0, InventoryService.HotbarWidth - 1);
            int restoredInventoryIndex = data.selectedInventoryIndex >= 0
                ? data.selectedInventoryIndex
                : restoredHotbarIndex;
            hotbarSelection.RestoreSelectionState(restoredHotbarIndex, restoredInventoryIndex);
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
            var inventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
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

        private static void NormalizeLoadedSaveData(GameSaveData saveData)
        {
            if (saveData == null)
            {
                return;
            }

            saveData.worldObjects ??= new List<WorldObjectSaveData>();
            saveData.farmTiles ??= new List<FarmTileSaveData>();
            saveData.cloudShadowScenes ??= new List<CloudShadowSceneSaveData>();
            saveData.offSceneWorldSnapshots ??= new List<SceneWorldSnapshotSaveData>();

            PromoteLegacyFarmStateForLoad(
                saveData.worldObjects,
                saveData.farmTiles,
                NormalizeSceneName(saveData.player?.sceneName));

            for (int index = 0; index < saveData.offSceneWorldSnapshots.Count; index++)
            {
                SceneWorldSnapshotSaveData snapshot = saveData.offSceneWorldSnapshots[index];
                if (snapshot == null)
                {
                    continue;
                }

                snapshot.worldObjects ??= new List<WorldObjectSaveData>();
                PromoteLegacyFarmStateForLoad(
                    snapshot.worldObjects,
                    null,
                    NormalizeSceneName(snapshot.sceneKey));
            }
        }

        private static void PromoteLegacyFarmStateForLoad(
            List<WorldObjectSaveData> worldObjects,
            List<FarmTileSaveData> rootFarmTiles,
            string fallbackSceneName)
        {
            if (worldObjects == null)
            {
                return;
            }

            PromoteLegacyRootFarmTilesIntoWorldObjects(worldObjects, rootFarmTiles, fallbackSceneName);

            HashSet<string> existingCropCellKeys = CollectExistingCropCellKeys(worldObjects);
            List<WorldObjectSaveData> promotedLegacyCrops = new List<WorldObjectSaveData>();

            for (int index = 0; index < worldObjects.Count; index++)
            {
                WorldObjectSaveData worldObject = worldObjects[index];
                if (worldObject == null
                    || !string.Equals(worldObject.objectType, "FarmTileManager", StringComparison.Ordinal)
                    || string.IsNullOrWhiteSpace(worldObject.genericData))
                {
                    continue;
                }

                FarmTileListWrapper wrapper = JsonUtility.FromJson<FarmTileListWrapper>(worldObject.genericData);
                if (wrapper?.tiles == null)
                {
                    continue;
                }

                string sceneName = NormalizeSceneName(worldObject.sceneName);
                if (string.IsNullOrWhiteSpace(sceneName))
                {
                    sceneName = fallbackSceneName;
                }

                for (int tileIndex = 0; tileIndex < wrapper.tiles.Count; tileIndex++)
                {
                    FarmTileSaveData tile = wrapper.tiles[tileIndex];
                    if (!HasLegacyCropPayload(tile))
                    {
                        continue;
                    }

                    string cropCellKey = BuildLegacyCropCellKey(tile.layer, tile.tileX, tile.tileY);
                    if (existingCropCellKeys.Contains(cropCellKey))
                    {
                        continue;
                    }

                    WorldObjectSaveData legacyCrop = BuildLegacyCropWorldObject(tile, sceneName);
                    if (legacyCrop == null)
                    {
                        continue;
                    }

                    promotedLegacyCrops.Add(legacyCrop);
                    existingCropCellKeys.Add(cropCellKey);
                }
            }

            if (promotedLegacyCrops.Count > 0)
            {
                worldObjects.AddRange(promotedLegacyCrops);
            }
        }

        private static void PromoteLegacyRootFarmTilesIntoWorldObjects(
            List<WorldObjectSaveData> worldObjects,
            List<FarmTileSaveData> rootFarmTiles,
            string sceneName)
        {
            if (worldObjects == null || rootFarmTiles == null || rootFarmTiles.Count <= 0)
            {
                return;
            }

            WorldObjectSaveData farmTileManagerData = FindWorldObjectSaveDataByType(worldObjects, "FarmTileManager");
            if (farmTileManagerData != null && !string.IsNullOrWhiteSpace(farmTileManagerData.genericData))
            {
                return;
            }

            WorldObjectSaveData promotedFarmTileManager = new WorldObjectSaveData
            {
                guid = "FarmTileManager",
                objectType = "FarmTileManager",
                sceneName = sceneName,
                isActive = true,
                genericData = JsonUtility.ToJson(new FarmTileListWrapper
                {
                    tiles = CloneLegacyFarmTiles(rootFarmTiles)
                })
            };

            if (farmTileManagerData == null)
            {
                worldObjects.Add(promotedFarmTileManager);
                return;
            }

            farmTileManagerData.guid = string.IsNullOrWhiteSpace(farmTileManagerData.guid)
                ? promotedFarmTileManager.guid
                : farmTileManagerData.guid;
            farmTileManagerData.sceneName = string.IsNullOrWhiteSpace(farmTileManagerData.sceneName)
                ? promotedFarmTileManager.sceneName
                : farmTileManagerData.sceneName;
            farmTileManagerData.isActive = true;
            farmTileManagerData.genericData = promotedFarmTileManager.genericData;
        }

        #pragma warning disable 0618
        private static List<FarmTileSaveData> CloneLegacyFarmTiles(List<FarmTileSaveData> source)
        {
            List<FarmTileSaveData> cloned = new List<FarmTileSaveData>(source?.Count ?? 0);
            if (source == null)
            {
                return cloned;
            }

            for (int index = 0; index < source.Count; index++)
            {
                FarmTileSaveData tile = source[index];
                if (tile == null)
                {
                    continue;
                }

                cloned.Add(new FarmTileSaveData
                {
                    tileX = tile.tileX,
                    tileY = tile.tileY,
                    layer = tile.layer,
                    soilState = tile.soilState,
                    isWatered = tile.isWatered,
                    wateredYesterday = tile.wateredYesterday,
                    waterTime = tile.waterTime,
                    puddleVariant = tile.puddleVariant,
                    hasEmptySinceRecord = tile.hasEmptySinceRecord,
                    emptySinceTotalDays = tile.emptySinceTotalDays,
                    cropId = tile.cropId,
                    cropGrowthStage = tile.cropGrowthStage,
                    cropQuality = tile.cropQuality,
                    daysGrown = tile.daysGrown,
                    daysWithoutWater = tile.daysWithoutWater
                });
            }

            return cloned;
        }

        private static HashSet<string> CollectExistingCropCellKeys(List<WorldObjectSaveData> worldObjects)
        {
            HashSet<string> keys = new HashSet<string>(StringComparer.Ordinal);
            if (worldObjects == null)
            {
                return keys;
            }

            for (int index = 0; index < worldObjects.Count; index++)
            {
                WorldObjectSaveData worldObject = worldObjects[index];
                if (worldObject == null
                    || !string.Equals(worldObject.objectType, "Crop", StringComparison.Ordinal)
                    || string.IsNullOrWhiteSpace(worldObject.genericData))
                {
                    continue;
                }

                CropSaveData cropData = JsonUtility.FromJson<CropSaveData>(worldObject.genericData);
                if (cropData == null)
                {
                    continue;
                }

                keys.Add(BuildLegacyCropCellKey(cropData.layerIndex, cropData.cellX, cropData.cellY));
            }

            return keys;
        }

        private static bool HasLegacyCropPayload(FarmTileSaveData tile)
        {
            return tile != null && tile.cropId >= 0;
        }

        private static string BuildLegacyCropCellKey(int layerIndex, int cellX, int cellY)
        {
            return $"{layerIndex}:{cellX}:{cellY}";
        }

        private static WorldObjectSaveData FindWorldObjectSaveDataByType(List<WorldObjectSaveData> worldObjects, string objectType)
        {
            if (worldObjects == null || string.IsNullOrWhiteSpace(objectType))
            {
                return null;
            }

            for (int index = 0; index < worldObjects.Count; index++)
            {
                WorldObjectSaveData worldObject = worldObjects[index];
                if (worldObject != null && string.Equals(worldObject.objectType, objectType, StringComparison.Ordinal))
                {
                    return worldObject;
                }
            }

            return null;
        }

        private static WorldObjectSaveData BuildLegacyCropWorldObject(FarmTileSaveData tile, string sceneName)
        {
            if (!HasLegacyCropPayload(tile))
            {
                return null;
            }

            CropSaveData cropData = new CropSaveData
            {
                seedId = tile.cropId,
                currentStage = Mathf.Max(0, tile.cropGrowthStage),
                grownDays = Mathf.Max(0, tile.daysGrown),
                daysWithoutWater = Mathf.Max(0, tile.daysWithoutWater),
                isWithered = false,
                quality = Mathf.Max(0, tile.cropQuality),
                harvestCount = 0,
                lastHarvestDay = 0,
                daysSinceMature = 0,
                layerIndex = tile.layer,
                cellX = tile.tileX,
                cellY = tile.tileY
            };

            return new WorldObjectSaveData
            {
                guid = $"LegacyCrop_{tile.layer}_{tile.tileX}_{tile.tileY}",
                objectType = "Crop",
                prefabId = tile.cropId.ToString(),
                sceneName = sceneName,
                layer = tile.layer,
                positionX = tile.tileX + 0.5f,
                positionY = tile.tileY + 0.5f,
                positionZ = 0f,
                isActive = true,
                genericData = JsonUtility.ToJson(cropData)
            };
        }
        #pragma warning restore 0618

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
            InventoryService runtimeInventory = PersistentPlayerSceneBridge.GetPreferredRuntimeInventoryService()
                ?? FindFirstObjectByType<InventoryService>(FindObjectsInactive.Include);
            EquipmentService runtimeEquipment = PersistentPlayerSceneBridge.GetPreferredRuntimeEquipmentService()
                ?? FindFirstObjectByType<EquipmentService>(FindObjectsInactive.Include);
            ItemDatabase runtimeDatabase = runtimeInventory != null ? runtimeInventory.Database : null;
            HotbarSelectionService runtimeSelection = PersistentPlayerSceneBridge.GetPreferredRuntimeHotbarSelectionService()
                ?? FindFirstObjectByType<HotbarSelectionService>(FindObjectsInactive.Include);
            InventorySortService sortService = FindFirstObjectByType<InventorySortService>(FindObjectsInactive.Include);
            if (sortService != null)
            {
                sortService.RebindRuntimeContext(runtimeInventory, runtimeDatabase);
            }

            CraftingService[] craftingServices = FindObjectsByType<CraftingService>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < craftingServices.Length; index++)
            {
                craftingServices[index]?.ConfigureRuntimeContext(runtimeInventory, runtimeDatabase);
            }

            InventoryService[] inventories = FindObjectsByType<InventoryService>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < inventories.Length; index++)
            {
                inventories[index]?.RefreshAll();
            }

            HotbarSelectionService[] selections = FindObjectsByType<HotbarSelectionService>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < selections.Length; index++)
            {
                selections[index]?.ReassertCurrentSelection(collapseInventorySelectionToHotbar: false, invokeEvent: true);
            }

            InventoryPanelUI[] inventoryPanels = FindObjectsByType<InventoryPanelUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < inventoryPanels.Length; index++)
            {
                InventoryPanelUI inventoryPanel = inventoryPanels[index];
                if (inventoryPanel == null)
                {
                    continue;
                }

                inventoryPanel.ConfigureRuntimeContext(
                    runtimeInventory,
                    runtimeEquipment,
                    runtimeDatabase,
                    runtimeSelection);
                inventoryPanel.EnsureBuilt();
                inventoryPanel.RefreshAll();
            }

            PackagePanelTabsUI[] packagePanels = FindObjectsByType<PackagePanelTabsUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < packagePanels.Length; index++)
            {
                PackagePanelTabsUI packagePanel = packagePanels[index];
                if (packagePanel == null)
                {
                    continue;
                }

                packagePanel.ConfigureRuntimeContext(
                    runtimeInventory,
                    runtimeEquipment,
                    runtimeDatabase,
                    runtimeSelection);
                packagePanel.EnsureReady();
            }

            ToolbarUI[] toolbars = FindObjectsByType<ToolbarUI>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < toolbars.Length; index++)
            {
                ToolbarUI toolbar = toolbars[index];
                if (toolbar == null)
                {
                    continue;
                }

                toolbar.ConfigureRuntimeContext(runtimeInventory, runtimeDatabase, runtimeSelection);
                toolbar.Build();
                toolbar.ForceRefresh();
            }

            InventoryInteractionManager[] interactionManagers = FindObjectsByType<InventoryInteractionManager>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);
            for (int index = 0; index < interactionManagers.Length; index++)
            {
                InventoryInteractionManager interactionManager = interactionManagers[index];
                if (interactionManager == null)
                {
                    continue;
                }

                interactionManager.ConfigureRuntimeContext(
                    runtimeInventory,
                    runtimeEquipment,
                    runtimeDatabase,
                    sortService);
                interactionManager.ClearHeldState();
                interactionManager.HideHeldIcon();
            }

            BoxPanelUI activeBoxPanel = BoxPanelUI.ActiveInstance;
            if (activeBoxPanel != null && activeBoxPanel.IsOpen)
            {
                activeBoxPanel.ConfigureRuntimeContext(
                    runtimeInventory,
                    runtimeEquipment,
                    runtimeDatabase,
                    runtimeSelection);
                activeBoxPanel.RefreshUI();
            }

            Canvas.ForceUpdateCanvases();

            if (showDebugInfo)
                Debug.Log("[SaveManager] UI 已刷新");
        }

        #endregion

    }
}
