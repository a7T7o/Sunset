using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sunset.Story.Editor
{
    public sealed class SpringDay1DirectorStagingWindow : EditorWindow
    {
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

        private SpringDay1DirectorStageBook _book;
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
            ReloadBook();
            EditorApplication.update += HandleEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= HandleEditorUpdate;
            _heldKeys.Clear();
            ResetRecordingState();
        }

        private void OnGUI()
        {
            HandleKeyboard(Event.current);
            DrawToolbar();
            EnsureBook();

            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            DrawBeatSection();
            EditorGUILayout.Space(10f);
            DrawCueSection();
            EditorGUILayout.Space(10f);
            DrawPlaybackSection();
            EditorGUILayout.EndScrollView();
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
            cue.pathPointsAreOffsets = EditorGUILayout.Toggle("路径点按偏移量", cue.pathPointsAreOffsets);
            cue.suspendRoam = EditorGUILayout.Toggle("回放时暂停 roam", cue.suspendRoam);
            cue.loopPath = EditorGUILayout.Toggle("循环路径", cue.loopPath);
            cue.moveSpeed = EditorGUILayout.FloatField("回放速度", cue.moveSpeed);
            cue.initialHoldSeconds = EditorGUILayout.FloatField("起始停顿", cue.initialHoldSeconds);
            cue.startPosition = EditorGUILayout.Vector2Field("起点", cue.startPosition);
            cue.facing = EditorGUILayout.Vector2Field("默认朝向", cue.facing);
            cue.lookAtTargetName = EditorGUILayout.TextField("看向目标", cue.lookAtTargetName);

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
                            OverwritePointFromSelection(point);
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
            EditorGUILayout.HelpBox("播放模式下：给选中的 NPC 挂 `SpringDay1DirectorStagingRehearsalDriver`，就能用 WASD / 方向键排练；Shift 加速。采点后保存 JSON，运行时 crowd 会按 beat 自动接入。", MessageType.None);

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

            if (selectedObject == null)
            {
                EditorGUILayout.HelpBox("先在 Hierarchy 里选中一个 NPC runtime 物体，再进行排练或手动回放。", MessageType.Info);
            }
            else if (_isRecording)
            {
                EditorGUILayout.HelpBox($"正在录制 {_recordedSamples.Count} 个采样点。停止录制后会把当前轨迹直接写回当前 Cue。", MessageType.Warning);
            }
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

            playback.ApplyCue(GetSelectedBeat().beatKey, cue, null);
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

            Vector3 position = Selection.activeTransform.position;
            cue.startPosition = new Vector2(position.x, position.y);
            Repaint();
        }

        private void AppendSelectedAsPathPoint(SpringDay1DirectorActorCue cue)
        {
            if (cue == null || Selection.activeTransform == null)
            {
                return;
            }

            Vector3 position = Selection.activeTransform.position;
            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>(cue.path ?? Array.Empty<SpringDay1DirectorPathPoint>())
            {
                new SpringDay1DirectorPathPoint
                {
                    position = new Vector2(position.x, position.y),
                    facing = cue.facing,
                    holdSeconds = 0.25f
                }
            };
            cue.path = points.ToArray();
            Repaint();
        }

        private static void OverwritePointFromSelection(SpringDay1DirectorPathPoint point)
        {
            if (point == null || Selection.activeTransform == null)
            {
                return;
            }

            Vector3 position = Selection.activeTransform.position;
            point.position = new Vector2(position.x, position.y);
        }

        private static void RemovePoint(SpringDay1DirectorActorCue cue, int index)
        {
            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>(cue.path ?? Array.Empty<SpringDay1DirectorPathPoint>());
            if (index < 0 || index >= points.Count)
            {
                return;
            }

            points.RemoveAt(index);
            cue.path = points.ToArray();
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
            _selectedBeatIndex = 0;
            _selectedCueIndex = 0;
            Repaint();
        }

        private void SaveBook()
        {
            EnsureBook();
            SpringDay1DirectorStagingDatabase.Save(_book);
            ShowNotification(new GUIContent("导演 JSON 已保存"));
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
            cue.pathPointsAreOffsets = false;
            cue.startPosition = _recordedSamples[0].Position;
            cue.facing = _recordedSamples[0].Facing;

            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>();
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

            cue.path = points.ToArray();
            ResetRecordingState();
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
    }
}
