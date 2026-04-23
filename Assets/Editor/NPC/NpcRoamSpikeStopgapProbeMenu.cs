#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

internal static class NpcRoamSpikeStopgapProbeMenu
{
    private const string MenuPath = "Tools/Sunset/NPC/Run Roam Spike Stopgap Probe";
    private const string OutputRelativePath = "Library/CodexEditorCommands/npc-roam-spike-stopgap-probe.json";
    private static ProbeSession currentSession;

    [MenuItem(MenuPath)]
    private static void RunProbe()
    {
        if (!EditorApplication.isPlaying)
        {
            WriteImmediateReport("blocked", "Editor 不在 Play Mode，无法执行 roam stopgap probe。");
            Debug.LogError("[NpcRoamSpikeStopgapProbe] blocked: editor is not playing.");
            return;
        }

        if (currentSession != null)
        {
            Debug.LogWarning("[NpcRoamSpikeStopgapProbe] probe already running.");
            return;
        }

        currentSession = new ProbeSession();
        currentSession.Start();
        Debug.Log("[NpcRoamSpikeStopgapProbe] started.");
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateRunProbe()
    {
        return EditorApplication.isPlaying;
    }

    private static void WriteImmediateReport(string status, string message)
    {
        WriteReport(new ProbeReport
        {
            status = status,
            scene = SceneManager.GetActiveScene().name,
            message = message,
            durationSeconds = 0f,
            sampleCount = 0,
            maxGcAllocBytes = -1
        });
    }

    private static void WriteReport(ProbeReport report)
    {
        string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
        string outputPath = Path.Combine(projectRoot, OutputRelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath) ?? Directory.GetCurrentDirectory());
        File.WriteAllText(outputPath, JsonUtility.ToJson(report, prettyPrint: true));
    }

    [Serializable]
    private sealed class ProbeReport
    {
        public string status;
        public string scene;
        public string message;
        public int npcCount;
        public int roamNpcCount;
        public float durationSeconds;
        public int sampleCount;
        public float avgFrameMs;
        public float maxFrameMs;
        public long maxGcAllocBytes;
        public int maxBlockedNpcCount;
        public int maxBlockedAdvanceFrames;
        public int maxConsecutivePathBuildFailures;
        public float maxPathBuildCooldownRemaining;
        public int blockedAdvanceStopgapSamples;
        public int passiveAvoidanceStopgapSamples;
        public int stuckCancelStopgapSamples;
        public ReasonCountEntry[] topSkipReasons;
    }

    [Serializable]
    private sealed class ReasonCountEntry
    {
        public string reason;
        public int count;
    }

    private sealed class ProbeSession
    {
        private const float ProbeDurationSeconds = 6f;
        private readonly Dictionary<string, int> reasonCounts = new Dictionary<string, int>(StringComparer.Ordinal);

        private ProfilerRecorder gcAllocRecorder;
        private double startRealtime;
        private double lastSampleRealtime;
        private int sampleCount;
        private float totalFrameMs;
        private float maxFrameMs;
        private long maxGcAllocBytes = -1;
        private int maxNpcCount;
        private int maxRoamNpcCount;
        private int maxBlockedNpcCount;
        private int maxBlockedAdvanceFrames;
        private int maxConsecutivePathBuildFailures;
        private float maxPathBuildCooldownRemaining;
        private int blockedAdvanceStopgapSamples;
        private int passiveAvoidanceStopgapSamples;
        private int stuckCancelStopgapSamples;

        public void Start()
        {
            startRealtime = EditorApplication.timeSinceStartup;
            lastSampleRealtime = startRealtime;
            try
            {
                gcAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
            }
            catch
            {
                maxGcAllocBytes = -1;
            }

            EditorApplication.update += Tick;
        }

        private void Tick()
        {
            if (!EditorApplication.isPlaying)
            {
                Finish("blocked", "Probe 在完成前退出了 Play Mode。");
                return;
            }

            SampleCurrentFrame();
            if (EditorApplication.timeSinceStartup - startRealtime >= ProbeDurationSeconds)
            {
                Finish("completed", "Npc roam spike stopgap probe completed.");
            }
        }

