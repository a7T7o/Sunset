#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sunset.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class TownSceneRuntimeAnchorReadinessMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Run Town Runtime Anchor Readiness Probe";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";
        private const string ManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";
        private const float SlotAlignmentBlockedDistance = 1.0f;
        private const float SlotAlignmentAttentionDistance = 0.2f;
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "town-runtime-anchor-readiness-probe.json");

        private static readonly AnchorExpectation[] Expectations = Array.Empty<AnchorExpectation>();

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string firstBlocker = string.Empty;
            public string message = string.Empty;
            public string scenePath = TownScenePath;
            public string manifestPath = ManifestPath;
            public string boundsPath = string.Empty;
            public string boundsMin = string.Empty;
            public string boundsMax = string.Empty;
            public List<AnchorReadinessRecord> anchors = new List<AnchorReadinessRecord>();
            public List<string> blockingFindings = new List<string>();
            public List<string> attentionFindings = new List<string>();
        }

        [Serializable]
        private sealed class AnchorReadinessRecord
        {
            public string anchorName = string.Empty;
            public bool anchorExists;
            public string anchorPath = string.Empty;
            public string anchorPosition = string.Empty;
            public bool anchorInsideBounds;
            public string primarySlotName = string.Empty;
            public string primarySlotRole = string.Empty;
            public bool primarySlotExists;
            public string primarySlotPath = string.Empty;
            public string primarySlotPosition = string.Empty;
            public float primarySlotDistance = -1f;
            public bool primarySlotInsideBounds;
            public string primaryHomeAnchorName = string.Empty;
            public bool primaryHomeAnchorExists;
            public string primaryHomeAnchorPath = string.Empty;
            public string primaryHomeAnchorPosition = string.Empty;
            public bool primaryHomeAnchorInsideBounds;
            public string secondarySlotName = string.Empty;
            public string secondarySlotRole = string.Empty;
            public bool secondarySlotExists;
            public string secondarySlotPath = string.Empty;
            public string secondarySlotPosition = string.Empty;
            public float secondarySlotDistance = -1f;
            public bool secondarySlotInsideBounds;
            public string secondaryHomeAnchorName = string.Empty;
            public bool secondaryHomeAnchorExists;
            public string secondaryHomeAnchorPath = string.Empty;
            public string secondaryHomeAnchorPosition = string.Empty;
            public bool secondaryHomeAnchorInsideBounds;
            public string[] consumerNpcIds = Array.Empty<string>();
            public string[] consumerDisplayNames = Array.Empty<string>();
        }

        private enum SlotRole
        {
            ResidentSlot,
            DirectorReady,
            BackstageSlot
        }

        private readonly struct AnchorExpectation
        {
            public AnchorExpectation(string anchorName, string primarySlotName, SlotRole primaryRole, string secondarySlotName = "")
            {
                AnchorName = anchorName;
                PrimarySlotName = primarySlotName;
                PrimaryRole = primaryRole;
                SecondarySlotName = secondarySlotName ?? string.Empty;
            }

            public string AnchorName { get; }
            public string PrimarySlotName { get; }
            public SlotRole PrimaryRole { get; }
            public string SecondarySlotName { get; }
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            ProbeResult result = BuildProbeResult();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[TownRuntimeAnchorReadinessProbe] {result.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
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

                ProbeResult result = new ProbeResult
                {
                    timestamp = DateTime.Now.ToString("O")
                };

                AddBlocker(
                    result,
                    "deprecated-runtime-anchor-readiness-probe",
                    "旧 Town runtime anchor readiness probe 已退役：它会把旧 slot/root/anchor 中间层当成真值。后续请改用 opening/dinner 的 staged marker probe。");
                FinalizeResult(result);
                return result;
            }
            finally
            {
                if (loadedTemporarily && loadedTownScene.IsValid() && loadedTownScene.isLoaded)
                {
                    EditorSceneManager.CloseScene(loadedTownScene, true);
                }
            }
        }

        private static AnchorReadinessRecord BuildRecord(
            AnchorExpectation expectation,
            Dictionary<string, Transform> lookup,
            Bounds bounds,
            SpringDay1NpcCrowdManifest manifest)
        {
            lookup.TryGetValue(expectation.AnchorName, out Transform anchor);
            lookup.TryGetValue(expectation.PrimarySlotName, out Transform primarySlot);
            Transform secondarySlot = null;
            if (!string.IsNullOrWhiteSpace(expectation.SecondarySlotName))
            {
                lookup.TryGetValue(expectation.SecondarySlotName, out secondarySlot);
            }

            Transform primaryHomeAnchor = FindHomeAnchor(primarySlot);
            Transform secondaryHomeAnchor = FindHomeAnchor(secondarySlot);

            SpringDay1NpcCrowdManifest.Entry[] entries = manifest != null ? manifest.Entries : Array.Empty<SpringDay1NpcCrowdManifest.Entry>();
            List<string> consumerNpcIds = new List<string>();
            List<string> consumerNames = new List<string>();
            for (int index = 0; index < entries.Length; index++)
            {
                SpringDay1NpcCrowdManifest.Entry entry = entries[index];
                if (entry == null || !entry.SupportsSemanticAnchor(expectation.AnchorName))
                {
                    continue;
                }

                consumerNpcIds.Add(entry.npcId ?? string.Empty);
                consumerNames.Add(entry.displayName ?? string.Empty);
            }

            AnchorReadinessRecord record = new AnchorReadinessRecord
            {
                anchorName = expectation.AnchorName,
                anchorExists = anchor != null,
                anchorPath = anchor != null ? GetTransformPath(anchor) : string.Empty,
                anchorPosition = anchor != null ? FormatVector(anchor.position) : string.Empty,
                anchorInsideBounds = anchor != null && Contains(bounds, anchor.position),
                primarySlotName = expectation.PrimarySlotName,
                primarySlotRole = expectation.PrimaryRole.ToString(),
                primarySlotExists = primarySlot != null,
                primarySlotPath = primarySlot != null ? GetTransformPath(primarySlot) : string.Empty,
                primarySlotPosition = primarySlot != null ? FormatVector(primarySlot.position) : string.Empty,
                primarySlotDistance = CalculateDistance(anchor, primarySlot),
                primarySlotInsideBounds = primarySlot != null && Contains(bounds, primarySlot.position),
                primaryHomeAnchorName = primaryHomeAnchor != null ? primaryHomeAnchor.name : string.Empty,
                primaryHomeAnchorExists = primaryHomeAnchor != null,
                primaryHomeAnchorPath = primaryHomeAnchor != null ? GetTransformPath(primaryHomeAnchor) : string.Empty,
                primaryHomeAnchorPosition = primaryHomeAnchor != null ? FormatVector(primaryHomeAnchor.position) : string.Empty,
                primaryHomeAnchorInsideBounds = primaryHomeAnchor != null && Contains(bounds, primaryHomeAnchor.position),
                secondarySlotName = expectation.SecondarySlotName,
                secondarySlotRole = string.IsNullOrWhiteSpace(expectation.SecondarySlotName) ? string.Empty : SlotRole.DirectorReady.ToString(),
                secondarySlotExists = secondarySlot != null,
                secondarySlotPath = secondarySlot != null ? GetTransformPath(secondarySlot) : string.Empty,
                secondarySlotPosition = secondarySlot != null ? FormatVector(secondarySlot.position) : string.Empty,
                secondarySlotDistance = CalculateDistance(anchor, secondarySlot),
                secondarySlotInsideBounds = secondarySlot != null && Contains(bounds, secondarySlot.position),
                secondaryHomeAnchorName = secondaryHomeAnchor != null ? secondaryHomeAnchor.name : string.Empty,
                secondaryHomeAnchorExists = secondaryHomeAnchor != null,
                secondaryHomeAnchorPath = secondaryHomeAnchor != null ? GetTransformPath(secondaryHomeAnchor) : string.Empty,
                secondaryHomeAnchorPosition = secondaryHomeAnchor != null ? FormatVector(secondaryHomeAnchor.position) : string.Empty,
                secondaryHomeAnchorInsideBounds = secondaryHomeAnchor != null && Contains(bounds, secondaryHomeAnchor.position),
                consumerNpcIds = consumerNpcIds.ToArray(),
                consumerDisplayNames = consumerNames.ToArray()
            };

            return record;
        }

        private static void EvaluateRecord(AnchorExpectation expectation, AnchorReadinessRecord record, ProbeResult result)
        {
            if (!record.anchorExists)
            {
                AddBlocker(result, "anchor-missing", $"{record.anchorName} 在 Town 中缺失，当前更深 runtime 承接仍有空洞。");
                return;
            }

            if (!record.anchorInsideBounds)
            {
                result.attentionFindings.Add($"{record.anchorName} 当前不在 _CameraBounds 内，更深 runtime 拉人过去时镜头边界可能先撞。");
            }

            if (!record.primarySlotExists)
            {
                AddBlocker(result, "primary-slot-missing", $"{record.anchorName} 缺少 {record.primarySlotName}，这条更深承接位当前还不能算 ready。");
            }
            else
            {
                EvaluateSlotDistance(record.anchorName, record.primarySlotName, record.primarySlotDistance, result);
                if (!record.primarySlotInsideBounds)
                {
                    result.attentionFindings.Add($"{record.primarySlotName} 当前不在 _CameraBounds 内，这条更深承接位后续 live 仍可能先撞边界。");
                }

                if (!record.primaryHomeAnchorExists)
                {
                    AddBlocker(result, "primary-home-anchor-missing", $"{record.primarySlotName} 还没有 HomeAnchor，用户当前没有可手摆的位置节点。");
                }
                else if (!record.primaryHomeAnchorInsideBounds)
                {
                    result.attentionFindings.Add($"{record.primaryHomeAnchorName} 当前不在 _CameraBounds 内，用户手摆位置后续可能先撞边界。");
                }
            }

            if (!string.IsNullOrWhiteSpace(expectation.SecondarySlotName))
            {
                if (!record.secondarySlotExists)
                {
                    AddBlocker(result, "secondary-slot-missing", $"{record.anchorName} 缺少 {record.secondarySlotName}，这条更深双层承接链还没完全站住。");
                }
                else
                {
                    EvaluateSlotDistance(record.anchorName, record.secondarySlotName, record.secondarySlotDistance, result);
                    if (!record.secondarySlotInsideBounds)
                    {
                        result.attentionFindings.Add($"{record.secondarySlotName} 当前不在 _CameraBounds 内，这条更深双层承接链后续 live 仍可能先撞边界。");
                    }

                    if (!record.secondaryHomeAnchorExists)
                    {
                        AddBlocker(result, "secondary-home-anchor-missing", $"{record.secondarySlotName} 还没有 HomeAnchor，这条更深双层承接链还没给用户留出位置配置点。");
                    }
                    else if (!record.secondaryHomeAnchorInsideBounds)
                    {
                        result.attentionFindings.Add($"{record.secondaryHomeAnchorName} 当前不在 _CameraBounds 内，这条更深双层承接链的用户配置位仍可能先撞边界。");
                    }
                }
            }

            if (record.consumerNpcIds == null || record.consumerNpcIds.Length == 0)
            {
                result.attentionFindings.Add($"{record.anchorName} 当前在 manifest 中没有消费者；如果不是刻意预留位，就要防止它变成只有名字没有 runtime 入口的空壳。");
            }
        }

        private static void EvaluateSlotDistance(string anchorName, string slotName, float distance, ProbeResult result)
        {
            if (distance < 0f)
            {
                return;
            }

            if (distance > SlotAlignmentBlockedDistance)
            {
                AddBlocker(result, "slot-alignment-drift", $"{anchorName} 与 {slotName} 当前错位过大，scene-side 位置合同已经漂出可直接消费范围。");
                return;
            }

            if (distance > SlotAlignmentAttentionDistance)
            {
                result.attentionFindings.Add($"{anchorName} 与 {slotName} 当前存在轻度错位，后续 runtime 消费前最好再做一次 live 对位确认。");
            }
        }

        private static void FinalizeResult(ProbeResult result)
        {
            if (result.blockingFindings.Count > 0)
            {
                result.status = "blocked";
                result.success = false;
                result.message = result.blockingFindings[0];
                return;
            }

            if (result.attentionFindings.Count > 0)
            {
                result.status = "attention";
                result.success = false;
                result.firstBlocker = string.Empty;
                result.message = result.attentionFindings[0];
                return;
            }

            result.status = "completed";
            result.success = true;
            result.firstBlocker = string.Empty;
            result.message = "Town 更深 anchor 的 resident/backstage/director-ready scene-side 承接层与 HomeAnchor 配置位已站住，可继续给 day1 消费更深 resident scene-side。";
        }

        private static void AddBlocker(ProbeResult result, string blockerCode, string message)
        {
            if (string.IsNullOrWhiteSpace(result.firstBlocker))
            {
                result.firstBlocker = blockerCode;
            }

            result.blockingFindings.Add(message);
        }

        private static float CalculateDistance(Transform anchor, Transform slot)
        {
            if (anchor == null || slot == null)
            {
                return -1f;
            }

            return Vector2.Distance(anchor.position, slot.position);
        }

        private static Transform FindHomeAnchor(Transform slot)
        {
            if (slot == null)
            {
                return null;
            }

            for (int index = 0; index < slot.childCount; index++)
            {
                Transform child = slot.GetChild(index);
                if (child == null)
                {
                    continue;
                }

                if (string.Equals(child.name, "HomeAnchor", StringComparison.OrdinalIgnoreCase)
                    || child.name.EndsWith("_HomeAnchor", StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }
            }

            return null;
        }

        private static PolygonCollider2D FindBoundsCollider(Dictionary<string, Transform> lookup)
        {
            if (lookup.TryGetValue("_CameraBounds", out Transform boundsRoot) && boundsRoot != null)
            {
                return boundsRoot.GetComponent<PolygonCollider2D>();
            }

            return null;
        }

        private static Dictionary<string, Transform> BuildTransformLookup(Scene scene)
        {
            Dictionary<string, Transform> lookup = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                CollectTransforms(roots[index].transform, lookup);
            }

            return lookup;
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

        private static bool Contains(Bounds bounds, Vector3 position)
        {
            return position.x >= bounds.min.x &&
                   position.x <= bounds.max.x &&
                   position.y >= bounds.min.y &&
                   position.y <= bounds.max.y;
        }

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            Stack<string> stack = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                stack.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", stack.ToArray());
        }

        private static string FormatVector(Vector3 value)
        {
            return $"{value.x:F3},{value.y:F3},{value.z:F3}";
        }
    }
}
#endif
