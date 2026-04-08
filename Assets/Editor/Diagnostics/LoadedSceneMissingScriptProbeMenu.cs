#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.Diagnostics
{
    internal static class LoadedSceneMissingScriptProbeMenu
    {
        private const string MenuPath = "Tools/Sunset/Diagnostics/Write Loaded Missing Script Probe";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, "loaded-missing-script-probe.json");

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public bool isPlaying;
            public int totalObjectsScanned;
            public int totalMissingComponents;
            public List<Entry> entries = new List<Entry>();
        }

        [Serializable]
        private sealed class Entry
        {
            public string scene = string.Empty;
            public string path = string.Empty;
            public int missingCount;
        }

        [MenuItem(MenuPath)]
        private static void Run()
        {
            Directory.CreateDirectory(CommandRoot);

            ProbeResult result = new ProbeResult
            {
                timestamp = DateTime.Now.ToString("O"),
                isPlaying = EditorApplication.isPlaying
            };

            HashSet<int> visited = new HashSet<int>();

            int sceneCount = SceneManager.sceneCount;
            for (int sceneIndex = 0; sceneIndex < sceneCount; sceneIndex++)
            {
                Scene scene = SceneManager.GetSceneAt(sceneIndex);
                if (!scene.IsValid() || !scene.isLoaded)
                {
                    continue;
                }

                GameObject[] roots = scene.GetRootGameObjects();
                for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
                {
                    ScanRecursive(roots[rootIndex], scene.path, result, visited);
                }
            }

            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            for (int objectIndex = 0; objectIndex < allObjects.Length; objectIndex++)
            {
                GameObject candidate = allObjects[objectIndex];
                if (candidate == null)
                {
                    continue;
                }

                if (!string.Equals(candidate.scene.name, "DontDestroyOnLoad", StringComparison.Ordinal))
                {
                    continue;
                }

                if (candidate.transform.parent != null)
                {
                    continue;
                }

                ScanRecursive(candidate, "DontDestroyOnLoad", result, visited);
            }

            File.WriteAllText(ResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[LoadedMissingScriptProbe] missing={result.totalMissingComponents}, objects={result.entries.Count}, output={ResultPath}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRun()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static void ScanRecursive(
            GameObject current,
            string sceneLabel,
            ProbeResult result,
            HashSet<int> visited)
        {
            if (current == null)
            {
                return;
            }

            if (!visited.Add(current.GetInstanceID()))
            {
                return;
            }

            result.totalObjectsScanned++;

            int missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(current);
            if (missingCount > 0)
            {
                result.totalMissingComponents += missingCount;
                result.entries.Add(new Entry
                {
                    scene = sceneLabel,
                    path = GetTransformPath(current.transform),
                    missingCount = missingCount
                });
            }

            Transform transform = current.transform;
            for (int childIndex = 0; childIndex < transform.childCount; childIndex++)
            {
                ScanRecursive(transform.GetChild(childIndex).gameObject, sceneLabel, result, visited);
            }
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
    }
}
#endif