        private void SampleCurrentFrame()
        {
            sampleCount++;

            double now = EditorApplication.timeSinceStartup;
            float frameMs = (float)Math.Max(0d, (now - lastSampleRealtime) * 1000d);
            lastSampleRealtime = now;
            totalFrameMs += frameMs;
            maxFrameMs = Mathf.Max(maxFrameMs, frameMs);

            if (gcAllocRecorder.Valid)
            {
                maxGcAllocBytes = Math.Max(maxGcAllocBytes, gcAllocRecorder.LastValue);
            }

            NPCAutoRoamController[] roamControllers = UnityEngine.Object.FindObjectsByType<NPCAutoRoamController>(
                FindObjectsInactive.Exclude,
                FindObjectsSortMode.None);

            maxNpcCount = Math.Max(maxNpcCount, roamControllers.Length);

            int roamNpcCount = 0;
            int blockedNpcCount = 0;
            for (int index = 0; index < roamControllers.Length; index++)
            {
                NPCAutoRoamController controller = roamControllers[index];
                if (controller == null || !controller.isActiveAndEnabled)
                {
                    continue;
                }

                if (controller.IsRoaming)
                {
                    roamNpcCount++;
                }

                if (controller.DebugBlockedAdvanceFrames > 0)
                {
                    blockedNpcCount++;
                }

                maxBlockedAdvanceFrames = Math.Max(maxBlockedAdvanceFrames, controller.DebugBlockedAdvanceFrames);
                maxConsecutivePathBuildFailures = Math.Max(maxConsecutivePathBuildFailures, controller.DebugConsecutivePathBuildFailures);
                maxPathBuildCooldownRemaining = Mathf.Max(maxPathBuildCooldownRemaining, controller.DebugPathBuildCooldownRemaining);

                string reason = string.IsNullOrWhiteSpace(controller.DebugLastMoveSkipReason)
                    ? "None"
                    : controller.DebugLastMoveSkipReason;
                reasonCounts.TryGetValue(reason, out int count);
                reasonCounts[reason] = count + 1;

                switch (reason)
                {
                    case "BlockedAdvanceStopgap":
                        blockedAdvanceStopgapSamples++;
                        break;
                    case "PassiveAvoidanceStopgap":
                        passiveAvoidanceStopgapSamples++;
                        break;
                    case "StuckCancelStopgap":
                        stuckCancelStopgapSamples++;
                        break;
                }
            }

            maxRoamNpcCount = Math.Max(maxRoamNpcCount, roamNpcCount);
            maxBlockedNpcCount = Math.Max(maxBlockedNpcCount, blockedNpcCount);
        }

        private void Finish(string status, string message)
        {
            EditorApplication.update -= Tick;
            if (gcAllocRecorder.Valid)
            {
                gcAllocRecorder.Dispose();
            }

            ProbeReport report = new ProbeReport
            {
                status = status,
                scene = SceneManager.GetActiveScene().name,
                message = message,
                npcCount = maxNpcCount,
                roamNpcCount = maxRoamNpcCount,
                durationSeconds = (float)(EditorApplication.timeSinceStartup - startRealtime),
                sampleCount = sampleCount,
                avgFrameMs = sampleCount > 0 ? totalFrameMs / sampleCount : 0f,
                maxFrameMs = maxFrameMs,
                maxGcAllocBytes = maxGcAllocBytes,
                maxBlockedNpcCount = maxBlockedNpcCount,
                maxBlockedAdvanceFrames = maxBlockedAdvanceFrames,
                maxConsecutivePathBuildFailures = maxConsecutivePathBuildFailures,
                maxPathBuildCooldownRemaining = maxPathBuildCooldownRemaining,
                blockedAdvanceStopgapSamples = blockedAdvanceStopgapSamples,
                passiveAvoidanceStopgapSamples = passiveAvoidanceStopgapSamples,
                stuckCancelStopgapSamples = stuckCancelStopgapSamples,
                topSkipReasons = reasonCounts
                    .OrderByDescending(entry => entry.Value)
                    .Take(8)
                    .Select(entry => new ReasonCountEntry { reason = entry.Key, count = entry.Value })
                    .ToArray()
            };

            WriteReport(report);
            Debug.Log($"[NpcRoamSpikeStopgapProbe] {status} => scene={report.scene}, maxFrameMs={report.maxFrameMs:F2}, maxBlockedNpcCount={report.maxBlockedNpcCount}, blockedAdvanceStopgapSamples={report.blockedAdvanceStopgapSamples}, stuckCancelStopgapSamples={report.stuckCancelStopgapSamples}");
            currentSession = null;
        }
    }
}
#endif
