#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using Sunset.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story.Editor
{
    internal static class SpringDay1ActorRuntimeProbeMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Write Spring Day1 Actor Runtime Probe Artifact";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string OutputPath = Path.Combine(CommandRoot, "spring-day1-actor-runtime-probe.json");
        private static readonly string[] TrackedNpcIds = { "001", "002", "003", "101", "102", "103", "104", "201", "202", "203" };
        private static readonly string[] KnownNpcRootNames =
        {
            "NPCs",
            "Resident_DefaultPresent",
            "Resident_DirectorTakeoverReady",
            "Resident_BackstagePresent",
            "SpringDay1NpcCrowdRuntimeRoot"
        };
        private static readonly Regex StrictNpcIdTokenRegex = new Regex(@"^(?:NPC_?)?(?<id>\d{1,3})$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        [MenuItem(MenuPath)]
        private static void WriteProbeArtifact()
        {
            Directory.CreateDirectory(CommandRoot);

            ProbeArtifact artifact = new ProbeArtifact
            {
                timestamp = DateTime.Now.ToString("O"),
                isPlaying = Application.isPlaying,
                activeScenePath = SceneManager.GetActiveScene().path,
                activeSceneName = SceneManager.GetActiveScene().name,
                loadedScenes = CaptureLoadedScenes()
            };

            if (!Application.isPlaying)
            {
                artifact.status = "blocked";
                artifact.message = "not-in-playmode";
                Persist(artifact);
                return;
            }

            StoryManager storyManager = StoryManager.Instance;
            SpringDay1Director director = SpringDay1Director.Instance;
            TimeManager timeManager = TimeManager.Instance;

            artifact.status = "completed";
            artifact.message = "ok";
            artifact.phase = storyManager != null ? storyManager.CurrentPhase.ToString() : string.Empty;
            artifact.beatKey = director != null ? director.GetCurrentBeatKey() : string.Empty;
            artifact.clock = timeManager != null ? timeManager.GetFormattedTime() : string.Empty;
            artifact.crowdSummary = SpringDay1NpcCrowdDirector.CurrentRuntimeSummary;
            artifact.residentProbeEntries = SpringDay1NpcCrowdDirector.CaptureResidentControlProbe().ToArray();
            artifact.trackedActors = CaptureTrackedActors();

            Persist(artifact);
            Debug.Log($"[SpringDay1ActorRuntimeProbe] 已写出 {Path.GetFileName(OutputPath)}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateWriteProbeArtifact()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static LoadedSceneEntry[] CaptureLoadedScenes()
        {
            List<LoadedSceneEntry> entries = new List<LoadedSceneEntry>(SceneManager.sceneCount);
            for (int index = 0; index < SceneManager.sceneCount; index++)
            {
                Scene scene = SceneManager.GetSceneAt(index);
                entries.Add(new LoadedSceneEntry
                {
                    name = scene.name,
                    path = scene.path,
                    isLoaded = scene.isLoaded,
                    isValid = scene.IsValid(),
                    rootCount = scene.rootCount
                });
            }

            return entries.ToArray();
        }

        private static TrackedActorEntry[] CaptureTrackedActors()
        {
            Dictionary<string, List<TrackedActorEntry>> entriesByNpcId = new Dictionary<string, List<TrackedActorEntry>>(StringComparer.OrdinalIgnoreCase);
            foreach (string trackedNpcId in TrackedNpcIds)
            {
                entriesByNpcId[trackedNpcId] = new List<TrackedActorEntry>();
            }

            NPCAutoRoamController[] roamControllers = UnityEngine.Object.FindObjectsByType<NPCAutoRoamController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (int index = 0; index < roamControllers.Length; index++)
            {
                NPCAutoRoamController roamController = roamControllers[index];
                if (roamController == null || roamController.gameObject == null)
                {
                    continue;
                }

                string trackedNpcId = ResolveTrackedNpcId(roamController.gameObject, roamController);
                if (string.IsNullOrEmpty(trackedNpcId))
                {
                    continue;
                }

                entriesByNpcId[trackedNpcId].Add(BuildTrackedActorEntry(trackedNpcId, roamController.gameObject, roamController));
            }

            List<TrackedActorEntry> flattenedEntries = new List<TrackedActorEntry>();
            foreach (string trackedNpcId in TrackedNpcIds)
            {
                List<TrackedActorEntry> matchedEntries = entriesByNpcId[trackedNpcId];
                if (matchedEntries.Count <= 0)
                {
                    flattenedEntries.Add(new TrackedActorEntry
                    {
                        npcId = trackedNpcId,
                        present = false
                    });
                    continue;
                }

                flattenedEntries.AddRange(matchedEntries.OrderBy(entry => entry.sceneName, StringComparer.OrdinalIgnoreCase));
            }

            return flattenedEntries.ToArray();
        }

        private static string ResolveTrackedNpcId(GameObject instance, NPCAutoRoamController roamController)
        {
            string objectNameKey = NormalizeNpcIdToken(instance != null ? instance.name : string.Empty);
            if (!string.IsNullOrEmpty(objectNameKey))
            {
                return objectNameKey;
            }

            string stableKey = roamController != null ? NormalizeNpcIdToken(roamController.ResidentStableKey) : string.Empty;
            if (!string.IsNullOrEmpty(stableKey) && IsKnownTrackedNpcRoot(instance != null ? instance.transform : null))
            {
                return stableKey;
            }

            return string.Empty;
        }

        private static bool IsKnownTrackedNpcRoot(Transform transform)
        {
            Transform current = transform;
            while (current != null)
            {
                for (int index = 0; index < KnownNpcRootNames.Length; index++)
                {
                    if (string.Equals(current.name, KnownNpcRootNames[index], StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                current = current.parent;
            }

            return false;
        }

        private static string NormalizeNpcIdToken(string rawValue)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return string.Empty;
            }

            string normalized = rawValue.Trim();
            int cloneMarker = normalized.IndexOf("(Clone)", StringComparison.OrdinalIgnoreCase);
            if (cloneMarker >= 0)
            {
                normalized = normalized.Substring(0, cloneMarker).Trim();
            }

            Match match = StrictNpcIdTokenRegex.Match(normalized);
            if (!match.Success)
            {
                return string.Empty;
            }

            normalized = match.Groups["id"].Value.PadLeft(3, '0');

            return TrackedNpcIds.Contains(normalized, StringComparer.OrdinalIgnoreCase)
                ? normalized
                : string.Empty;
        }

        private static TrackedActorEntry BuildTrackedActorEntry(string trackedNpcId, GameObject instance, NPCAutoRoamController roamController)
        {
            NPCMotionController motionController = instance != null ? instance.GetComponent<NPCMotionController>() : null;
            string sceneName = instance != null && instance.scene.IsValid() ? instance.scene.name : string.Empty;
            string scenePath = instance != null && instance.scene.IsValid() ? instance.scene.path : string.Empty;
            string hierarchyPath = BuildHierarchyPath(instance != null ? instance.transform : null);

            return new TrackedActorEntry
            {
                npcId = trackedNpcId,
                present = instance != null,
                objectName = instance != null ? instance.name : string.Empty,
                sceneName = sceneName,
                scenePath = scenePath,
                hierarchyPath = hierarchyPath,
                activeSelf = instance != null && instance.activeSelf,
                activeInHierarchy = instance != null && instance.activeInHierarchy,
                positionX = instance != null ? instance.transform.position.x : 0f,
                positionY = instance != null ? instance.transform.position.y : 0f,
                positionZ = instance != null ? instance.transform.position.z : 0f,
                roamControllerEnabled = roamController != null && roamController.enabled,
                isRoaming = roamController != null && roamController.IsRoaming,
                isMoving = roamController != null && roamController.IsMoving,
                roamState = roamController != null ? roamController.DebugState : string.Empty,
                lastMoveSkipReason = roamController != null ? roamController.DebugLastMoveSkipReason : string.Empty,
                blockedAdvanceFrames = roamController != null ? roamController.DebugBlockedAdvanceFrames : 0,
                consecutivePathBuildFailures = roamController != null ? roamController.DebugConsecutivePathBuildFailures : 0,
                scriptedControlActive = roamController != null && roamController.IsResidentScriptedControlActive,
                scriptedControlOwnerKey = roamController != null ? roamController.ResidentScriptedControlOwnerKey : string.Empty,
                scriptedMoveActive = roamController != null && roamController.IsResidentScriptedMoveActive,
                scriptedMovePaused = roamController != null && roamController.IsResidentScriptedMovePaused,
                resumeRoamWhenResidentControlReleases = roamController != null && roamController.ResumeRoamWhenResidentControlReleases,
                sharedAvoidanceBlockingFrames = roamController != null ? roamController.DebugSharedAvoidanceBlockingFrames : 0,
                hasSharedAvoidanceDetour = roamController != null && roamController.DebugHasSharedAvoidanceDetour
            };
        }

        private static string BuildHierarchyPath(Transform transform)
        {
            if (transform == null)
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder(transform.name);
            Transform current = transform.parent;
            while (current != null)
            {
                builder.Insert(0, current.name + "/");
                current = current.parent;
            }

            return builder.ToString();
        }

        private static void Persist(ProbeArtifact artifact)
        {
            File.WriteAllText(OutputPath, JsonUtility.ToJson(artifact, true), new UTF8Encoding(false));
        }

        [Serializable]
        private sealed class ProbeArtifact
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public string message = string.Empty;
            public bool isPlaying;
            public string activeScenePath = string.Empty;
            public string activeSceneName = string.Empty;
            public string phase = string.Empty;
            public string beatKey = string.Empty;
            public string clock = string.Empty;
            public string crowdSummary = string.Empty;
            public LoadedSceneEntry[] loadedScenes = Array.Empty<LoadedSceneEntry>();
            public SpringDay1NpcCrowdDirector.ResidentControlProbeEntry[] residentProbeEntries = Array.Empty<SpringDay1NpcCrowdDirector.ResidentControlProbeEntry>();
            public TrackedActorEntry[] trackedActors = Array.Empty<TrackedActorEntry>();
        }

        [Serializable]
        private sealed class LoadedSceneEntry
        {
            public string name = string.Empty;
            public string path = string.Empty;
            public bool isLoaded;
            public bool isValid;
            public int rootCount;
        }

        [Serializable]
        private sealed class TrackedActorEntry
        {
            public string npcId = string.Empty;
            public bool present;
            public string objectName = string.Empty;
            public string sceneName = string.Empty;
            public string scenePath = string.Empty;
            public string hierarchyPath = string.Empty;
            public bool activeSelf;
            public bool activeInHierarchy;
            public float positionX;
            public float positionY;
            public float positionZ;
            public bool roamControllerEnabled;
            public bool isRoaming;
            public bool isMoving;
            public string roamState = string.Empty;
            public string lastMoveSkipReason = string.Empty;
            public int blockedAdvanceFrames;
            public int consecutivePathBuildFailures;
            public bool scriptedControlActive;
            public string scriptedControlOwnerKey = string.Empty;
            public bool scriptedMoveActive;
            public bool scriptedMovePaused;
            public bool resumeRoamWhenResidentControlReleases;
            public int sharedAvoidanceBlockingFrames;
            public bool hasSharedAvoidanceDetour;
        }
    }
}
#endif
