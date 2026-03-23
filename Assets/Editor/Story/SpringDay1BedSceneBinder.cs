using System.Linq;
using Sunset.Story;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.Story
{
    [InitializeOnLoad]
    public static class SpringDay1BedSceneBinder
    {
        private static readonly string[] CandidateNames = { "Bed", "PlayerBed", "HomeBed" };
        private const string TargetSceneName = "Primary";

        static SpringDay1BedSceneBinder()
        {
            EditorApplication.delayCall += TryBindBed;
            EditorApplication.hierarchyChanged += HandleHierarchyChanged;
        }

        private static void HandleHierarchyChanged()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling)
            {
                return;
            }

            EditorApplication.delayCall -= TryBindBed;
            EditorApplication.delayCall += TryBindBed;
        }

        private static void TryBindBed()
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
                if (!IsBedCandidate(candidate) || candidate.GetComponent<Collider2D>() == null)
                {
                    continue;
                }

                SpringDay1BedInteractable interactable = candidate.GetComponent<SpringDay1BedInteractable>();
                bool created = false;
                if (interactable == null)
                {
                    interactable = Undo.AddComponent<SpringDay1BedInteractable>(candidate.gameObject);
                    created = true;
                    interactable.ConfigureRuntimeDefaults("睡觉", 1.6f, 24);
                    EditorUtility.SetDirty(interactable);
                }

                if (created)
                {
                    EditorSceneManager.MarkSceneDirty(activeScene);
                    Debug.Log($"[SpringDay1BedSceneBinder] 已为 {candidate.name} 自动补挂 SpringDay1BedInteractable。");
                }
            }
        }

        private static bool IsBedCandidate(Transform candidate)
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

            return candidate.name.ToLowerInvariant().Contains("bed");
        }
    }
}
