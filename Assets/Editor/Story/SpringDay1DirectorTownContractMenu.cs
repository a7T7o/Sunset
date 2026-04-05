#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Editor.Story
{
    internal static class SpringDay1DirectorTownContractMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Run Director Town Contract Probe";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "spring-day1-town-contract-probe.json");

        private static readonly string[] TargetAnchors =
        {
            "EnterVillageCrowdRoot",
            "KidLook_01",
            "NightWitness_01",
            "DinnerBackgroundRoot",
            "DailyStand_01"
        };

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ResultPath, ToJson(result), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[SpringDay1DirectorTownContract] {result.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static ProbeResult BuildProbeResult()
        {
            Scene loadedTownScene = default;
            bool loadedTemporarily = false;
            try
            {
                loadedTownScene = SceneManager.GetSceneByPath(TownScenePath);
                if (!loadedTownScene.IsValid() || !loadedTownScene.isLoaded)
                {
                    loadedTownScene = EditorSceneManager.OpenScene(TownScenePath, OpenSceneMode.Additive);
                    loadedTemporarily = true;
                }

                List<AnchorRecord> anchors = BuildAnchorRecords(loadedTownScene);
                if (!loadedTownScene.IsValid() || !loadedTownScene.isLoaded)
                {
                    return ProbeResult.Blocked(
                        "town-scene-unavailable",
                        "Town 场景当前无法只读加载，无法继续做导演 Town contract probe。",
                        anchors);
                }

                for (int index = 0; index < anchors.Count; index++)
                {
                    if (!anchors[index].exists)
                    {
                        return ProbeResult.Blocked(
                            "anchor-missing",
                            $"Town 里缺少锚点 {anchors[index].anchorName}，当前无法继续做导演 live contract。",
                            anchors);
                    }
                }

                bool allAtOrigin = true;
                bool allSameWorldPosition = true;
                Vector3 firstWorldPosition = anchors[0].worldPosition;
                for (int index = 0; index < anchors.Count; index++)
                {
                    AnchorRecord record = anchors[index];
                    if (record.worldPosition.sqrMagnitude > 0.0001f)
                    {
                        allAtOrigin = false;
                    }

                    if ((record.worldPosition - firstWorldPosition).sqrMagnitude > 0.0001f)
                    {
                        allSameWorldPosition = false;
                    }
                }

                if (allAtOrigin && allSameWorldPosition)
                {
                    return ProbeResult.Blocked(
                        "town-anchor-empty-transform",
                        "Town 里的 Day1 anchor 现在都还是空 Transform，世界位置重合在原点。导演语义已成立，但 scene anchor -> runtime 真落位这条 contract 还没接上，当前不适合把它包装成可直接 live 排练的最终 anchor。",
                        anchors);
                }

                return ProbeResult.Blocked(
                    "town-anchor-contract-pending",
                    "Town anchor 已存在，但当前还缺 scene anchor -> runtime 真落位的消费 contract。场内 live probe 已证明确有锚点，下一步应继续把 runtime contract 补进导演链。",
                    anchors);
            }
            finally
            {
                if (loadedTemporarily && loadedTownScene.IsValid() && loadedTownScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(loadedTownScene, true);
                }
            }
        }

        private static List<AnchorRecord> BuildAnchorRecords(Scene townScene)
        {
            Dictionary<string, Transform> lookup = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            if (townScene.IsValid() && townScene.isLoaded)
            {
                GameObject[] roots = townScene.GetRootGameObjects();
                for (int index = 0; index < roots.Length; index++)
                {
                    CollectTransforms(roots[index].transform, lookup);
                }
            }

            List<AnchorRecord> anchors = new List<AnchorRecord>(TargetAnchors.Length);
            foreach (string anchorName in TargetAnchors)
            {
                lookup.TryGetValue(anchorName, out Transform anchorTransform);
                anchors.Add(new AnchorRecord(anchorName, anchorTransform));
            }

            return anchors;
        }

        private static void CollectTransforms(Transform current, Dictionary<string, Transform> lookup)
        {
            if (current == null)
            {
                return;
            }

            if (!lookup.ContainsKey(current.name))
            {
                lookup[current.name] = current;
            }

            for (int index = 0; index < current.childCount; index++)
            {
                CollectTransforms(current.GetChild(index), lookup);
            }
        }

        private static string ToJson(ProbeResult result)
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
            builder.Append(",\"anchors\":[");
            for (int index = 0; index < result.anchors.Count; index++)
            {
                if (index > 0)
                {
                    builder.Append(',');
                }

                AnchorRecord record = result.anchors[index];
                builder.Append('{');
                AppendPair(builder, "anchorName", record.anchorName);
                builder.Append(',');
                builder.Append("\"exists\":");
                builder.Append(record.exists ? "true" : "false");
                builder.Append(',');
                AppendPair(builder, "parentName", record.parentName);
                builder.Append(',');
                AppendPair(builder, "worldPosition", FormatVector(record.worldPosition));
                builder.Append(',');
                AppendPair(builder, "localPosition", FormatVector(record.localPosition));
                builder.Append('}');
            }

            builder.Append("]}");
            return builder.ToString();
        }

        private static string FormatVector(Vector3 value)
        {
            return $"{value.x:F3},{value.y:F3},{value.z:F3}";
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

        private readonly struct ProbeResult
        {
            private ProbeResult(string status, bool success, string firstBlocker, string message, List<AnchorRecord> anchors)
            {
                this.status = status;
                this.success = success;
                this.firstBlocker = firstBlocker;
                this.message = message;
                this.anchors = anchors;
            }

            public string status { get; }
            public bool success { get; }
            public string firstBlocker { get; }
            public string message { get; }
            public List<AnchorRecord> anchors { get; }

            public static ProbeResult Pass(string message, List<AnchorRecord> anchors)
            {
                return new ProbeResult("completed", true, string.Empty, message, anchors);
            }

            public static ProbeResult Blocked(string blocker, string message, List<AnchorRecord> anchors)
            {
                return new ProbeResult("blocked", false, blocker, message, anchors);
            }
        }

        private readonly struct AnchorRecord
        {
            public AnchorRecord(string anchorName, Transform anchor)
            {
                this.anchorName = anchorName;
                exists = anchor != null;
                parentName = anchor != null && anchor.parent != null
                    ? anchor.parent.name
                    : string.Empty;
                worldPosition = anchor != null ? anchor.position : Vector3.zero;
                localPosition = anchor != null ? anchor.localPosition : Vector3.zero;
            }

            public string anchorName { get; }
            public bool exists { get; }
            public string parentName { get; }
            public Vector3 worldPosition { get; }
            public Vector3 localPosition { get; }
        }
    }
}
#endif
