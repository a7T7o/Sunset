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
    internal static class TownNativeResidentMigrationMenu
    {
        private const string MenuPath = "Tools/Sunset/Scene/Migrate Town Native Residents";
        private const string TownScenePath = "Assets/000_Scenes/Town.unity";
        private const string PrimaryScenePath = "Assets/000_Scenes/Primary.unity";
        private const string CrowdManifestPath = "Assets/Resources/Story/SpringDay1/SpringDay1NpcCrowdManifest.asset";
        private const string TownResidentRootName = "";
        private const string TownResidentDefaultRootName = "";
        private const string TownResidentTakeoverRootName = "";
        private const string TownResidentBackstageRootName = "";
        private const string TownStoryNpcRootName = "NPCs";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "town-native-resident-migration.json");

        private static readonly Dictionary<string, string> SemanticHomeAnchorNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly StoryNpcMigrationPlan[] StoryNpcPlans =
        {
            new StoryNpcMigrationPlan("001", "Assets/222_Prefabs/NPC/001.prefab"),
            new StoryNpcMigrationPlan("002", "Assets/222_Prefabs/NPC/002.prefab")
        };

        [Serializable]
        private sealed class MigrationReport
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public bool success;
            public string message = string.Empty;
            public string firstBlocker = string.Empty;
            public string townScenePath = TownScenePath;
            public string primaryScenePath = PrimaryScenePath;
            public string manifestPath = CrowdManifestPath;
            public List<ResidentRecord> townCrowdResidents = new List<ResidentRecord>();
            public List<ResidentRecord> townStoryResidents = new List<ResidentRecord>();
            public List<string> primaryRemoved = new List<string>();
            public List<string> attentionFindings = new List<string>();
            public List<string> blockingFindings = new List<string>();
        }

        [Serializable]
        private sealed class ResidentRecord
        {
            public string npcId = string.Empty;
            public string sceneObjectName = string.Empty;
            public string parentPath = string.Empty;
            public string position = string.Empty;
            public string homeAnchorName = string.Empty;
            public string homeAnchorPath = string.Empty;
            public string homeAnchorPosition = string.Empty;
            public string sourceAnchorName = string.Empty;
            public bool createdResident;
            public bool createdHomeAnchor;
        }

        private readonly struct StoryNpcMigrationPlan
        {
            public StoryNpcMigrationPlan(string npcId, string prefabPath)
            {
                NpcId = npcId;
                PrefabPath = prefabPath;
            }

            public string NpcId { get; }
            public string PrefabPath { get; }
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);
            MigrationReport report = Migrate();
            File.WriteAllText(ResultPath, JsonUtility.ToJson(report, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[TownNativeResidentMigration] {report.message}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static MigrationReport Migrate()
        {
            MigrationReport report = new MigrationReport
            {
                timestamp = DateTime.Now.ToString("O")
            };

            AddBlocker(
                report,
                "deprecated-town-native-resident-migration",
                "旧 Town native resident migration 已退役：它会把 Town_Day1Residents/旧 semantic anchor/旧 baseline root 回写进 scene。后续不要再用这条旧迁移链。");
            FinalizeReport(report);
            return report;
        }

        private static bool TryResolveCrowdBasePosition(
            SpringDay1NpcCrowdManifest.Entry entry,
            Dictionary<string, Transform> townLookup,
            out Vector3 basePosition,
            out string sourceAnchorName)
        {
            basePosition = Vector3.zero;
            sourceAnchorName = string.Empty;
            if (entry == null)
            {
                return false;
            }

            string[] semanticAnchors = entry.semanticAnchorIds ?? Array.Empty<string>();
            for (int index = 0; index < semanticAnchors.Length; index++)
            {
                string semanticAnchorId = semanticAnchors[index];
                if (string.IsNullOrWhiteSpace(semanticAnchorId))
                {
                    continue;
                }

                if (SemanticHomeAnchorNames.TryGetValue(semanticAnchorId.Trim(), out string homeAnchorName)
                    && townLookup.TryGetValue(homeAnchorName, out Transform mappedHomeAnchor))
                {
                    basePosition = mappedHomeAnchor.position + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
                    sourceAnchorName = mappedHomeAnchor.name;
                    return true;
                }

                if (townLookup.TryGetValue(semanticAnchorId.Trim(), out Transform semanticAnchor))
                {
                    basePosition = semanticAnchor.position + new Vector3(entry.spawnOffset.x, entry.spawnOffset.y, 0f);
                    sourceAnchorName = semanticAnchor.name;
                    return true;
                }
            }

            return false;
        }

        private static bool TryResolveStoryNpcSource(
            Dictionary<string, Transform> primaryLookup,
            string npcId,
            out Vector3 sourcePosition,
            out string sourceAnchorName)
        {
            sourcePosition = Vector3.zero;
            sourceAnchorName = string.Empty;

            if (primaryLookup.TryGetValue($"{npcId}_HomeAnchor", out Transform homeAnchor))
            {
                sourcePosition = homeAnchor.position;
                sourceAnchorName = homeAnchor.name;
                return true;
            }

            if (primaryLookup.TryGetValue(npcId, out Transform resident))
            {
                sourcePosition = resident.position;
                sourceAnchorName = resident.name;
                return true;
            }

            return false;
        }

        private static Transform ResolveResidentRoot(
            SpringDay1NpcCrowdManifest.Entry entry,
            Transform residentDefaultRoot,
            Transform residentTakeoverRoot,
            Transform residentBackstageRoot)
        {
            if (entry == null)
            {
                return residentDefaultRoot;
            }

            return entry.residentBaseline switch
            {
                SpringDay1CrowdResidentBaseline.PeripheralResident => residentTakeoverRoot,
                SpringDay1CrowdResidentBaseline.NightResident => residentBackstageRoot,
                _ => residentDefaultRoot
            };
        }

        private static void BindRoamControllerToTownScene(
            NPCAutoRoamController roamController,
            Transform homeAnchor,
            NavGrid2D townNavGrid)
        {
            if (roamController == null)
            {
                return;
            }

            roamController.BindResidentHomeAnchor(homeAnchor);
            roamController.SetNavGrid(townNavGrid);
            roamController.SetTraversalSoftPassBlockers(Array.Empty<Collider2D>(), enabled: false);
            EditorUtility.SetDirty(roamController);
        }

        private static Transform RequireTransform(Scene scene, string name, MigrationReport report, string blockerCode)
        {
            Transform transform = FindSceneObject(scene, name)?.transform;
            if (transform == null)
            {
                AddBlocker(report, blockerCode, $"Town 当前缺少 {name}，不能继续把原生 resident 落进指定 resident root。");
            }

            return transform;
        }

        private static bool TryOpenScene(string scenePath, out Scene scene, out bool openedTemporarily, out string error)
        {
            scene = SceneManager.GetSceneByPath(scenePath);
            openedTemporarily = false;
            error = string.Empty;

            if (scene.IsValid() && scene.isLoaded)
            {
                return true;
            }

            try
            {
                scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                openedTemporarily = true;
                return scene.IsValid() && scene.isLoaded;
            }
            catch (Exception exception)
            {
                error = $"{scenePath} 打开失败：{exception.GetType().Name}: {exception.Message}";
                return false;
            }
        }

        private static Dictionary<string, Transform> BuildLookup(Scene scene)
        {
            Dictionary<string, Transform> lookup = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return lookup;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                AppendLookup(roots[index].transform, lookup);
            }

            return lookup;
        }

        private static void AppendLookup(Transform transform, Dictionary<string, Transform> lookup)
        {
            if (transform == null)
            {
                return;
            }

            if (!lookup.ContainsKey(transform.name))
            {
                lookup.Add(transform.name, transform);
            }

            for (int index = 0; index < transform.childCount; index++)
            {
                AppendLookup(transform.GetChild(index), lookup);
            }
        }

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            if (!scene.IsValid() || !scene.isLoaded)
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                T component = roots[index].GetComponentInChildren<T>(true);
                if (component != null)
                {
                    return component;
                }
            }

            return null;
        }

        private static GameObject FindSceneObject(Scene scene, string objectName)
        {
            if (!scene.IsValid() || string.IsNullOrWhiteSpace(objectName))
            {
                return null;
            }

            GameObject[] roots = scene.GetRootGameObjects();
            for (int index = 0; index < roots.Length; index++)
            {
                Transform root = roots[index].transform;
                if (string.Equals(root.name, objectName, StringComparison.OrdinalIgnoreCase))
                {
                    return root.gameObject;
                }

                Transform nested = FindChildRecursive(root, objectName);
                if (nested != null)
                {
                    return nested.gameObject;
                }
            }

            return null;
        }

        private static Transform FindChildRecursive(Transform root, string targetName)
        {
            if (root == null)
            {
                return null;
            }

            for (int index = 0; index < root.childCount; index++)
            {
                Transform child = root.GetChild(index);
                if (string.Equals(child.name, targetName, StringComparison.OrdinalIgnoreCase))
                {
                    return child;
                }

                Transform nested = FindChildRecursive(child, targetName);
                if (nested != null)
                {
                    return nested;
                }
            }

            return null;
        }

        private static Transform FindOrCreateSceneRoot(Scene scene, string rootName)
        {
            GameObject existing = FindSceneObject(scene, rootName);
            if (existing != null)
            {
                return existing.transform;
            }

            GameObject created = new GameObject(rootName);
            SceneManager.MoveGameObjectToScene(created, scene);
            return created.transform;
        }

        private static void AddBlocker(MigrationReport report, string blockerCode, string message)
        {
            if (report == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(report.firstBlocker))
            {
                report.firstBlocker = blockerCode;
            }

            report.blockingFindings.Add(message);
        }

        private static void FinalizeReport(MigrationReport report)
        {
            if (report == null)
            {
                return;
            }

            bool hasBlockingFindings = report.blockingFindings.Count > 0;
            report.success = !hasBlockingFindings;
            report.status = hasBlockingFindings ? "blocked" : "completed";
            report.message = hasBlockingFindings
                ? report.blockingFindings[0]
                : $"Town 原生 resident 已迁入 crowd={report.townCrowdResidents.Count}，story={report.townStoryResidents.Count}；Primary 清掉 {report.primaryRemoved.Count} 个旧对象。";
        }

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            Stack<string> names = new Stack<string>();
            Transform current = transform;
            while (current != null)
            {
                names.Push(current.name);
                current = current.parent;
            }

            return string.Join("/", names);
        }

        private static string FormatVector(Vector3 vector)
        {
            return $"{vector.x:F3},{vector.y:F3},{vector.z:F3}";
        }
    }
}
#endif
