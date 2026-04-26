#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using Sunset.Story;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story.Editor
{
    internal static class SpringDay1LiveSnapshotArtifactMenu
    {
        private const string SnapshotMenuPath = "Sunset/Story/Validation/Write Spring Day1 Live Snapshot Artifact";
        private const string StepMenuPath = "Sunset/Story/Validation/Trigger Spring Day1 Recommended Action Artifact";
        private const string TownLeadTransitionMenuPath = "Sunset/Story/Validation/Request Spring Day1 Town Lead Transition Artifact";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string SnapshotPath = Path.Combine(CommandRoot, "spring-day1-live-snapshot.json");

        [MenuItem(SnapshotMenuPath)]
        private static void WriteSnapshotArtifact()
        {
            WriteArtifact(triggerRecommendedAction: false);
        }

        [MenuItem(StepMenuPath)]
        private static void TriggerRecommendedActionArtifact()
        {
            WriteArtifact(triggerRecommendedAction: true);
        }

        [MenuItem(TownLeadTransitionMenuPath)]
        private static void RequestTownLeadTransitionArtifact()
        {
            WriteArtifact(
                triggerRecommendedAction: false,
                actionOverride: TryRequestTownLeadTransition());
        }

        [MenuItem(SnapshotMenuPath, true)]
        [MenuItem(StepMenuPath, true)]
        [MenuItem(TownLeadTransitionMenuPath, true)]
        private static bool ValidateMenus()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static void WriteArtifact(bool triggerRecommendedAction, string actionOverride = null)
        {
            Directory.CreateDirectory(CommandRoot);

            SnapshotArtifact artifact = new SnapshotArtifact
            {
                timestamp = DateTime.Now.ToString("O"),
                activeScene = SceneManager.GetActiveScene().path,
                isPlaying = Application.isPlaying
            };

            if (!Application.isPlaying)
            {
                artifact.status = "blocked";
                artifact.message = "not-in-playmode";
                Persist(artifact);
                Debug.LogWarning("[SpringDay1LiveSnapshotArtifact] 当前不在 PlayMode，未写 live 快照。");
                return;
            }

            SpringDay1LiveValidationRunner runner = SpringDay1LiveValidationRunner.EnsureRuntime();
            artifact.bootstrap = runner.BootstrapRuntime();
            if (!string.IsNullOrWhiteSpace(actionOverride))
            {
                artifact.actionResult = actionOverride;
            }
            else if (triggerRecommendedAction)
            {
                artifact.actionResult = runner.TriggerRecommendedAction();
            }

            artifact.snapshot = runner.BuildSnapshot(triggerRecommendedAction ? "step-artifact" : "snapshot-artifact");
            artifact.nextAction = runner.GetRecommendedNextAction();
            artifact.status = "completed";
            artifact.message = "ok";
            Persist(artifact);
            Debug.Log($"[SpringDay1LiveSnapshotArtifact] 已写出 {Path.GetFileName(SnapshotPath)}");
        }

        private static string TryRequestTownLeadTransition()
        {
            if (!Application.isPlaying)
            {
                return "not-in-playmode";
            }

            SpringDay1Director director = SpringDay1Director.Instance ?? UnityEngine.Object.FindFirstObjectByType<SpringDay1Director>(FindObjectsInactive.Include);
            if (director != null)
            {
                string directorResult = director.TryRequestValidationEscortTransition();
                if (!string.Equals(directorResult, "当前没有待请求的 escort 转场。", StringComparison.Ordinal))
                {
                    return directorResult;
                }
            }

            string targetSceneName = ResolveFallbackTargetSceneName();
            SceneTransitionTrigger2D trigger = ResolveTransitionTrigger(targetSceneName);
            if (trigger == null)
            {
                return "town-transition-trigger-missing";
            }

            return trigger.TryStartTransition()
                ? $"transition-requested:{trigger.name}"
                : $"transition-request-failed:{trigger.name}";
        }

        private static string ResolveFallbackTargetSceneName()
        {
            Scene activeScene = SceneManager.GetActiveScene();
            if (string.Equals(activeScene.name, "Town", StringComparison.Ordinal))
            {
                return "Primary";
            }

            if (string.Equals(activeScene.name, "Primary", StringComparison.Ordinal))
            {
                return "Town";
            }

            return string.Empty;
        }

        private static SceneTransitionTrigger2D ResolveTransitionTrigger(string targetSceneName)
        {
            SceneTransitionTrigger2D[] triggers = UnityEngine.Object.FindObjectsByType<SceneTransitionTrigger2D>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            SceneTransitionTrigger2D fallback = null;
            for (int index = 0; index < triggers.Length; index++)
            {
                SceneTransitionTrigger2D trigger = triggers[index];
                if (trigger == null)
                {
                    continue;
                }

                if (fallback == null)
                {
                    fallback = trigger;
                }

                if (!string.IsNullOrWhiteSpace(targetSceneName)
                    && string.Equals(trigger.TargetSceneName, targetSceneName, StringComparison.Ordinal))
                {
                    return trigger;
                }
            }

            return fallback;
        }

        private static void Persist(SnapshotArtifact artifact)
        {
            File.WriteAllText(SnapshotPath, JsonUtility.ToJson(artifact, true), new UTF8Encoding(false));
        }

        [Serializable]
        private sealed class SnapshotArtifact
        {
            public string timestamp = string.Empty;
            public string status = string.Empty;
            public string message = string.Empty;
            public bool isPlaying;
            public string activeScene = string.Empty;
            public string bootstrap = string.Empty;
            public string actionResult = string.Empty;
            public string nextAction = string.Empty;
            public string snapshot = string.Empty;
        }
    }
}
#endif
