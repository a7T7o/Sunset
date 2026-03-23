using System.Linq;
using Sunset.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.Story
{
    [InitializeOnLoad]
    public static class SpringDay1WorkbenchSceneBinder
    {
        private static readonly string[] CandidateNames = { "Anvil_0", "Workbench", "Anvil" };
        private const string TargetSceneName = "Primary";

        static SpringDay1WorkbenchSceneBinder()
        {
            EditorApplication.delayCall += TryBindWorkbench;
            EditorApplication.hierarchyChanged += HandleHierarchyChanged;
        }

        private static void HandleHierarchyChanged()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {
                return;
            }

            EditorApplication.delayCall -= TryBindWorkbench;
            EditorApplication.delayCall += TryBindWorkbench;
        }

        private static void TryBindWorkbench()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {
                return;
            }

            Scene activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != TargetSceneName)
            {
                return;
            }

            Transform[] roots = activeScene.GetRootGameObjects()
                .SelectMany(root => root.GetComponentsInChildren<Transform>(true))
                .ToArray();

            for (int index = 0; index < roots.Length; index++)
            {
                Transform candidate = roots[index];
                if (!IsWorkbenchCandidate(candidate))
                {
                    continue;
                }

                if (candidate.GetComponent<Collider2D>() == null)
                {
                    continue;
                }

                CraftingStationInteractable interactable = candidate.GetComponent<CraftingStationInteractable>();
                bool created = false;
                if (interactable == null)
                {
                    interactable = Undo.AddComponent<CraftingStationInteractable>(candidate.gameObject);
                    created = true;
                    interactable.ConfigureRuntimeDefaults(FarmGame.Data.CraftingStation.Workbench, "使用工作台", 1.8f, 28);
                    EditorUtility.SetDirty(interactable);
                }

                if (created)
                {
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    Debug.Log($"[SpringDay1WorkbenchSceneBinder] 已为 {candidate.name} 自动补挂 CraftingStationInteractable。");
                }
            }
        }

        private static bool IsWorkbenchCandidate(Transform candidate)
        {
            if (candidate == null)
            {
                return false;
            }

            for (int index = 0; index < CandidateNames.Length; index++)
            {
                if (candidate.name == CandidateNames[index])
                {
                    return true;
                }
            }

            string loweredName = candidate.name.ToLowerInvariant();
            return loweredName.Contains("anvil") || loweredName.Contains("workbench");
        }
    }
}
