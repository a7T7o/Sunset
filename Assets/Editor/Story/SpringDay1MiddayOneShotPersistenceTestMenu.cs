#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

namespace Sunset.Editor.Story
{
    internal static class SpringDay1MiddayOneShotPersistenceTestMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Run Midday One-Shot Persistence Test";
        private const string ResultFileName = "spring-day1-midday-oneshot-persistence-test.json";
        private const string TargetTestName = "SpringDay1MiddayRuntimeBridgeTests.OneShotSummary_ShouldBackfillPhaseImpliedCompletedSequencesAfterDialogueRuntimeReset";
        private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
        private static readonly string ResultPath = Path.Combine(CommandRoot, ResultFileName);

        private static TestRunnerApi _runnerApi;
        private static TestRunCallback _callback;

        [MenuItem(MenuPath)]
        private static void RunTest()
        {
            Directory.CreateDirectory(CommandRoot);
            File.WriteAllText(ResultPath, BuildJson("running", true, 0, 0, 0, "started", TargetTestName));

            _runnerApi ??= ScriptableObject.CreateInstance<TestRunnerApi>();
            if (_callback != null)
            {
                _runnerApi.UnregisterCallbacks(_callback);
            }

            _callback = new TestRunCallback();
            _runnerApi.RegisterCallbacks(_callback);
            _runnerApi.Execute(new ExecutionSettings(new Filter
            {
                testMode = TestMode.EditMode,
                testNames = new[] { TargetTestName }
            }));
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateRunTest()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
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
            public void RunStarted(ITestAdaptor testsToRun)
            {
                File.WriteAllText(ResultPath, BuildJson("running", true, 0, 0, 0, "run-started", testsToRun?.Name ?? string.Empty));
            }

            public void RunFinished(ITestResultAdaptor result)
            {
                bool success = result != null && result.FailCount == 0;
                string details = result != null
                    ? $"{result.Name}|state={result.ResultState}|duration={result.Duration:F3}|message={result.Message}|stack={result.StackTrace}|output={result.Output}"
                    : "no-result";
                File.WriteAllText(
                    ResultPath,
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
            }
        }
    }
}
#endif
