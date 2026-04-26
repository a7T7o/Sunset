#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sunset.Story;

namespace Sunset.Editor.Story
{
    [InitializeOnLoad]
    internal static class SpringDay1DirectorPrimaryRehearsalBakeMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Run Director Primary Rehearsal Bake";
        private const string FallbackMenuPath = "Sunset/Story/Validation/Run Director Primary Rehearsal Bake (Edit Fallback)";
        private const string ManifestResourcePath = "Story/SpringDay1/SpringDay1NpcCrowdManifest";
        private const float RecordInterval = 0.06f;
        private const float RecordMinDistance = 0.045f;
        private const string QueuedSessionKey = "Sunset.SpringDay1DirectorPrimaryRehearsalBake.Queued";

        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "spring-day1-director-primary-rehearsal-bake.json");

        private static readonly BakeTargetSpec[] Targets =
        {
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.EnterVillagePostEntry, "enter-crowd-101", "101", 1.55f, new[]
            {
                new BakeStep(new Vector2(-0.65f, 0.25f), 0.28f, false, 0.16f),
                new BakeStep(new Vector2(0.45f, 0.15f), 0.18f, false, 0.12f),
                new BakeStep(Vector2.zero, 0.22f, false, 0.24f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.EnterVillagePostEntry, "enter-kid-103", "103", 1.7f, new[]
            {
                new BakeStep(new Vector2(0.55f, 0.3f), 0.22f, true, 0.1f),
                new BakeStep(new Vector2(-0.25f, 0.2f), 0.18f, false, 0.18f),
                new BakeStep(Vector2.zero, 0.24f, false, 0.26f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.FreeTimeNightWitness, "night-witness-102", "102", 1.45f, new[]
            {
                new BakeStep(new Vector2(-0.35f, -0.15f), 0.28f, false, 0.18f),
                new BakeStep(new Vector2(0.2f, 0.1f), 0.16f, false, 0.1f),
                new BakeStep(Vector2.zero, 0.3f, false, 0.36f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-203", "203", 1.2f, new[]
            {
                new BakeStep(new Vector2(0.6f, -0.1f), 0.2f, false, 0.14f),
                new BakeStep(new Vector2(-0.45f, 0.2f), 0.18f, false, 0.18f),
                new BakeStep(new Vector2(0.2f, 0.1f), 0.14f, false, 0.12f),
                new BakeStep(Vector2.zero, 0.24f, false, 0.32f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-104", "104", 1.15f, new[]
            {
                new BakeStep(new Vector2(-0.45f, 0.05f), 0.18f, false, 0.16f),
                new BakeStep(new Vector2(0.35f, -0.1f), 0.16f, false, 0.14f),
                new BakeStep(new Vector2(-0.2f, 0.2f), 0.14f, false, 0.1f),
                new BakeStep(Vector2.zero, 0.22f, false, 0.3f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-201", "201", 1.05f, new[]
            {
                new BakeStep(new Vector2(0.35f, -0.25f), 0.18f, false, 0.16f),
                new BakeStep(new Vector2(-0.25f, 0.15f), 0.18f, false, 0.12f),
                new BakeStep(new Vector2(0.18f, 0.08f), 0.12f, false, 0.1f),
                new BakeStep(Vector2.zero, 0.24f, false, 0.34f)
            }),
            new BakeTargetSpec(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-202", "202", 1.08f, new[]
            {
                new BakeStep(new Vector2(0.55f, -0.3f), 0.22f, false, 0.14f),
                new BakeStep(new Vector2(-0.35f, 0.18f), 0.18f, false, 0.16f),
                new BakeStep(new Vector2(0.22f, 0.06f), 0.12f, false, 0.12f),
                new BakeStep(Vector2.zero, 0.24f, false, 0.34f)
            })
        };

        private static bool _queued;
        private static bool _running;
        private static bool _stopRequested;
        private static SpringDay1DirectorStageBook _book;
        private static SpringDay1NpcCrowdManifest _manifest;
        private static int _targetIndex;
        private static BakeTargetSpec _currentTarget;
        private static SpringDay1DirectorActorCue _currentCue;
        private static GameObject _currentNpc;
        private static SpringDay1DirectorStagingRehearsalDriver _currentDriver;
        private static int _currentStepIndex;
        private static double _currentStepStartedAt;
        private static double _lastRecordAt;
        private static double _lastDriveAt;
        private static double _queuedAt;
        private static readonly List<RecordedCueSample> RecordedSamples = new List<RecordedCueSample>();
        private static readonly List<BakeTargetRunResult> CompletedTargets = new List<BakeTargetRunResult>();

        static SpringDay1DirectorPrimaryRehearsalBakeMenu()
        {
            _queued = SessionState.GetBool(QueuedSessionKey, false);
            EditorApplication.update -= Tick;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
            EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            RunFromBridge();
        }

        [MenuItem(FallbackMenuPath)]
        private static void RunFallback()
        {
            ResetSessionState();
            RunEditModeFallbackBake();
        }

        internal static void RunFromBridge()
        {
            Directory.CreateDirectory(CommandRoot);
            if ((_queued || _running) && !EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                ResetSessionState();
            }

            if (_queued || _running || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                WriteReport(new BakeReport
                {
                    timestamp = DateTime.Now.ToString("O"),
                    status = "blocked",
                    success = false,
                    firstBlocker = "session-busy",
                    message = "导演 bake 已在运行或编辑器正在切换 Play Mode。",
                    activeScene = SceneManager.GetActiveScene().path,
                    targets = CompletedTargets.ToArray()
                });
                return;
            }

            ResetSessionState();
            _queued = true;
            SessionState.SetBool(QueuedSessionKey, true);
            _queuedAt = EditorApplication.timeSinceStartup;
            WriteReport(new BakeReport
            {
                timestamp = DateTime.Now.ToString("O"),
                status = "queued",
                success = true,
                message = $"导演 bake 已排队，将在 Play Mode 下排练并写回 {Targets.Length} 条 cue。",
                activeScene = SceneManager.GetActiveScene().path,
                targets = Array.Empty<BakeTargetRunResult>()
            });

            EditorApplication.delayCall -= EnterPlayModeFromQueue;
            EditorApplication.delayCall += EnterPlayModeFromQueue;
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(FallbackMenuPath, true)]
        private static bool ValidateRunFallback()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static void HandlePlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                TryStartSession();
                return;
            }

            if (state == PlayModeStateChange.EnteredEditMode && _stopRequested)
            {
                ResetSessionState();
            }
        }

        private static void EnterPlayModeFromQueue()
        {
            EditorApplication.delayCall -= EnterPlayModeFromQueue;
            if (!_queued || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.isPlaying = true;
        }

        private static void Tick()
        {
            if (!_queued && SessionState.GetBool(QueuedSessionKey, false))
            {
                _queued = true;
            }

            if (_queued
                && !EditorApplication.isPlaying
                && !EditorApplication.isPlayingOrWillChangePlaymode
                && EditorApplication.timeSinceStartup - _queuedAt > 1.2d)
            {
                RunEditModeFallbackBake();
                return;
            }

            if (_queued && EditorApplication.isPlaying && !_running)
            {
                TryStartSession();
            }

            if (!_running || !EditorApplication.isPlaying)
            {
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (_currentNpc == null)
            {
                if (!PrepareNextTarget(now))
                {
                    FinishSuccess();
                }

                return;
            }

            UpdateCurrentTarget(now);
        }

        private static void TryStartSession()
        {
            if (!_queued || _running || !EditorApplication.isPlaying)
            {
                return;
            }

            _book = SpringDay1DirectorStagingDatabase.Load(forceReload: true);
            _manifest = Resources.Load<SpringDay1NpcCrowdManifest>(ManifestResourcePath);
            if (_book == null)
            {
                Fail("stage-book-missing", "未能加载导演 StageBook，无法继续 rehearsal bake。");
                return;
            }

            if (_manifest == null)
            {
                Fail("manifest-missing", "未能加载 SpringDay1NpcCrowdManifest，无法继续 rehearsal bake。");
                return;
            }

            _queued = false;
            SessionState.SetBool(QueuedSessionKey, false);
            _running = true;
            _targetIndex = 0;
            WriteReport(new BakeReport
            {
                timestamp = DateTime.Now.ToString("O"),
                status = "running",
                success = true,
                message = "导演 rehearsal bake 已进入 Play Mode，开始逐条排练并写回 cue。",
                activeScene = SceneManager.GetActiveScene().path,
                targets = CompletedTargets.ToArray()
            });
        }

        private static bool PrepareNextTarget(double now)
        {
            CleanupCurrentRuntime();
            while (_targetIndex < Targets.Length)
            {
                BakeTargetSpec nextTarget = Targets[_targetIndex++];
                SpringDay1DirectorBeatEntry beat = _book.FindBeat(nextTarget.beatKey);
                SpringDay1DirectorActorCue cue = FindCueById(beat, nextTarget.cueId, nextTarget.npcId);
                SpringDay1NpcCrowdManifest.Entry entry = FindEntryByNpcId(_manifest, nextTarget.npcId);
                if (beat == null || cue == null || entry == null || entry.prefab == null)
                {
                    Fail("target-missing", $"导演 bake 缺少目标定义：beat={nextTarget.beatKey}, cue={nextTarget.cueId}, npcId={nextTarget.npcId}。");
                    return false;
                }

                cue.EnsureDefaults();
                _currentTarget = nextTarget;
                _currentCue = cue;
                _currentNpc = UnityEngine.Object.Instantiate(entry.prefab);
                _currentNpc.name = $"RehearsalBake_{nextTarget.cueId}_{nextTarget.npcId}";
                _currentNpc.transform.position = ToVector3(ResolveInitialPosition(entry, cue));

                NPCMotionController motionController = _currentNpc.GetComponent<NPCMotionController>();
                if (motionController != null)
                {
                    motionController.StopMotion();
                    motionController.SetFacingDirection(cue.facing);
                }

                _currentDriver = _currentNpc.GetComponent<SpringDay1DirectorStagingRehearsalDriver>();
                if (_currentDriver == null)
                {
                    _currentDriver = _currentNpc.AddComponent<SpringDay1DirectorStagingRehearsalDriver>();
                }

                _currentDriver.Configure(Mathf.Max(0.6f, nextTarget.rehearsalSpeed));
                _currentStepIndex = 0;
                _currentStepStartedAt = now;
                _lastRecordAt = 0d;
                _lastDriveAt = now;
                RecordedSamples.Clear();
                TryRecordSample(force: true, facingHint: cue.facing, holdSeconds: Mathf.Max(0.12f, cue.initialHoldSeconds));
                return true;
            }

            return false;
        }

        private static void UpdateCurrentTarget(double now)
        {
            if (_currentDriver == null || _currentCue == null)
            {
                Fail("runtime-target-missing", "导演 bake 当前目标运行态已丢失，无法继续。");
                return;
            }

            if (_currentStepIndex >= _currentTarget.steps.Length)
            {
                CompleteCurrentTarget();
                return;
            }

            BakeStep step = _currentTarget.steps[_currentStepIndex];
            double elapsed = now - _currentStepStartedAt;
            if (elapsed < step.duration)
            {
                DriveCurrentTarget(step, now);
                TryRecordSample(force: false, facingHint: step.input, holdSeconds: 0.08f);
                return;
            }

            _currentDriver.SetRehearsalInput(Vector2.zero, false);
            StopCurrentTargetMotion();
            TryRecordSample(force: true, facingHint: step.input, holdSeconds: Mathf.Max(0.08f, step.holdSecondsAfter));
            _currentStepIndex++;
            _currentStepStartedAt = now;
        }

        private static void CompleteCurrentTarget()
        {
            _currentDriver.SetRehearsalInput(Vector2.zero, false);
            TryRecordSample(force: true, facingHint: Vector2.zero, holdSeconds: 0.12f);
            if (RecordedSamples.Count < 2)
            {
                Fail("recorded-samples-too-short", $"Cue {_currentTarget.cueId} 采样点不足，无法写回。");
                return;
            }

            ApplySamplesToCue(_currentCue, _currentTarget, RecordedSamples);
            CompletedTargets.Add(new BakeTargetRunResult
            {
                cueId = _currentTarget.cueId,
                npcId = _currentTarget.npcId,
                beatKey = _currentTarget.beatKey,
                sampleCount = RecordedSamples.Count,
                pathPointCount = _currentCue.path != null ? _currentCue.path.Length : 0,
                startPosition = FormatVector(_currentCue.startPosition),
                finalPosition = FormatVector(RecordedSamples[RecordedSamples.Count - 1].position)
            });

            CleanupCurrentRuntime();
        }

        private static void ApplySamplesToCue(SpringDay1DirectorActorCue cue, BakeTargetSpec target, List<RecordedCueSample> samples)
        {
            cue.keepCurrentSpawnPosition = false;
            cue.useSemanticAnchorAsStart = false;
            cue.startPositionIsSemanticAnchorOffset = false;
            cue.pathPointsAreOffsets = false;
            cue.startPosition = samples[0].position;
            cue.facing = samples[0].facing;
            cue.initialHoldSeconds = Mathf.Max(0.12f, samples[0].holdSeconds);
            cue.moveSpeed = Mathf.Max(0.6f, target.rehearsalSpeed);

            List<SpringDay1DirectorPathPoint> points = new List<SpringDay1DirectorPathPoint>(samples.Count - 1);
            for (int index = 1; index < samples.Count; index++)
            {
                RecordedCueSample sample = samples[index];
                points.Add(new SpringDay1DirectorPathPoint
                {
                    position = sample.position,
                    facing = sample.facing,
                    holdSeconds = Mathf.Max(0.08f, sample.holdSeconds),
                    lookAtTargetName = cue.lookAtTargetName
                });
            }

            cue.path = points.ToArray();
        }

        private static void TryRecordSample(bool force, Vector2 facingHint, float holdSeconds)
        {
            if (_currentNpc == null)
            {
                return;
            }

            double now = EditorApplication.timeSinceStartup;
            if (!force && now - _lastRecordAt < RecordInterval)
            {
                return;
            }

            Vector2 position = _currentNpc.transform.position;
            Vector2 facing = ResolveFacing(_currentNpc, facingHint, _currentCue != null ? _currentCue.facing : Vector2.down);
            if (!force && RecordedSamples.Count > 0)
            {
                RecordedCueSample last = RecordedSamples[RecordedSamples.Count - 1];
                bool movedEnough = Vector2.Distance(last.position, position) >= RecordMinDistance;
                bool facingChanged = Vector2.Distance(last.facing, facing) >= 0.2f;
                if (!movedEnough && !facingChanged)
                {
                    return;
                }
            }

            RecordedSamples.Add(new RecordedCueSample(position, facing, holdSeconds));
            _lastRecordAt = now;
        }

        private static Vector2 ResolveFacing(GameObject targetObject, Vector2 facingHint, Vector2 fallbackFacing)
        {
            if (facingHint.sqrMagnitude > 0.0001f)
            {
                return SnapFacing(facingHint);
            }

            if (targetObject != null)
            {
                NPCAnimController animController = targetObject.GetComponent<NPCAnimController>();
                if (animController != null)
                {
                    return animController.CurrentDirection switch
                    {
                        NPCAnimController.NPCAnimDirection.Up => Vector2.up,
                        NPCAnimController.NPCAnimDirection.Right => Vector2.right,
                        NPCAnimController.NPCAnimDirection.Left => Vector2.left,
                        _ => Vector2.down
                    };
                }
            }

            return fallbackFacing.sqrMagnitude > 0.0001f ? SnapFacing(fallbackFacing) : Vector2.down;
        }

        private static Vector2 SnapFacing(Vector2 input)
        {
            if (Mathf.Abs(input.x) >= Mathf.Abs(input.y))
            {
                return input.x >= 0f ? Vector2.right : Vector2.left;
            }

            return input.y >= 0f ? Vector2.up : Vector2.down;
        }

        private static Vector2 ResolveInitialPosition(SpringDay1NpcCrowdManifest.Entry entry, SpringDay1DirectorActorCue cue)
        {
            if (!cue.keepCurrentSpawnPosition && cue.startPosition.sqrMagnitude > 0.0001f)
            {
                return cue.startPosition;
            }

            return entry != null ? entry.fallbackWorldPosition + entry.spawnOffset : cue.startPosition;
        }

        private static void FinishSuccess()
        {
            try
            {
                SpringDay1DirectorStagingDatabase.Save(_book);
                WriteReport(new BakeReport
                {
                    timestamp = DateTime.Now.ToString("O"),
                    status = "completed",
                    success = true,
                    message = $"导演 rehearsal bake 已完成，写回 {CompletedTargets.Count} 条 cue。",
                    activeScene = SceneManager.GetActiveScene().path,
                    targets = CompletedTargets.ToArray()
                });
            }
            catch (Exception exception)
            {
                Fail("save-failed", $"导演 bake 保存 StageBook 失败：{exception.GetType().Name}: {exception.Message}");
                return;
            }

            _running = false;
            _stopRequested = true;
            CleanupCurrentRuntime();
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
        }

        private static void RunEditModeFallbackBake()
        {
            try
            {
                _book = SpringDay1DirectorStagingDatabase.Load(forceReload: true);
                _manifest = Resources.Load<SpringDay1NpcCrowdManifest>(ManifestResourcePath);
                if (_book == null)
                {
                    Fail("stage-book-missing", "未能加载导演 StageBook，无法继续 fallback bake。");
                    return;
                }

                if (_manifest == null)
                {
                    Fail("manifest-missing", "未能加载 SpringDay1NpcCrowdManifest，无法继续 fallback bake。");
                    return;
                }

                _queued = false;
                SessionState.SetBool(QueuedSessionKey, false);
                CompletedTargets.Clear();
                for (int index = 0; index < Targets.Length; index++)
                {
                    if (!BakeTargetInEditMode(Targets[index]))
                    {
                        return;
                    }
                }

                SpringDay1DirectorStagingDatabase.Save(_book);
                WriteReport(new BakeReport
                {
                    timestamp = DateTime.Now.ToString("O"),
                    status = "completed",
                    success = true,
                    message = $"导演 rehearsal bake 已通过 edit-mode fallback 完成，写回 {CompletedTargets.Count} 条 cue。",
                    activeScene = SceneManager.GetActiveScene().path,
                    targets = CompletedTargets.ToArray()
                });
                ResetSessionState();
            }
            catch (Exception exception)
            {
                Fail("fallback-bake-failed", $"edit-mode fallback bake 失败：{exception.GetType().Name}: {exception.Message}");
            }
        }

        private static bool BakeTargetInEditMode(BakeTargetSpec target)
        {
            SpringDay1DirectorBeatEntry beat = _book.FindBeat(target.beatKey);
            SpringDay1DirectorActorCue cue = FindCueById(beat, target.cueId, target.npcId);
            SpringDay1NpcCrowdManifest.Entry entry = FindEntryByNpcId(_manifest, target.npcId);
            if (beat == null || cue == null || entry == null || entry.prefab == null)
            {
                Fail("target-missing", $"fallback bake 缺少目标定义：beat={target.beatKey}, cue={target.cueId}, npcId={target.npcId}。");
                return false;
            }

            cue.EnsureDefaults();
            GameObject npc = UnityEngine.Object.Instantiate(entry.prefab);
            try
            {
                npc.name = $"FallbackBake_{target.cueId}_{target.npcId}";
                npc.transform.position = ToVector3(ResolveInitialPosition(entry, cue));
                NPCMotionController motionController = npc.GetComponent<NPCMotionController>();
                if (motionController != null)
                {
                    motionController.StopMotion();
                    motionController.SetFacingDirection(cue.facing);
                }

                List<RecordedCueSample> samples = new List<RecordedCueSample>();
                RecordDirectSample(samples, npc.transform.position, cue.facing, Mathf.Max(0.12f, cue.initialHoldSeconds));

                const float simulationStep = 1f / 30f;
                for (int stepIndex = 0; stepIndex < target.steps.Length; stepIndex++)
                {
                    BakeStep step = target.steps[stepIndex];
                    int frameCount = Mathf.Max(1, Mathf.CeilToInt(step.duration / simulationStep));
                    for (int frame = 0; frame < frameCount; frame++)
                    {
                        if (step.input.sqrMagnitude > 0.0001f)
                        {
                            float speed = Mathf.Max(0.6f, target.rehearsalSpeed) * (step.sprint ? 1.6f : 1f);
                            Vector3 delta = new Vector3(step.input.x, step.input.y, 0f).normalized * (speed * simulationStep);
                            npc.transform.position += delta;
                            if (motionController != null)
                            {
                                Vector2 velocity = new Vector2(delta.x, delta.y) / simulationStep;
                                motionController.SetExternalVelocity(velocity);
                                motionController.SetFacingDirection(step.input);
                            }
                        }
                        else if (motionController != null)
                        {
                            motionController.StopMotion();
                        }
                    }

                    RecordDirectSample(samples, npc.transform.position, step.input, Mathf.Max(0.08f, step.holdSecondsAfter));
                }

                if (motionController != null)
                {
                    motionController.StopMotion();
                }

                ApplySamplesToCue(cue, target, samples);
                CompletedTargets.Add(new BakeTargetRunResult
                {
                    cueId = target.cueId,
                    npcId = target.npcId,
                    beatKey = target.beatKey,
                    sampleCount = samples.Count,
                    pathPointCount = cue.path != null ? cue.path.Length : 0,
                    startPosition = FormatVector(cue.startPosition),
                    finalPosition = FormatVector(samples[samples.Count - 1].position)
                });
                return true;
            }
            finally
            {
                if (npc != null)
                {
                    UnityEngine.Object.DestroyImmediate(npc);
                }
            }
        }

        private static void RecordDirectSample(List<RecordedCueSample> samples, Vector3 worldPosition, Vector2 facingHint, float holdSeconds)
        {
            samples.Add(new RecordedCueSample(new Vector2(worldPosition.x, worldPosition.y), facingHint.sqrMagnitude > 0.0001f ? SnapFacing(facingHint) : Vector2.down, holdSeconds));
        }

        private static void Fail(string blocker, string message)
        {
            WriteReport(new BakeReport
            {
                timestamp = DateTime.Now.ToString("O"),
                status = "blocked",
                success = false,
                firstBlocker = blocker,
                message = message,
                activeScene = SceneManager.GetActiveScene().path,
                targets = CompletedTargets.ToArray()
            });

            _queued = false;
            _running = false;
            _stopRequested = true;
            CleanupCurrentRuntime();
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
        }

        private static void CleanupCurrentRuntime()
        {
            StopCurrentTargetMotion();
            if (_currentDriver != null)
            {
                UnityEngine.Object.Destroy(_currentDriver);
                _currentDriver = null;
            }

            if (_currentNpc != null)
            {
                UnityEngine.Object.Destroy(_currentNpc);
                _currentNpc = null;
            }

            _currentCue = null;
            _currentStepIndex = 0;
            _currentStepStartedAt = 0d;
            _lastRecordAt = 0d;
            _lastDriveAt = 0d;
            _queuedAt = 0d;
            RecordedSamples.Clear();
        }

        private static void DriveCurrentTarget(BakeStep step, double now)
        {
            if (_currentDriver == null || _currentNpc == null)
            {
                return;
            }

            _currentDriver.SetRehearsalInput(step.input, step.sprint);
            float deltaTime = (float)Math.Max(0.0, now - _lastDriveAt);
            _lastDriveAt = now;
            if (deltaTime <= 0f || step.input.sqrMagnitude <= 0.0001f)
            {
                StopCurrentTargetMotion();
                return;
            }

            float sprintMultiplier = step.sprint ? 1.6f : 1f;
            float speed = Mathf.Max(0.6f, _currentTarget.rehearsalSpeed) * sprintMultiplier;
            Vector3 delta = new Vector3(step.input.x, step.input.y, 0f).normalized * (speed * deltaTime);
            _currentNpc.transform.position += delta;

            NPCMotionController motionController = _currentNpc.GetComponent<NPCMotionController>();
            if (motionController != null)
            {
                Vector2 velocity = new Vector2(delta.x, delta.y) / Mathf.Max(deltaTime, 0.0001f);
                motionController.SetExternalVelocity(velocity);
                motionController.SetFacingDirection(step.input);
            }
        }

        private static void StopCurrentTargetMotion()
        {
            if (_currentNpc == null)
            {
                return;
            }

            NPCMotionController motionController = _currentNpc.GetComponent<NPCMotionController>();
            if (motionController != null)
            {
                motionController.StopMotion();
            }
        }

        private static SpringDay1DirectorActorCue FindCueById(SpringDay1DirectorBeatEntry beat, string cueId, string npcId)
        {
            if (beat == null || beat.actorCues == null)
            {
                return null;
            }

            for (int index = 0; index < beat.actorCues.Length; index++)
            {
                SpringDay1DirectorActorCue cue = beat.actorCues[index];
                if (cue == null)
                {
                    continue;
                }

                if (string.Equals(cue.cueId?.Trim(), cueId, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(cue.npcId?.Trim(), npcId, StringComparison.OrdinalIgnoreCase))
                {
                    return cue;
                }
            }

            return null;
        }

        private static SpringDay1NpcCrowdManifest.Entry FindEntryByNpcId(SpringDay1NpcCrowdManifest manifest, string npcId)
        {
            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry != null && string.Equals(entry.npcId?.Trim(), npcId, StringComparison.OrdinalIgnoreCase))
                {
                    return entry;
                }
            }

            return null;
        }

        private static void WriteReport(BakeReport report)
        {
            Directory.CreateDirectory(CommandRoot);
            File.WriteAllText(ResultPath, JsonUtility.ToJson(report, prettyPrint: true), new UTF8Encoding(false));
        }

        private static void ResetSessionState()
        {
            _queued = false;
            SessionState.SetBool(QueuedSessionKey, false);
            _running = false;
            _stopRequested = false;
            _book = null;
            _manifest = null;
            _targetIndex = 0;
            _currentTarget = default;
            _queuedAt = 0d;
            CleanupCurrentRuntime();
            CompletedTargets.Clear();
        }

        private static Vector3 ToVector3(Vector2 value)
        {
            return new Vector3(value.x, value.y, 0f);
        }

        private static string FormatVector(Vector2 value)
        {
            return $"{value.x:F3},{value.y:F3}";
        }

        [Serializable]
        private sealed class BakeReport
        {
            public string timestamp;
            public string status;
            public bool success;
            public string firstBlocker;
            public string message;
            public string activeScene;
            public BakeTargetRunResult[] targets;
        }

        [Serializable]
        private sealed class BakeTargetRunResult
        {
            public string cueId;
            public string npcId;
            public string beatKey;
            public int sampleCount;
            public int pathPointCount;
            public string startPosition;
            public string finalPosition;
        }

        private readonly struct BakeTargetSpec
        {
            public BakeTargetSpec(string beatKey, string cueId, string npcId, float rehearsalSpeed, BakeStep[] steps)
            {
                this.beatKey = beatKey;
                this.cueId = cueId;
                this.npcId = npcId;
                this.rehearsalSpeed = rehearsalSpeed;
                this.steps = steps ?? Array.Empty<BakeStep>();
            }

            public string beatKey { get; }
            public string cueId { get; }
            public string npcId { get; }
            public float rehearsalSpeed { get; }
            public BakeStep[] steps { get; }
        }

        private readonly struct BakeStep
        {
            public BakeStep(Vector2 input, float duration, bool sprint, float holdSecondsAfter)
            {
                this.input = input;
                this.duration = Mathf.Max(0.01f, duration);
                this.sprint = sprint;
                this.holdSecondsAfter = Mathf.Max(0.08f, holdSecondsAfter);
            }

            public Vector2 input { get; }
            public float duration { get; }
            public bool sprint { get; }
            public float holdSecondsAfter { get; }
        }

        private readonly struct RecordedCueSample
        {
            public RecordedCueSample(Vector2 position, Vector2 facing, float holdSeconds)
            {
                this.position = position;
                this.facing = facing;
                this.holdSeconds = holdSeconds;
            }

            public Vector2 position { get; }
            public Vector2 facing { get; }
            public float holdSeconds { get; }
        }
    }
}
#endif
