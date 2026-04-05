#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Sunset.Editor.Story
{
    internal static class SpringDay1TargetedEditModeTestMenu
    {
        private const string WorkbenchFallbackGuardMenuPath = "Sunset/Story/Validation/Run Workbench Fallback Guard Test";
        private const string OpeningBridgeMenuPath = "Sunset/Story/Validation/Run Opening Bridge Tests";
        private const string OpeningGraphMenuPath = "Sunset/Story/Validation/Run Opening Graph Tests";
        private const string MiddayBridgeMenuPath = "Sunset/Story/Validation/Run Midday Bridge Tests";
        private const string MiddayGraphMenuPath = "Sunset/Story/Validation/Run Midday Graph Tests";
        private const string MiddayHealingBridgeMenuPath = "Sunset/Story/Validation/Run Midday Healing Bridge Test";
        private const string MiddayReminderBridgeMenuPath = "Sunset/Story/Validation/Run Midday Reminder Bridge Test";
        private const string LateDayBridgeMenuPath = "Sunset/Story/Validation/Run Late-Day Bridge Tests";
        private const string PromptOverlayGuardMenuPath = "Sunset/Story/Validation/Run PromptOverlay Guard Tests";
        private const string DirectorStagingMenuPath = "Sunset/Story/Validation/Run Director Staging Tests";
        private const string ResultFileName = "spring-day1-workbench-fallback-test.json";
        private const string OpeningResultFileName = "spring-day1-opening-bridge-tests.json";
        private const string OpeningGraphResultFileName = "spring-day1-opening-graph-tests.json";
        private const string MiddayResultFileName = "spring-day1-midday-bridge-tests.json";
        private const string MiddayGraphResultFileName = "spring-day1-midday-graph-tests.json";
        private const string MiddayHealingResultFileName = "spring-day1-midday-healing-bridge-test.json";
        private const string MiddayReminderResultFileName = "spring-day1-midday-reminder-bridge-test.json";
        private const string LateDayResultFileName = "spring-day1-late-day-bridge-tests.json";
        private const string PromptOverlayGuardResultFileName = "spring-day1-prompt-overlay-guard-tests.json";
        private const string DirectorStagingResultFileName = "spring-day1-director-staging-tests.json";
        private const string TargetTestName = "SpringDay1LateDayRuntimeTests.Director_WorkbenchFallback_ShouldNotMarkCraftObjectiveComplete";
        private static readonly string[] OpeningTargetTestNames =
        {
            "SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_FormExpectedFollowupGraph",
            "SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_PreserveOpeningSemantics",
            "SpringDay1OpeningRuntimeBridgeTests.HouseArrivalCompletion_ShouldBridgeIntoHealingAndHp",
            "SpringDay1OpeningRuntimeBridgeTests.LiveValidationRunner_ShouldRecommendOpeningActionsForCrashAndEnterVillage"
        };

        private static readonly string[] OpeningGraphTargetTestNames =
        {
            "SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_FormExpectedFollowupGraph",
            "SpringDay1OpeningDialogueAssetGraphTests.OpeningDialogueAssets_PreserveOpeningSemantics"
        };

        private static readonly string[] MiddayTargetTestNames =
        {
            "SpringDay1MiddayDialogueAssetGraphTests.MiddayDialogueAssets_ShouldExistWithExpectedSequenceIds",
            "SpringDay1MiddayDialogueAssetGraphTests.MiddayDialogueAssets_ShouldPreserveLaterDaySemantics",
            "SpringDay1MiddayRuntimeBridgeTests.Director_ShouldPreferAuthoredDialogueAssetsForMiddayPhases",
            "SpringDay1MiddayRuntimeBridgeTests.HealingCompletion_ShouldAdvanceIntoWorkbenchFlashback",
            "SpringDay1MiddayRuntimeBridgeTests.WorkbenchCompletion_ShouldAdvanceIntoFarmingTutorial",
            "SpringDay1MiddayRuntimeBridgeTests.FarmingTutorialCompletion_ShouldImmediatelyBridgeIntoDinnerConflict",
            "SpringDay1MiddayRuntimeBridgeTests.DinnerAndReminderPhases_ShouldYieldWorkbenchToFormalStory",
            "SpringDay1MiddayRuntimeBridgeTests.DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime"
        };

        private static readonly string[] MiddayGraphTargetTestNames =
        {
            "SpringDay1MiddayDialogueAssetGraphTests.MiddayDialogueAssets_ShouldExistWithExpectedSequenceIds",
            "SpringDay1MiddayDialogueAssetGraphTests.MiddayDialogueAssets_ShouldPreserveLaterDaySemantics",
            "SpringDay1MiddayRuntimeBridgeTests.Director_ShouldPreferAuthoredDialogueAssetsForMiddayPhases"
        };

        private static readonly string[] PromptOverlayGuardTargetTestNames =
        {
            "SpringDay1LateDayRuntimeTests.PromptOverlay_ShouldRecoverFromDestroyedRowCanvasGroup",
            "SpringDay1LateDayRuntimeTests.PromptOverlay_CompletionAnimation_ShouldStopTouchingDestroyedRowCanvasGroup",
            "SpringDay1LateDayRuntimeTests.PromptOverlay_Show_ShouldReactivateInactiveRuntimeInstanceBeforeStartingTransition"
        };

        private static readonly string[] DirectorStagingTargetTestNames =
        {
            "SpringDay1DirectorStagingTests.StageBook_ShouldResolveCueBySemanticAnchor",
            "SpringDay1DirectorStagingTests.StagingPlayback_ShouldPlaceNpcAtCustomStartAndExposeCueIdentity",
            "SpringDay1DirectorStagingTests.StagingPlayback_ReapplyingSameCue_ShouldNotSnapNpcBackToStart",
            "SpringDay1DirectorStagingTests.Director_ShouldExposeFreeTimeAndDayEndBeatKeys",
            "SpringDay1DirectorStagingTests.NpcTakeover_ShouldDisableRoamAndInteractionsUntilRelease",
            "SpringDay1DirectorStagingTests.PlayerRehearsalLock_ShouldDisablePlayerMotionUntilRelease"
        };

        private static readonly string[] LateDayTargetTestNames =
        {
            "SpringDay1LateDayRuntimeTests.FreeTimeValidationStep_AdvancesFromFinalCallToDayEnd",
            "SpringDay1LateDayRuntimeTests.BedBridge_EndsDayAndRestoresSystems",
            "SpringDay1LateDayRuntimeTests.FreeTimePlayerFacingCopy_ShouldTightenAcrossNightPressure",
            "SpringDay1LateDayRuntimeTests.DayEndPlayerFacingCopy_ShouldCarryTomorrowBurdenAndClearWorkbenchState",
            "SpringDay1LateDayRuntimeTests.ReminderCompletion_ShouldEnterFreeTimeWithIntroPendingAndYieldWorkbenchToFormalNightIntro"
        };

        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, ResultFileName);
        private static readonly string OpeningResultPath = Path.Combine(CommandRoot, OpeningResultFileName);
        private static readonly string OpeningGraphResultPath = Path.Combine(CommandRoot, OpeningGraphResultFileName);
        private static readonly string MiddayResultPath = Path.Combine(CommandRoot, MiddayResultFileName);
        private static readonly string MiddayGraphResultPath = Path.Combine(CommandRoot, MiddayGraphResultFileName);
        private static readonly string MiddayHealingResultPath = Path.Combine(CommandRoot, MiddayHealingResultFileName);
        private static readonly string MiddayReminderResultPath = Path.Combine(CommandRoot, MiddayReminderResultFileName);
        private static readonly string LateDayResultPath = Path.Combine(CommandRoot, LateDayResultFileName);
        private static readonly string PromptOverlayGuardResultPath = Path.Combine(CommandRoot, PromptOverlayGuardResultFileName);
        private static readonly string DirectorStagingResultPath = Path.Combine(CommandRoot, DirectorStagingResultFileName);

        private static TestRunnerApi _runnerApi;
        private static TestRunCallback _callback;

        [MenuItem(WorkbenchFallbackGuardMenuPath)]
        private static void RunWorkbenchFallbackGuardTest()
        {
            RunTargetedTests(ResultPath, new[] { TargetTestName });
        }

        [MenuItem(OpeningBridgeMenuPath)]
        private static void RunOpeningBridgeTests()
        {
            RunTargetedTests(OpeningResultPath, OpeningTargetTestNames);
        }

        [MenuItem(OpeningGraphMenuPath)]
        private static void RunOpeningGraphTests()
        {
            RunTargetedTests(OpeningGraphResultPath, OpeningGraphTargetTestNames);
        }

        [MenuItem(MiddayBridgeMenuPath)]
        private static void RunMiddayBridgeTests()
        {
            RunTargetedTests(MiddayResultPath, MiddayTargetTestNames);
        }

        [MenuItem(MiddayGraphMenuPath)]
        private static void RunMiddayGraphTests()
        {
            RunTargetedTests(MiddayGraphResultPath, MiddayGraphTargetTestNames);
        }

        [MenuItem(MiddayHealingBridgeMenuPath)]
        private static void RunMiddayHealingBridgeTest()
        {
            RunTargetedTests(MiddayHealingResultPath, new[] { "SpringDay1MiddayRuntimeBridgeTests.HealingCompletion_ShouldAdvanceIntoWorkbenchFlashback" });
        }

        [MenuItem(MiddayReminderBridgeMenuPath)]
        private static void RunMiddayReminderBridgeTest()
        {
            RunTargetedTests(MiddayReminderResultPath, new[] { "SpringDay1MiddayRuntimeBridgeTests.DinnerAndReminderCompletion_ShouldBridgeIntoFreeTime" });
        }

        [MenuItem(LateDayBridgeMenuPath)]
        private static void RunLateDayBridgeTests()
        {
            RunTargetedTests(LateDayResultPath, LateDayTargetTestNames);
        }

        [MenuItem(PromptOverlayGuardMenuPath)]
        private static void RunPromptOverlayGuardTests()
        {
            RunTargetedTests(PromptOverlayGuardResultPath, PromptOverlayGuardTargetTestNames);
        }

        [MenuItem(DirectorStagingMenuPath)]
        private static void RunDirectorStagingTests()
        {
            RunTargetedTests(DirectorStagingResultPath, DirectorStagingTargetTestNames);
        }

        [MenuItem(WorkbenchFallbackGuardMenuPath, true)]
        private static bool ValidateRunWorkbenchFallbackGuardTest()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(OpeningBridgeMenuPath, true)]
        private static bool ValidateRunOpeningBridgeTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(OpeningGraphMenuPath, true)]
        private static bool ValidateRunOpeningGraphTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(MiddayBridgeMenuPath, true)]
        private static bool ValidateRunMiddayBridgeTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(MiddayGraphMenuPath, true)]
        private static bool ValidateRunMiddayGraphTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(MiddayHealingBridgeMenuPath, true)]
        private static bool ValidateRunMiddayHealingBridgeTest()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(MiddayReminderBridgeMenuPath, true)]
        private static bool ValidateRunMiddayReminderBridgeTest()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(LateDayBridgeMenuPath, true)]
        private static bool ValidateRunLateDayBridgeTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(PromptOverlayGuardMenuPath, true)]
        private static bool ValidateRunPromptOverlayGuardTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        [MenuItem(DirectorStagingMenuPath, true)]
        private static bool ValidateRunDirectorStagingTests()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        private static void RunTargetedTests(string resultPath, string[] testNames)
        {
            Directory.CreateDirectory(CommandRoot);
            File.WriteAllText(resultPath, BuildJson("running", true, 0, 0, 0, "started", string.Join(";", testNames ?? Array.Empty<string>())));

            _runnerApi ??= ScriptableObject.CreateInstance<TestRunnerApi>();
            if (_callback != null)
            {
                _runnerApi.UnregisterCallbacks(_callback);
            }

            _callback = new TestRunCallback(resultPath);
            _runnerApi.RegisterCallbacks(_callback);

            Filter filter = new Filter
            {
                testMode = TestMode.EditMode,
                testNames = testNames
            };

            ExecutionSettings settings = new ExecutionSettings(filter);
            _runnerApi.Execute(settings);
        }

        private static string BuildJson(string status, bool success, int passCount, int failCount, int skipCount, string message, string details)
        {
            return "{" +
                   $"\"timestamp\":\"{DateTime.Now:O}\"," +
                   $"\"status\":\"{Escape(status)}\"," +
                   $"\"success\":{success.ToString().ToLowerInvariant()}," +
                   $"\"passCount\":{passCount}," +
                   $"\"failCount\":{failCount}," +
                   $"\"skipCount\":{skipCount}," +
                   $"\"message\":\"{Escape(message)}\"," +
                   $"\"details\":\"{Escape(details)}\"" +
                   "}";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\r", " ")
                .Replace("\n", " ");
        }

        private sealed class TestRunCallback : ICallbacks
        {
            private readonly string _resultPath;
            private readonly List<string> _testOutcomes = new();

            public TestRunCallback(string resultPath)
            {
                _resultPath = resultPath;
            }

            public void RunStarted(ITestAdaptor testsToRun)
            {
                File.WriteAllText(_resultPath, BuildJson("running", true, 0, 0, 0, "run-started", testsToRun?.Name ?? string.Empty));
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                bool success = result != null && result.FailCount == 0;
                string details;
                if (_testOutcomes.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    for (int index = 0; index < _testOutcomes.Count; index++)
                    {
                        if (index > 0)
                        {
                            builder.Append(" || ");
                        }

                        builder.Append(_testOutcomes[index]);
                    }

                    details = builder.ToString();
                }
                else
                {
                    details = result != null
                        ? $"{result.Name}|state={result.ResultState}|duration={result.Duration:F3}|output={result.Output}|message={result.Message}|stack={result.StackTrace}"
                        : "no-result";
                }

                File.WriteAllText(
                    _resultPath,
                    BuildJson(
                        "completed",
                        success,
                        result?.PassCount ?? 0,
                        result?.FailCount ?? 0,
                        result?.SkipCount ?? 0,
                        success ? "completed" : "failed",
                        details));
            }

            public void TestStarted(ITestAdaptor test)
            {
            }

            public void TestFinished(ITestResultAdaptor result)
            {
                if (result == null)
                {
                    return;
                }

                _testOutcomes.Add(
                    $"{result.Name}|state={result.ResultState}|duration={result.Duration:F3}|message={result.Message}|stack={result.StackTrace}|output={result.Output}");
            }
        }
    }
}
#endif
