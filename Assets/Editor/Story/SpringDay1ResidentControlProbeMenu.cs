#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story.Editor
{
    internal static class SpringDay1ResidentControlProbeMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Write Spring Day1 Resident Control Probe Artifact";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string OutputPath = Path.Combine(CommandRoot, "spring-day1-resident-control-probe.json");

        [MenuItem(MenuPath)]
        private static void WriteProbeArtifact()
        {
            Directory.CreateDirectory(CommandRoot);

            ProbeArtifact artifact = new ProbeArtifact
            {
                timestamp = DateTime.Now.ToString("O"),
                isPlaying = Application.isPlaying,
                activeScene = SceneManager.GetActiveScene().path
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
            artifact.entries = SpringDay1NpcCrowdDirector.CaptureResidentControlProbe();

            Persist(artifact);
            Debug.Log($"[SpringDay1ResidentControlProbe] 已写出 {Path.GetFileName(OutputPath)}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateWriteProbeArtifact()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
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
            public string activeScene = string.Empty;
            public string phase = string.Empty;
            public string beatKey = string.Empty;
            public string clock = string.Empty;
            public string crowdSummary = string.Empty;
            public List<SpringDay1NpcCrowdDirector.ResidentControlProbeEntry> entries = new List<SpringDay1NpcCrowdDirector.ResidentControlProbeEntry>();
        }
    }
}
#endif
