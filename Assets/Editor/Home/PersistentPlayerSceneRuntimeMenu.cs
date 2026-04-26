#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Sunset.Story;
using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.EditorTools.SceneSync
{
    internal static class PersistentPlayerSceneRuntimeMenu
    {
        private const string TriggerPrimaryHomeDoorMenuPath = "Tools/Sunset/Runtime/Trigger Primary Home Door";
        private const string TriggerHomeDoorMenuPath = "Tools/Sunset/Runtime/Trigger Home Door";
        private const string ProbeMenuPath = "Tools/Sunset/Runtime/Write Persistent Player Scene Probe";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ProbeResultPath = Path.Combine(CommandRoot, "persistent-player-scene-probe.json");

        [Serializable]
        private sealed class ProbeResult
        {
            public string timestamp = string.Empty;
            public string activeScene = string.Empty;
            public bool hasPersistentPlayerBridge;
            public bool hasGameInputManagerInActiveScene;
            public bool hasCinemachineCameraInActiveScene;
            public int totalPlayerCount;
            public int activeScenePlayerCount;
            public int dontDestroyOnLoadPlayerCount;
            public string persistentPlayerScene = string.Empty;
            public string persistentPlayerPath = string.Empty;
            public string persistentPlayerPosition = string.Empty;
        }

        [MenuItem(TriggerPrimaryHomeDoorMenuPath)]
        private static void TriggerPrimaryHomeDoor()
        {
            TriggerTransition("PrimaryHomeDoor");
        }

        [MenuItem(TriggerHomeDoorMenuPath)]
        private static void TriggerHomeDoor()
        {
            TriggerTransition("HomeDoor");
        }

        [MenuItem(ProbeMenuPath)]
        private static void WriteProbe()
        {
            Directory.CreateDirectory(CommandRoot);

            Scene activeScene = SceneManager.GetActiveScene();
            ProbeResult result = new ProbeResult
            {
                timestamp = DateTime.Now.ToString("O"),
                activeScene = activeScene.IsValid() ? activeScene.path : string.Empty,
                hasPersistentPlayerBridge = GameObject.Find("_PersistentPlayerSceneBridge") != null,
                hasGameInputManagerInActiveScene = FindFirstComponentInScene<GameInputManager>(activeScene) != null,
                hasCinemachineCameraInActiveScene = FindFirstComponentInScene<CinemachineCamera>(activeScene) != null
            };

            PlayerMovement[] players = UnityEngine.Object.FindObjectsByType<PlayerMovement>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            result.totalPlayerCount = players.Length;
            for (int index = 0; index < players.Length; index++)
            {
                PlayerMovement player = players[index];
                if (player == null)
                {
                    continue;
                }

                Scene playerScene = player.gameObject.scene;
                if (playerScene == activeScene)
                {
                    result.activeScenePlayerCount++;
                }

                if (string.Equals(playerScene.name, "DontDestroyOnLoad", StringComparison.Ordinal))
                {
                    result.dontDestroyOnLoadPlayerCount++;
                    if (string.IsNullOrWhiteSpace(result.persistentPlayerPath))
                    {
                        result.persistentPlayerScene = playerScene.name;
                        result.persistentPlayerPath = GetTransformPath(player.transform);
                        result.persistentPlayerPosition = FormatVector(player.transform.position);
                    }
                }
            }

            File.WriteAllText(ProbeResultPath, JsonUtility.ToJson(result, true), new UTF8Encoding(false));
            AssetDatabase.Refresh();
            Debug.Log($"[PersistentPlayerSceneRuntimeMenu] 已写出 probe: {ProbeResultPath}");
        }

        [MenuItem(TriggerPrimaryHomeDoorMenuPath, true)]
        [MenuItem(TriggerHomeDoorMenuPath, true)]
        [MenuItem(ProbeMenuPath, true)]
        private static bool ValidateRuntimeMenu()
        {
            return EditorApplication.isPlaying && !EditorApplication.isPaused;
        }

        private static void TriggerTransition(string triggerName)
        {
            SceneTransitionTrigger2D[] triggers = UnityEngine.Object.FindObjectsByType<SceneTransitionTrigger2D>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < triggers.Length; index++)
            {
                SceneTransitionTrigger2D trigger = triggers[index];
                if (trigger == null || !string.Equals(trigger.gameObject.name, triggerName, StringComparison.Ordinal))
                {
                    continue;
                }

                bool started = trigger.TryStartTransition();
                Debug.Log($"[PersistentPlayerSceneRuntimeMenu] {triggerName} TryStartTransition => {started}");
                return;
            }

            Debug.LogWarning($"[PersistentPlayerSceneRuntimeMenu] 未找到 {triggerName}，当前无法触发切场。");
        }

        private static T FindFirstComponentInScene<T>(Scene scene) where T : Component
        {
            T[] components = UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < components.Length; index++)
            {
                T component = components[index];
                if (component != null && component.gameObject.scene == scene)
                {
                    return component;
                }
            }

            return null;
        }

        private static string GetTransformPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            string path = transform.name;
            Transform current = transform.parent;
            while (current != null)
            {
                path = current.name + "/" + path;
                current = current.parent;
            }

            return path;
        }

        private static string FormatVector(Vector3 value)
        {
            return $"{value.x:F3},{value.y:F3},{value.z:F3}";
        }
    }
}
#endif
