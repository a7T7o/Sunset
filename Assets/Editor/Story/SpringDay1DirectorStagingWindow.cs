using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sunset.Story.Editor
{
    public sealed class SpringDay1DirectorStagingWindow : EditorWindow
    {
        [Serializable]
        private sealed class WindowDraftState
        {
            public SpringDay1DirectorStageBook book;
            public int selectedBeatIndex;
            public int selectedCueIndex;
            public float rehearsalSpeed = 2.6f;
            public float recordSampleInterval = 0.18f;
            public float recordMinDistance = 0.08f;
        }

        private readonly struct RecordedCueSample
        {
            public RecordedCueSample(Vector2 position, Vector2 facing)
            {
                Position = position;
                Facing = facing;
            }

            public Vector2 Position { get; }
            public Vector2 Facing { get; }
        }

        private const string DraftEditorPrefKey = "Sunset.SpringDay1.DirectorStagingWindow.Draft";
        private const string CrowdManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";

        private SpringDay1DirectorStageBook _book;
        private bool _hasRecoveredDraft;
        private bool _isDraftDirty;
        private Vector2 _scroll;
        private int _selectedBeatIndex;
        private int _selectedCueIndex;
        private float _rehearsalSpeed = 2.6f;
        private float _recordSampleInterval = 0.18f;
        private float _recordMinDistance = 0.08f;
        private readonly HashSet<KeyCode> _heldKeys = new();
        private readonly List<RecordedCueSample> _recordedSamples = new();
        private int _rehearsalTargetInstanceId;
        private bool _isRecording;
        private double _lastRecordSampleAt;

        [MenuItem("Tools/Story/Spring Day1/导演摆位 MVP")]
        private static void Open()
        {
            GetWindow<SpringDay1DirectorStagingWindow>("Day1 导演摆位");
        }

        private void OnEnable()
        {
            LoadBookOrDraft();
            EditorApplication.update += HandleEditorUpdate;
        }

        private void OnDisable()
        {
            PersistDraftStateIfDirty();
            EditorApplication.update -= HandleEditorUpdate;
            _heldKeys.Clear();
            ResetRecordingState();
        }

        private void OnGUI()
        {
            HandleKeyboard(Event.current);
            DrawToolbar();
            EnsureBook();

            EditorGUI.BeginChangeCheck();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawBeatSection();
            EditorGUILayout.Space(10f);
            DrawCueSection();
            EditorGUILayout.Space(10f);
            DrawPlaybackSection();
            EditorGUILayout.EndScrollView();

            if (EditorGUI.EndChangeCheck())
            {
                MarkDraftDirtyAndPersist();
            }
        }

        private void DrawToolbar()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("重载", EditorStyles.toolbarButton, GUILayout.Width(48f)))
                {
                    ReloadBook();
                }

                if (GUILayout.Button("保存 JSON", EditorStyles.toolbarButton, GUILayout.Width(76f)))
                {
                    SaveBook();
                }

                GUILayout.FlexibleSpace();
                if (_hasRecoveredDraft)
                {
                    EditorGUILayout.LabelField("已恢复草稿", EditorStyles.miniLabel, GUILayout.Width(64f));
                }

                EditorGUILayout.LabelField(SpringDay1DirectorStagingDatabase.AssetPath, EditorStyles.miniLabel);
            }
        }

        private void DrawBeatSection()
        {
            EditorGUILayout.LabelField("Beat", EditorStyles.boldLabel);
            string[] beatKeys = GetBeatKeys();
            _selectedBeatIndex = Mathf.Clamp(_selectedBeatIndex, 0, Mathf.Max(0, beatKeys.Length - 1));
            _selectedBeatIndex = EditorGUILayout.Popup("当前 Beat", _selectedBeatIndex, beatKeys);

            SpringDay1DirectorBeatEntry beat = GetSelectedBeat();
            beat.beatKey = beatKeys[_selectedBeatIndex];
            beat.phaseKey = EditorGUILayout.TextField("所属 Phase", beat.phaseKey);
            beat.sceneLabel = EditorGUILayout.TextField("分场标签", beat.sceneLabel);
            EditorGUILayout.LabelField("正式摘要");
            beat.storySummary = EditorGUILayout.TextArea(beat.storySummary, GUILayout.MinHeight(72f));
            EditorGUILayout.LabelField("资产化钩子");
            beat.assetizationHook = EditorGUILayout.TextArea(beat.assetizationHook, GUILayout.MinHeight(54f));
        }

        private void DrawCueSection()
        {
            SpringDay1DirectorBeatEntry beat = GetSelectedBeat();
            EditorGUILayout.LabelField("Actor Cue", EditorStyles.boldLabel);

            EnsureCueArray(beat);
            string[] cueNames = BuildCueNames(beat);
            _selectedCueIndex = Mathf.Clamp(_selectedCueIndex, 0, Mathf.Max(0, cueNames.Length - 1));

            using (new EditorGUILayout.HorizontalScope())
            {
                _selectedCueIndex = EditorGUILayout.Popup("当前 Cue", _selectedCueIndex, cueNames);
                if (GUILayout.Button("新增 Cue", GUILayout.Width(78f)))
                {
                    AddCue(beat);
                }

                using (new EditorGUI.DisabledScope(beat.actorCues.Length == 0))
                {
                    if (GUILayout.Button("删除 Cue", GUILayout.Width(78f)))
                    {
                        RemoveCue(beat, _selectedCueIndex);
                    }
                }
            }

            if (beat.actorCues.Length == 0)
            {
                EditorGUILayout.HelpBox("当前 beat 还没有 NPC cue。先新增一个，再开始采点和回放。", MessageType.Info);
                return;
            }

            SpringDay1DirectorActorCue cue = beat.actorCues[_selectedCueIndex];
            cue.cueId = EditorGUILayout.TextField("Cue Id", cue.cueId);
            cue.npcId = EditorGUILayout.TextField("NPC Id", cue.npcId);
            cue.semanticAnchorId = EditorGUILayout.TextField("语义锚点", cue.semanticAnchorId);
            cue.duty = (SpringDay1CrowdSceneDuty)EditorGUILayout.EnumPopup("Duty", cue.duty);
            cue.keepCurrentSpawnPosition = EditorGUILayout.Toggle("沿用当前出生位", cue.keepCurrentSpawnPosition);
            cue.useSemanticAnchorAsStart = EditorGUILayout.Toggle("语义锚点作为起点", cue.useSemanticAnchorAsStart);
            using (new EditorGUI.DisabledScope(!cue.useSemanticAnchorAsStart))
            {
                cue.startPositionIsSemanticAnchorOffset = EditorGUILayout.Toggle("起点按锚点偏移", cue.startPositionIsSemanticAnchorOffset);
            }
            cue.pathPointsAreOffsets = EditorGUILayout.Toggle("路径点按偏移量", cue.pathPointsAreOffsets);
            cue.suspendRoam = EditorGUILayout.Toggle("回放时暂停 roam", cue.suspendRoam);
            cue.loopPath = EditorGUILayout.Toggle("循环路径", cue.loopPath);
            cue.moveSpeed = EditorGUILayout.FloatField("回放速度", cue.moveSpeed);
            cue.initialHoldSeconds = EditorGUILayout.FloatField("起始停顿", cue.initialHoldSeconds);
            cue.startPosition = EditorGUILayout.Vector2Field("起点", cue.startPosition);
            cue.facing = EditorGUILayout.Vector2Field("默认朝向", cue.facing);
            cue.lookAtTargetName = EditorGUILayout.TextField("看向目标", cue.lookAtTargetName);

            if (cue.useSemanticAnchorAsStart)
            {
                if (TryResolveSemanticAnchorWorldPosition(cue, out Vector3 anchorWorldPosition))
                {
                    EditorGUILayout.HelpBox($"当前语义锚点已解析到 ({anchorWorldPosition.x:F2}, {anchorWorldPosition.y:F2})。录制写回会优先落成 anchor 相对数据。", MessageType.Info);
                }
                else
                {
                    EditorGUILayout.HelpBox("当前语义锚点还没解析到 live 物体或 Town contract；录制时会先回退成绝对坐标。", MessageType.Warning);
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("选中物体设为起点"))
                {
                    CaptureSelectedAsStart(cue);
                }

                if (GUILayout.Button("追加选中物体为路径点"))
                {
                    AppendSelectedAsPathPoint(cue);
                }

                if (GUILayout.Button("清空路径"))
                {
                    cue.path = Array.Empty<SpringDay1DirectorPathPoint>();
                    MarkDraftDirtyAndPersist();
                }
            }

            DrawPathList(cue);
        }

        private void DrawPathList(SpringDay1DirectorActorCue cue)
        {
            cue.path ??= Array.Empty<SpringDay1DirectorPathPoint>();
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField($"路径点 ({cue.path.Length})", EditorStyles.miniBoldLabel);

            for (int index = 0; index < cue.path.Length; index++)
            {
                SpringDay1DirectorPathPoint point = cue.path[index] ??= new SpringDay1DirectorPathPoint();
                using (new EditorGUILayout.VerticalScope("box"))
                {
                    EditorGUILayout.LabelField($"Point {index + 1}", EditorStyles.miniBoldLabel);
                    point.position = EditorGUILayout.Vector2Field("位置", point.position);
                    point.facing = EditorGUILayout.Vector2Field("朝向", point.facing);
                    point.holdSeconds = EditorGUILayout.FloatField("停顿", point.holdSeconds);
                    point.lookAtTargetName = EditorGUILayout.TextField("看向目标", point.lookAtTargetName);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("用当前选中物体覆盖"))
                        {
                            OverwritePointFromSelection(cue, point);
                        }

                        if (GUILayout.Button("删除这个点"))
                        {
                            RemovePoint(cue, index);
                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }
        }

        private void DrawPlaybackSection()
        {
            EditorGUILayout.LabelField("排练 / 回放", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("播放模式下：给选中的 NPC 挂 `SpringDay1DirectorStagingRehearsalDriver`，就能用 WASD / 方向键排练；Shift 加速。采点后保存 JSON，运行时 crowd 会按 beat 自动接入。现在也可以一键把“当前 beat”整批预演到场景 resident 上。", MessageType.None);

            _rehearsalSpeed = EditorGUILayout.FloatField("排练速度", _rehearsalSpeed);
            _recordSampleInterval = EditorGUILayout.FloatField("录制采样间隔", _recordSampleInterval);
            _recordMinDistance = EditorGUILayout.FloatField("最小采样位移", _recordMinDistance);
            GameObject selectedObject = Selection.activeGameObject;
            GameObject rehearsalTarget = ResolveRehearsalTarget();

            if (rehearsalTarget != null)
            {
                EditorGUILayout.HelpBox($"当前导演已接管：{rehearsalTarget.name}。只要这个窗口保持焦点，WASD / 方向键只会驱动它，不会再把玩家一起带走。", MessageType.Info);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(selectedObject == null))
                {
                    if (GUILayout.Button("开始排练"))
                    {
                        StartRehearsal(selectedObject);
                    }
                }

                using (new EditorGUI.DisabledScope(selectedObject == null && rehearsalTarget == null))
                {
                    if (GUILayout.Button("停止排练"))
                    {
                        StopRehearsal(selectedObject != null ? selectedObject : rehearsalTarget);
                    }

                    if (GUILayout.Button(_isRecording ? "停止录制写回 Cue" : "开始录制 Cue"))
                    {
                        ToggleRecording(rehearsalTarget);
                    }
                }

                using (new EditorGUI.DisabledScope(selectedObject == null))
                {
                    if (GUILayout.Button("把当前 Cue 应到选中 NPC"))
                    {
                        ApplyCueToSelected(selectedObject);
                    }

                    if (GUILayout.Button("清掉选中 NPC 的 Cue"))
                    {
                        ClearCueOnSelected(selectedObject);
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!EditorApplication.isPlaying))
                {
                    if (GUILayout.Button("预演当前 Beat（整批）"))
                    {
                        PreviewCurrentBeatAcrossScene();
                    }

                    if (GUILayout.Button("清理当前 Beat 预演"))
                    {
                        ClearCurrentBeatPreviewAcrossScene();
                    }
                }
            }

            if (selectedObject == null)
            {
                EditorGUILayout.HelpBox("先在 Hierarchy 里选中一个 NPC runtime 物体，再进行排练或手动回放。", MessageType.Info);
            }
            else if (_isRecording)
            {
                EditorGUILayout.HelpBox($"正在录制 {_recordedSamples.Count} 个采样点。停止录制后会把当前轨迹直接写回当前 Cue。", MessageType.Warning);
            }
        }

        private void PreviewCurrentBeatAcrossScene()
        {
            SpringDay1DirectorBeatEntry beat = GetSelectedBeat();
            if (beat == null)
            {
                ShowNotification(new GUIContent("当前 beat 不存在"));
                return;
            }

            SpringDay1NpcCrowdManifest manifest = Resources.Load<SpringDay1NpcCrowdManifest>(CrowdManifestResourcePath);
            if (manifest == null || manifest.Entries == null || manifest.Entries.Length == 0)
            {
                ShowNotification(new GUIContent("未找到 CrowdManifest"));
                return;
            }

            int appliedCount = 0;
            int missingResidentCount = 0;
            for (int index = 0; index < manifest.Entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = manifest.Entries[index];
                if (entry == null
                    || string.IsNullOrWhiteSpace(entry.npcId)
                    || !TryResolveDraftCueForPreview(beat, entry, out SpringDay1DirectorActorCue cue))
                {
                    continue;
                }

                GameObject resident = FindSceneResident(entry.npcId);
                if (resident == null)
                {
                    missingResidentCount++;
                    continue;
                }

                if (!resident.activeSelf)
                {
                    resident.SetActive(true);
                }

                bool cueNormalized = NormalizeCueForSceneWorldPreview(cue);
                SpringDay1DirectorStagingPlayback playback = resident.GetComponent<SpringDay1DirectorStagingPlayback>();
                if (playback == null)
                {
                    playback = resident.AddComponent<SpringDay1DirectorStagingPlayback>();
                }

                playback.ApplyCue(
                    beat.beatKey,
                    cue,
                    FindSceneResidentHomeAnchor(entry.npcId, resident),
                    manualPreviewLock: true,
                    forceRestart: true);
                EditorUtility.SetDirty(resident);
                if (cueNormalized)
                {
                    MarkDraftDirtyAndPersist();
                }
                appliedCount++;
            }

            string message = appliedCount > 0
                ? $"已预演 {beat.beatKey}：{appliedCount} 个 resident"
                : $"当前 beat 没有可预演 resident（缺 resident: {missingResidentCount}）";
            ShowNotification(new GUIContent(message));
            Repaint();
        }

        private bool TryResolveDraftCueForPreview(SpringDay1DirectorBeatEntry beat, SpringDay1NpcCrowdManifest.Entry entry, out SpringDay1DirectorActorCue cue)
        {
            cue = beat?.TryResolveCue(entry);
            if (cue != null)
            {
                return true;
            }

            return false;
        }

        private void ClearCurrentBeatPreviewAcrossScene()
        {
            SpringDay1DirectorBeatEntry beat = GetSelectedBeat();
            string beatKey = beat != null ? beat.beatKey : string.Empty;
            int clearedCount = 0;

            SpringDay1DirectorStagingPlayback[] playbacks = Resources.FindObjectsOfTypeAll<SpringDay1DirectorStagingPlayback>();
            for (int index = 0; index < playbacks.Length; index++)
            {
                SpringDay1DirectorStagingPlayback playback = playbacks[index];
                if (playback == null
                    || playback.gameObject == null
                    || !playback.gameObject.scene.IsValid()
                    || EditorUtility.IsPersistent(playback))
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(beatKey)
                    && !string.Equals(playback.CurrentBeatKey, beatKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                GameObject owner = playback.gameObject;
                playback.ClearCue();
                DestroyImmediate(playback);
                if (owner != null)
                {
                    EditorUtility.SetDirty(owner);
                }
                clearedCount++;
            }

            ShowNotification(new GUIContent(clearedCount > 0 ? $"已清理 {clearedCount} 个预演 cue" : "当前没有可清理的预演 cue"));
            Repaint();
        }

        private void StartRehearsal(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            GameObject currentTarget = ResolveRehearsalTarget();
            if (currentTarget != null && currentTarget != selectedObject)
            {
                StopRehearsal(currentTarget);
            }

            SpringDay1DirectorStagingRehearsalDriver driver = selectedObject.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
            if (driver == null)
            {
                driver = selectedObject.AddComponent<SpringDay1DirectorStagingRehearsalDriver>();
            }

            driver.Configure(_rehearsalSpeed);
            _rehearsalTargetInstanceId = selectedObject.GetInstanceID();
            EditorUtility.SetDirty(selectedObject);
        }

        private void StopRehearsal(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            if (_isRecording)
            {
                CommitRecordingIntoSelectedCue();
            }

            SpringDay1DirectorStagingRehearsalDriver driver = selectedObject.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
            if (driver != null)
            {
                DestroyImmediate(driver);
                EditorUtility.SetDirty(selectedObject);
            }

            if (_rehearsalTargetInstanceId == selectedObject.GetInstanceID())
            {
                _rehearsalTargetInstanceId = 0;
            }

            _heldKeys.Clear();
            ResetRecordingState();
        }

        private void ApplyCueToSelected(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            SpringDay1DirectorActorCue cue = GetSelectedCue();
            if (cue == null)
            {
                return;
            }

            SpringDay1DirectorStagingPlayback playback = selectedObject.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback == null)
            {
                playback = selectedObject.AddComponent<SpringDay1DirectorStagingPlayback>();
            }

            SpringDay1DirectorStagingRehearsalDriver rehearsalDriver = selectedObject.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
            if (rehearsalDriver != null)
            {
                StopRehearsal(selectedObject);
            }

            bool cueNormalized = NormalizeCueForSceneWorldPreview(cue);
            playback.ApplyCue(
                GetSelectedBeat().beatKey,
                cue,
                FindSceneResidentHomeAnchor(selectedObject.name, selectedObject),
                manualPreviewLock: true,
                forceRestart: true);
            if (cueNormalized)
            {
                MarkDraftDirtyAndPersist();
            }
            EditorUtility.SetDirty(selectedObject);
        }

        private static void ClearCueOnSelected(GameObject selectedObject)
        {
            if (selectedObject == null)
            {
                return;
            }

            SpringDay1DirectorStagingPlayback playback = selectedObject.GetComponent<SpringDay1DirectorStagingPlayback>();
            if (playback != null)
            {
                playback.ClearCue();
                DestroyImmediate(playback);
                EditorUtility.SetDirty(selectedObject);
            }
        }

        private void CaptureSelectedAsStart(SpringDay1DirectorActorCue cue)
        {
            if (cue == null || Selection.activeTransform == null)
            {
                return;
            }

            NormalizeCueToSceneWorldPoints(cue);
            Vector3 position = Selection.activeTransform.position;
            cue.startPosition = EncodeCueStartPosition(cue, position);
            MarkDraftDirtyAndPersist();
            Repaint();
        }

        private void AppendSelectedAsPathPoint(SpringDay1DirectorActorCue cue)
        {
            if (cue == null || Selection.activeTransform == null)
            {
                return;
            }

            NormalizeCueToSceneWorldPoints(cue);
            Vector3 position = Selection.activeTransform.position;
            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>(cue.path ?? Array.Empty<SpringDay1DirectorPathPoint>())
            {
                new SpringDay1DirectorPathPoint
                {
                    position = EncodePathPointPosition(cue, position),
                    facing = cue.facing,
                    holdSeconds = 0.25f
                }
            };
            cue.path = points.ToArray();
            MarkDraftDirtyAndPersist();
            Repaint();
        }

        private void OverwritePointFromSelection(SpringDay1DirectorActorCue cue, SpringDay1DirectorPathPoint point)
        {
            if (cue == null || point == null || Selection.activeTransform == null)
            {
                return;
            }

            NormalizeCueToSceneWorldPoints(cue);
            Vector3 position = Selection.activeTransform.position;
            point.position = EncodePathPointPosition(cue, position);
            MarkDraftDirtyAndPersist();
        }

        private static void NormalizeCueToSceneWorldPoints(SpringDay1DirectorActorCue cue)
        {
            if (cue == null)
            {
                return;
            }

            cue.keepCurrentSpawnPosition = false;
            cue.useSemanticAnchorAsStart = false;
            cue.startPositionIsSemanticAnchorOffset = false;
            cue.pathPointsAreOffsets = false;
        }

        private static bool NormalizeCueForSceneWorldPreview(SpringDay1DirectorActorCue cue)
        {
            if (cue == null
                || !cue.keepCurrentSpawnPosition
                || !cue.useSemanticAnchorAsStart
                || !cue.startPositionIsSemanticAnchorOffset
                || cue.pathPointsAreOffsets
                || cue.path == null
                || cue.path.Length == 0
                || cue.startPosition.sqrMagnitude <= 0.0001f)
            {
                return false;
            }

            NormalizeCueToSceneWorldPoints(cue);
            return true;
        }

        private void RemovePoint(SpringDay1DirectorActorCue cue, int index)
        {
            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>(cue.path ?? Array.Empty<SpringDay1DirectorPathPoint>());
            if (index < 0 || index >= points.Count)
            {
                return;
            }

            points.RemoveAt(index);
            cue.path = points.ToArray();
            MarkDraftDirtyAndPersist();
        }

        private void AddCue(SpringDay1DirectorBeatEntry beat)
        {
            List<SpringDay1DirectorActorCue> cues = new List<SpringDay1DirectorActorCue>(beat.actorCues ?? Array.Empty<SpringDay1DirectorActorCue>())
            {
                new SpringDay1DirectorActorCue
                {
                    cueId = $"cue-{Mathf.Max(1, (beat.actorCues?.Length ?? 0) + 1)}"
                }
            };
            beat.actorCues = cues.ToArray();
            _selectedCueIndex = beat.actorCues.Length - 1;
            MarkDraftDirtyAndPersist();
        }

        private void RemoveCue(SpringDay1DirectorBeatEntry beat, int index)
        {
            List<SpringDay1DirectorActorCue> cues = new List<SpringDay1DirectorActorCue>(beat.actorCues ?? Array.Empty<SpringDay1DirectorActorCue>());
            if (index < 0 || index >= cues.Count)
            {
                return;
            }

            cues.RemoveAt(index);
            beat.actorCues = cues.ToArray();
            _selectedCueIndex = Mathf.Clamp(_selectedCueIndex, 0, Mathf.Max(0, beat.actorCues.Length - 1));
            MarkDraftDirtyAndPersist();
        }

        private SpringDay1DirectorBeatEntry GetSelectedBeat()
        {
            EnsureBook();
            string beatKey = GetBeatKeys()[_selectedBeatIndex];
            SpringDay1DirectorBeatEntry beat = _book.FindBeat(beatKey);
            if (beat != null)
            {
                return beat;
            }

            List<SpringDay1DirectorBeatEntry> beats = new List<SpringDay1DirectorBeatEntry>(_book.beats ?? Array.Empty<SpringDay1DirectorBeatEntry>())
            {
                new SpringDay1DirectorBeatEntry
                {
                    beatKey = beatKey,
                    phaseKey = string.Empty,
                    sceneLabel = beatKey,
                    storySummary = string.Empty,
                    assetizationHook = string.Empty
                }
            };
            _book.beats = beats.ToArray();
            return beats[beats.Count - 1];
        }

        private SpringDay1DirectorActorCue GetSelectedCue()
        {
            SpringDay1DirectorBeatEntry beat = GetSelectedBeat();
            EnsureCueArray(beat);
            if (beat.actorCues.Length == 0)
            {
                return null;
            }

            _selectedCueIndex = Mathf.Clamp(_selectedCueIndex, 0, beat.actorCues.Length - 1);
            return beat.actorCues[_selectedCueIndex];
        }

        private void EnsureBook()
        {
            _book ??= SpringDay1DirectorStagingDatabase.Load(forceReload: false) ?? SpringDay1DirectorStageBook.CreateEmpty();
            _book.EnsureDefaults();
        }

        private void EnsureCueArray(SpringDay1DirectorBeatEntry beat)
        {
            beat.actorCues ??= Array.Empty<SpringDay1DirectorActorCue>();
            for (int index = 0; index < beat.actorCues.Length; index++)
            {
                beat.actorCues[index] ??= new SpringDay1DirectorActorCue();
                beat.actorCues[index].EnsureDefaults();
            }
        }

        private void ReloadBook()
        {
            _book = SpringDay1DirectorStagingDatabase.Load(forceReload: true) ?? SpringDay1DirectorStageBook.CreateEmpty();
            _book.EnsureDefaults();
            _hasRecoveredDraft = false;
            _isDraftDirty = false;
            _selectedBeatIndex = 0;
            _selectedCueIndex = 0;
            DeleteDraftState();
            Repaint();
        }

        private void SaveBook()
        {
            EnsureBook();
            SpringDay1DirectorStagingDatabase.Save(_book);
            _hasRecoveredDraft = false;
            _isDraftDirty = false;
            DeleteDraftState();
            ShowNotification(new GUIContent("导演 JSON 已保存"));
        }

        private void LoadBookOrDraft()
        {
            if (TryRestoreDraftState())
            {
                _hasRecoveredDraft = true;
                _isDraftDirty = true;
                Repaint();
                return;
            }

            _book = SpringDay1DirectorStagingDatabase.Load(forceReload: true) ?? SpringDay1DirectorStageBook.CreateEmpty();
            _book.EnsureDefaults();
            _hasRecoveredDraft = false;
            _isDraftDirty = false;
            _selectedBeatIndex = 0;
            _selectedCueIndex = 0;
            Repaint();
        }

        private bool TryRestoreDraftState()
        {
            if (!EditorPrefs.HasKey(DraftEditorPrefKey))
            {
                return false;
            }

            string raw = EditorPrefs.GetString(DraftEditorPrefKey, string.Empty);
            if (string.IsNullOrWhiteSpace(raw))
            {
                return false;
            }

            WindowDraftState draft;
            try
            {
                draft = JsonUtility.FromJson<WindowDraftState>(raw);
            }
            catch (Exception)
            {
                DeleteDraftState();
                return false;
            }

            if (draft?.book == null)
            {
                DeleteDraftState();
                return false;
            }

            _book = draft.book;
            _book.EnsureDefaults();
            _selectedBeatIndex = Mathf.Max(0, draft.selectedBeatIndex);
            _selectedCueIndex = Mathf.Max(0, draft.selectedCueIndex);
            _rehearsalSpeed = draft.rehearsalSpeed > 0f ? draft.rehearsalSpeed : 2.6f;
            _recordSampleInterval = draft.recordSampleInterval > 0f ? draft.recordSampleInterval : 0.18f;
            _recordMinDistance = draft.recordMinDistance > 0f ? draft.recordMinDistance : 0.08f;
            return true;
        }

        private void MarkDraftDirtyAndPersist()
        {
            _isDraftDirty = true;
            PersistDraftStateIfDirty();
        }

        private void PersistDraftStateIfDirty()
        {
            if (!_isDraftDirty)
            {
                return;
            }

            PersistDraftState();
        }

        private void PersistDraftState()
        {
            if (_book == null)
            {
                return;
            }

            WindowDraftState draft = new WindowDraftState
            {
                book = _book,
                selectedBeatIndex = _selectedBeatIndex,
                selectedCueIndex = _selectedCueIndex,
                rehearsalSpeed = _rehearsalSpeed,
                recordSampleInterval = _recordSampleInterval,
                recordMinDistance = _recordMinDistance
            };

            EditorPrefs.SetString(DraftEditorPrefKey, JsonUtility.ToJson(draft));
        }

        private static void DeleteDraftState()
        {
            if (EditorPrefs.HasKey(DraftEditorPrefKey))
            {
                EditorPrefs.DeleteKey(DraftEditorPrefKey);
            }
        }

        private string[] GetBeatKeys()
        {
            List<string> keys = new List<string>();
            foreach (string knownKey in SpringDay1DirectorBeatKeys.OrderedKeys)
            {
                keys.Add(knownKey);
            }

            if (_book?.beats != null)
            {
                for (int index = 0; index < _book.beats.Length; index++)
                {
                    string beatKey = _book.beats[index]?.beatKey;
                    if (!string.IsNullOrWhiteSpace(beatKey) && !keys.Contains(beatKey))
                    {
                        keys.Add(beatKey);
                    }
                }
            }

            return keys.ToArray();
        }

        private static string[] BuildCueNames(SpringDay1DirectorBeatEntry beat)
        {
            if (beat.actorCues == null || beat.actorCues.Length == 0)
            {
                return Array.Empty<string>();
            }

            string[] names = new string[beat.actorCues.Length];
            for (int index = 0; index < beat.actorCues.Length; index++)
            {
                SpringDay1DirectorActorCue cue = beat.actorCues[index];
                string core = cue != null && !string.IsNullOrWhiteSpace(cue.StableKey) ? cue.StableKey : $"Cue {index + 1}";
                names[index] = core;
            }

            return names;
        }

        private static GameObject FindSceneResident(string npcId)
        {
            if (string.IsNullOrWhiteSpace(npcId))
            {
                return null;
            }

            foreach (string candidateName in EnumerateResidentLookupNames(npcId))
            {
                GameObject resident = FindSceneObject(candidateName);
                if (resident != null)
                {
                    return resident;
                }
            }

            return null;
        }

        private static Transform FindSceneResidentHomeAnchor(string npcId, GameObject resident)
        {
            foreach (string candidateName in EnumerateResidentLookupNames(npcId))
            {
                foreach (string anchorName in EnumerateHomeAnchorNames(candidateName))
                {
                    GameObject anchorObject = FindSceneObject(anchorName);
                    if (anchorObject != null)
                    {
                        return anchorObject.transform;
                    }
                }
            }

            if (resident != null)
            {
                Transform nestedAnchor = FindChildRecursive(resident.transform, "HomeAnchor");
                if (nestedAnchor != null)
                {
                    return nestedAnchor;
                }
            }

            return null;
        }

        private static GameObject FindSceneObject(string objectName)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                return null;
            }

            GameObject[] sceneObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int index = 0; index < sceneObjects.Length; index++)
            {
                GameObject candidate = sceneObjects[index];
                if (candidate == null
                    || !candidate.scene.IsValid()
                    || EditorUtility.IsPersistent(candidate))
                {
                    continue;
                }

                if (string.Equals(candidate.name, objectName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return candidate;
                }
            }

            return null;
        }

        private static IEnumerable<string> EnumerateResidentLookupNames(string npcId)
        {
            string trimmed = npcId?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                yield break;
            }

            yield return trimmed;
            if (int.TryParse(trimmed, out int numericId))
            {
                yield return numericId.ToString("000");
            }
        }

        private static IEnumerable<string> EnumerateHomeAnchorNames(string candidateName)
        {
            string trimmed = candidateName?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                yield break;
            }

            yield return $"{trimmed}_HomeAnchor";

            string alias = TryBuildHomeAnchorAlias(trimmed);
            if (!string.IsNullOrWhiteSpace(alias) && !string.Equals(alias, $"{trimmed}_HomeAnchor", StringComparison.OrdinalIgnoreCase))
            {
                yield return alias;
            }
        }

        private static string TryBuildHomeAnchorAlias(string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                return string.Empty;
            }

            string trimmed = candidateName.Trim();
            if (trimmed.EndsWith("_HomeAnchor", StringComparison.OrdinalIgnoreCase))
            {
                return trimmed;
            }

            if (!int.TryParse(trimmed, out int numericId))
            {
                return string.Empty;
            }

            return $"{numericId:000}_HomeAnchor";
        }

        private static Transform FindChildRecursive(Transform root, string childName)
        {
            if (root == null)
            {
                return null;
            }

            for (int index = 0; index < root.childCount; index++)
            {
                Transform child = root.GetChild(index);
                if (string.Equals(child.name, childName, StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }

                Transform nested = FindChildRecursive(child, childName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private void HandleEditorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                _heldKeys.Clear();
                ResetRecordingState();
                return;
            }

            GameObject targetObject = ResolveRehearsalTarget();
            if (targetObject == null)
            {
                return;
            }

            SpringDay1DirectorStagingRehearsalDriver driver = targetObject.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
            if (driver == null)
            {
                _rehearsalTargetInstanceId = 0;
                _heldKeys.Clear();
                return;
            }

            Vector2 input = BuildHeldInput();
            bool sprintHeld = _heldKeys.Contains(KeyCode.LeftShift) || _heldKeys.Contains(KeyCode.RightShift);
            driver.SetRehearsalInput(input, sprintHeld);

            if (hasFocus && input.sqrMagnitude > 0.0001f)
            {
                Repaint();
            }

            if (_isRecording)
            {
                TryRecordCueSample(targetObject);
            }
        }

        private void HandleKeyboard(Event currentEvent)
        {
            if (!hasFocus || currentEvent == null)
            {
                return;
            }

            if (!IsTrackedRehearsalKey(currentEvent.keyCode))
            {
                return;
            }

            if (currentEvent.type == EventType.KeyDown)
            {
                _heldKeys.Add(currentEvent.keyCode);
                currentEvent.Use();
            }
            else if (currentEvent.type == EventType.KeyUp)
            {
                _heldKeys.Remove(currentEvent.keyCode);
                currentEvent.Use();
            }
        }

        private GameObject ResolveRehearsalTarget()
        {
            if (_rehearsalTargetInstanceId == 0)
            {
                return null;
            }

            UnityEngine.Object target = EditorUtility.InstanceIDToObject(_rehearsalTargetInstanceId);
            return target as GameObject;
        }

        private static bool IsTrackedRehearsalKey(KeyCode keyCode)
        {
            return keyCode == KeyCode.W
                || keyCode == KeyCode.A
                || keyCode == KeyCode.S
                || keyCode == KeyCode.D
                || keyCode == KeyCode.UpArrow
                || keyCode == KeyCode.DownArrow
                || keyCode == KeyCode.LeftArrow
                || keyCode == KeyCode.RightArrow
                || keyCode == KeyCode.LeftShift
                || keyCode == KeyCode.RightShift;
        }

        private Vector2 BuildHeldInput()
        {
            Vector2 input = Vector2.zero;
            if (_heldKeys.Contains(KeyCode.W) || _heldKeys.Contains(KeyCode.UpArrow))
            {
                input.y += 1f;
            }

            if (_heldKeys.Contains(KeyCode.S) || _heldKeys.Contains(KeyCode.DownArrow))
            {
                input.y -= 1f;
            }

            if (_heldKeys.Contains(KeyCode.A) || _heldKeys.Contains(KeyCode.LeftArrow))
            {
                input.x -= 1f;
            }

            if (_heldKeys.Contains(KeyCode.D) || _heldKeys.Contains(KeyCode.RightArrow))
            {
                input.x += 1f;
            }

            return input;
        }

        private void ToggleRecording(GameObject rehearsalTarget)
        {
            if (_isRecording)
            {
                CommitRecordingIntoSelectedCue();
                return;
            }

            if (rehearsalTarget == null)
            {
                ShowNotification(new GUIContent("先开始排练，再录制"));
                return;
            }

            _recordedSamples.Clear();
            _isRecording = true;
            _lastRecordSampleAt = 0d;
            TryRecordCueSample(rehearsalTarget, force: true);
            ShowNotification(new GUIContent("已开始录制当前 Cue"));
        }

        private void CommitRecordingIntoSelectedCue()
        {
            SpringDay1DirectorActorCue cue = GetSelectedCue();
            if (cue == null || _recordedSamples.Count == 0)
            {
                ResetRecordingState();
                return;
            }

            cue.keepCurrentSpawnPosition = false;
            cue.facing = _recordedSamples[0].Facing;

            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>();
            if (cue.useSemanticAnchorAsStart && TryResolveSemanticAnchorWorldPosition(cue, out Vector3 anchorWorldPosition))
            {
                cue.startPositionIsSemanticAnchorOffset = true;
                cue.pathPointsAreOffsets = true;
                cue.startPosition = _recordedSamples[0].Position - new Vector2(anchorWorldPosition.x, anchorWorldPosition.y);

                for (int index = 1; index < _recordedSamples.Count; index++)
                {
                    RecordedCueSample sample = _recordedSamples[index];
                    points.Add(new SpringDay1DirectorPathPoint
                    {
                        position = sample.Position - _recordedSamples[0].Position,
                        facing = sample.Facing,
                        holdSeconds = 0.08f,
                        lookAtTargetName = cue.lookAtTargetName
                    });
                }
            }
            else
            {
                cue.startPositionIsSemanticAnchorOffset = false;
                cue.pathPointsAreOffsets = false;
                cue.startPosition = _recordedSamples[0].Position;

                for (int index = 1; index < _recordedSamples.Count; index++)
                {
                    RecordedCueSample sample = _recordedSamples[index];
                    points.Add(new SpringDay1DirectorPathPoint
                    {
                        position = sample.Position,
                        facing = sample.Facing,
                        holdSeconds = 0.08f,
                        lookAtTargetName = cue.lookAtTargetName
                    });
                }
            }

            cue.path = points.ToArray();
            ResetRecordingState();
            MarkDraftDirtyAndPersist();
            Repaint();
            ShowNotification(new GUIContent("录制轨迹已写回当前 Cue"));
        }

        private void ResetRecordingState()
        {
            _isRecording = false;
            _lastRecordSampleAt = 0d;
            _recordedSamples.Clear();
        }

        private void TryRecordCueSample(GameObject targetObject, bool force = false)
        {
            if (targetObject == null)
            {
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (!force && now - _lastRecordSampleAt < Mathf.Max(0.03f, _recordSampleInterval))
            {
                return;
            }

            Vector2 position = targetObject.transform.position;
            Vector2 facing = ResolveFacing(targetObject);
            if (!force && _recordedSamples.Count > 0)
            {
                RecordedCueSample last = _recordedSamples[_recordedSamples.Count - 1];
                bool movedEnough = Vector2.Distance(last.Position, position) >= Mathf.Max(0.02f, _recordMinDistance);
                bool facingChanged = Vector2.Distance(last.Facing, facing) >= 0.2f;
                if (!movedEnough && !facingChanged)
                {
                    return;
                }
            }

            _recordedSamples.Add(new RecordedCueSample(position, facing));
            _lastRecordSampleAt = now;
        }

        private static Vector2 ResolveFacing(GameObject targetObject)
        {
            if (targetObject == null)
            {
                return Vector2.down;
            }

            NPCAnimController animController = targetObject.GetComponent<NPCAnimController>();
            if (animController == null)
            {
                return Vector2.down;
            }

            return animController.CurrentDirection switch
            {
                NPCAnimController.NPCAnimDirection.Up => Vector2.up,
                NPCAnimController.NPCAnimDirection.Right => Vector2.right,
                NPCAnimController.NPCAnimDirection.Left => Vector2.left,
                _ => Vector2.down
            };
        }

        private static Vector2 EncodeCueStartPosition(SpringDay1DirectorActorCue cue, Vector3 worldPosition)
        {
            if (cue != null
                && cue.useSemanticAnchorAsStart
                && cue.startPositionIsSemanticAnchorOffset
                && TryResolveSemanticAnchorWorldPosition(cue, out Vector3 anchorWorldPosition))
            {
                return new Vector2(worldPosition.x - anchorWorldPosition.x, worldPosition.y - anchorWorldPosition.y);
            }

            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private static Vector2 EncodePathPointPosition(SpringDay1DirectorActorCue cue, Vector3 worldPosition)
        {
            if (cue != null
                && cue.pathPointsAreOffsets
                && TryResolveCueStartWorldPosition(cue, out Vector3 cueStartWorldPosition))
            {
                return new Vector2(worldPosition.x - cueStartWorldPosition.x, worldPosition.y - cueStartWorldPosition.y);
            }

            return new Vector2(worldPosition.x, worldPosition.y);
        }

        private static bool TryResolveCueStartWorldPosition(SpringDay1DirectorActorCue cue, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            if (cue == null)
            {
                return false;
            }

            if (cue.useSemanticAnchorAsStart
                && TryResolveSemanticAnchorWorldPosition(cue, out Vector3 anchorWorldPosition))
            {
                Vector2 start2D = cue.startPositionIsSemanticAnchorOffset
                    ? new Vector2(anchorWorldPosition.x, anchorWorldPosition.y) + cue.startPosition
                    : cue.startPosition;
                worldPosition = new Vector3(start2D.x, start2D.y, 0f);
                return true;
            }

            worldPosition = new Vector3(cue.startPosition.x, cue.startPosition.y, 0f);
            return true;
        }

        private static bool TryResolveSemanticAnchorWorldPosition(SpringDay1DirectorActorCue cue, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            return cue != null
                && cue.useSemanticAnchorAsStart
                && SpringDay1DirectorSemanticAnchorResolver.TryResolveWorldPosition(cue.semanticAnchorId, out worldPosition);
        }
    }
}
