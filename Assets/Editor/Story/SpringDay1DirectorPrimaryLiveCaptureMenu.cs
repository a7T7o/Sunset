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
    internal static class SpringDay1DirectorPrimaryLiveCaptureMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Run Director Primary Live Capture";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "spring-day1-director-primary-live-capture.json");
        private static readonly string RehearsalBakeSignalPath = Path.Combine(CommandRoot, "spring-day1-director-primary-rehearsal-bake.signal");

        private readonly struct CaptureTarget
        {
            public CaptureTarget(string beatKey, string cueId, string npcId)
            {
                this.beatKey = beatKey;
                this.cueId = cueId;
                this.npcId = npcId;
            }

            public string beatKey { get; }
            public string cueId { get; }
            public string npcId { get; }
        }

        private static readonly CaptureTarget[] Targets =
        {
            new CaptureTarget(SpringDay1DirectorBeatKeys.EnterVillagePostEntry, "enter-crowd-101", "101"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.EnterVillagePostEntry, "enter-kid-103", "103"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-203", "203"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-104", "104"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-201", "201"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DinnerConflictTable, "dinner-bg-202", "202"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.FreeTimeNightWitness, "night-witness-102", "102"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.FreeTimeNightWitness, "night-witness-301", "301"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-101", "101"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-103", "103"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-102", "102"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-104", "104"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-203", "203"),
            new CaptureTarget(SpringDay1DirectorBeatKeys.DailyStandPreview, "daily-201", "201")
        };

        [MenuItem(MenuPath)]
        private static void Run()
        {
            if (File.Exists(RehearsalBakeSignalPath))
            {
                File.Delete(RehearsalBakeSignalPath);
                SpringDay1DirectorPrimaryRehearsalBakeMenu.RunFromBridge();
                return;
            }

            Directory.CreateDirectory(CommandRoot);
            CaptureResult result = Capture();
            File.WriteAllText(ResultPath, ToJson(result), new UTF8Encoding(false));
            if (result.success)
            {
                SpringDay1DirectorStagingDatabase.Save(result.book);
            }

            AssetDatabase.Refresh();
            Debug.Log($"[SpringDay1DirectorPrimaryLiveCapture] {result.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static CaptureResult Capture()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            SpringDay1DirectorStageBook book = SpringDay1DirectorStagingDatabase.Load(forceReload: true);
            SpringDay1NpcCrowdManifest manifest = Resources.Load<SpringDay1NpcCrowdManifest>("Story/SpringDay1/SpringDay1NpcCrowdManifest");

            if (!activeScene.IsValid() || !string.Equals(activeScene.name, "Primary", StringComparison.OrdinalIgnoreCase))
            {
                return CaptureResult.Blocked(book, "active-scene-not-primary", $"当前活动场景不是 Primary，而是 {activeScene.name}。Primary live capture 只吃当前代理现场。");
            }

            if (manifest == null)
            {
                return CaptureResult.Blocked(book, "manifest-missing", "未找到 SpringDay1NpcCrowdManifest，无法继续做 live capture。");
            }

            List<string> updates = new List<string>();
            foreach (CaptureTarget target in Targets)
            {
                SpringDay1DirectorBeatEntry beat = book.FindBeat(target.beatKey);
                if (beat == null)
                {
                    return CaptureResult.Blocked(book, "beat-missing", $"StageBook 缺少 beat {target.beatKey}。");
                }

                SpringDay1DirectorActorCue cue = FindCueById(beat, target.cueId);
                if (cue == null)
                {
                    return CaptureResult.Blocked(book, "cue-missing", $"Beat {target.beatKey} 缺少 cue {target.cueId}。");
                }

                SpringDay1NpcCrowdManifest.Entry entry = FindEntryByNpcId(manifest, target.npcId);
                if (entry == null)
                {
                    return CaptureResult.Blocked(book, "manifest-entry-missing", $"Manifest 缺少 npcId={target.npcId}。");
                }

                if (!TryResolvePrimaryProxyBasePosition(entry, out Vector2 basePosition, out string anchorName))
                {
                    return CaptureResult.Blocked(book, "primary-proxy-anchor-missing", $"当前 Primary 现场找不到 npcId={target.npcId} 的代理锚点链：{entry.anchorObjectName}。");
                }

                cue.EnsureDefaults();
                cue.keepCurrentSpawnPosition = false;
                cue.startPosition = basePosition;
                if (cue.pathPointsAreOffsets)
                {
                    for (int index = 0; index < cue.path.Length; index++)
                    {
                        SpringDay1DirectorPathPoint point = cue.path[index];
                        if (point == null)
                        {
                            continue;
                        }

                        point.position = basePosition + point.position;
                    }

                    cue.pathPointsAreOffsets = false;
                }

                updates.Add($"{target.cueId}@{anchorName}->{basePosition.x:F2}/{basePosition.y:F2}");
            }

            book.EnsureDefaults();
            return CaptureResult.Pass(book, $"Primary live capture 已把 {updates.Count} 条关键 cue 写成绝对落位。", updates);
        }

        private static SpringDay1DirectorActorCue FindCueById(SpringDay1DirectorBeatEntry beat, string cueId)
        {
            if (beat == null || beat.actorCues == null)
            {
                return null;
            }

            for (int index = 0; index < beat.actorCues.Length; index++)
            {
                SpringDay1DirectorActorCue cue = beat.actorCues[index];
                if (cue != null && string.Equals(cue.cueId?.Trim(), cueId, StringComparison.OrdinalIgnoreCase))
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

        private static bool TryResolvePrimaryProxyBasePosition(SpringDay1NpcCrowdManifest.Entry entry, out Vector2 basePosition, out string anchorName)
        {
            basePosition = Vector2.zero;
            anchorName = string.Empty;
            if (entry == null || string.IsNullOrWhiteSpace(entry.anchorObjectName))
            {
                return false;
            }

            string[] candidates = entry.anchorObjectName.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < candidates.Length; index++)
            {
                string candidate = candidates[index].Trim();
                if (string.IsNullOrWhiteSpace(candidate))
                {
                    continue;
                }

                GameObject anchor = GameObject.Find(candidate);
                if (anchor == null)
                {
                    string derivedHomeAnchor = TryBuildHomeAnchorAlias(candidate);
                    if (!string.IsNullOrEmpty(derivedHomeAnchor))
                    {
                        anchor = GameObject.Find(derivedHomeAnchor);
                    }

                    if (anchor == null && !candidate.EndsWith("_HomeAnchor", StringComparison.OrdinalIgnoreCase))
                    {
                        anchor = GameObject.Find($"{candidate}_HomeAnchor");
                    }
                }

                if (anchor == null)
                {
                    continue;
                }

                Vector3 world = anchor.transform.position + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
                basePosition = new Vector2(world.x, world.y);
                anchorName = anchor.name;
                return true;
            }

            return false;
        }

        private static string TryBuildHomeAnchorAlias(string candidateName)
        {
            if (!candidateName.StartsWith("NPC", StringComparison.OrdinalIgnoreCase) || candidateName.Length <= 3)
            {
                return null;
            }

            string numericSuffix = candidateName.Substring(3);
            for (int index = 0; index < numericSuffix.Length; index++)
            {
                if (!char.IsDigit(numericSuffix[index]))
                {
                    return null;
                }
            }

            return $"{numericSuffix}_HomeAnchor";
        }

        private static string ToJson(CaptureResult result)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('{');
            AppendPair(builder, "timestamp", DateTime.Now.ToString("O"));
            builder.Append(',');
            AppendPair(builder, "status", result.status);
            builder.Append(',');
            builder.Append("\"success\":");
            builder.Append(result.success ? "true" : "false");
            builder.Append(',');
            AppendPair(builder, "firstBlocker", result.firstBlocker);
            builder.Append(',');
            AppendPair(builder, "message", result.message);
            builder.Append(",\"updates\":[");
            for (int index = 0; index < result.updates.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(',');
                }

                builder.Append('"');
                builder.Append(Escape(result.updates[index]));
                builder.Append('"');
            }

            builder.Append("]}");
            return builder.ToString();
        }

        private static void AppendPair(StringBuilder builder, string key, string value)
        {
            builder.Append('"');
            builder.Append(Escape(key));
            builder.Append("\":\"");
            builder.Append(Escape(value));
            builder.Append('"');
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private readonly struct CaptureResult
        {
            private CaptureResult(SpringDay1DirectorStageBook book, string status, bool success, string firstBlocker, string message, List<string> updates)
            {
                this.book = book;
                this.status = status;
                this.success = success;
                this.firstBlocker = firstBlocker;
                this.message = message;
                this.updates = updates;
            }

            public SpringDay1DirectorStageBook book { get; }
            public string status { get; }
            public bool success { get; }
            public string firstBlocker { get; }
            public string message { get; }
            public List<string> updates { get; }

            public static CaptureResult Pass(SpringDay1DirectorStageBook book, string message, List<string> updates)
            {
                return new CaptureResult(book, "completed", true, string.Empty, message, updates);
            }

            public static CaptureResult Blocked(SpringDay1DirectorStageBook book, string firstBlocker, string message)
            {
                return new CaptureResult(book, "blocked", false, firstBlocker, message, new List<string>());
            }
        }
    }
}
#endif
