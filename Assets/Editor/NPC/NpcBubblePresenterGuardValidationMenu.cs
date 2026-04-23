#if UNITY_EDITOR
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.TestTools.TestRunner.Api;
using UnityEngine;

internal static class NpcBubblePresenterGuardValidationMenu
{
    private const string MenuPath = "Tools/Sunset/NPC/Validation/Run Bubble Presenter Guard Tests";
    private const string ResultFileName = "npc-bubble-presenter-guard-tests.json";

    private static readonly string CommandRoot = Path.Combine(Directory.GetCurrentDirectory(), "Library", "CodexEditorCommands");
    private static readonly string ResultPath = Path.Combine(CommandRoot, ResultFileName);
    private static readonly string[] TargetTestNames =
    {
        "NpcBubblePresenterEditModeGuardTests.PrefabAssetPresenter_ShouldNotBuildRuntimeBubbleUi",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShouldAllowRuntimeBubbleUiForTests",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldMakeBubbleVisible",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowConversationImmediate_ShouldMakeBubbleVisible",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldClearStalePromptSuppression",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldRefreshLegacyBubbleSprites",
        "NpcBubblePresenterEditModeGuardTests.TemporaryEditObjectPresenter_ShowText_ShouldRecoverCachedBubbleSpritesAfterTheyGoNull"
    };

    private static TestRunnerApi s_runnerApi;
    private static Callback s_callback;

    [MenuItem(MenuPath)]
    private static void RunTests()
    {
        Directory.CreateDirectory(CommandRoot);
        File.WriteAllText(ResultPath, BuildJson("running", true, 0, 0, 0, "started", string.Join(";", TargetTestNames)));

        s_runnerApi ??= ScriptableObject.CreateInstance<TestRunnerApi>();
        if (s_callback != null)
        {
            s_runnerApi.UnregisterCallbacks(s_callback);
        }

        s_callback = new Callback(ResultPath);
        s_runnerApi.RegisterCallbacks(s_callback);
        s_runnerApi.Execute(new ExecutionSettings(new Filter
        {
            testMode = TestMode.EditMode,
            testNames = TargetTestNames
        }));
    }

    [MenuItem(MenuPath, true)]
    private static bool ValidateRunTests()
    {
        return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
    }

    private static string BuildJson(string status, bool success, int total, int passed, int failed, string summary, string details)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append('{');
        AppendJson(builder, "status", status);
        builder.Append(',');
        AppendJson(builder, "success", success);
        builder.Append(',');
        AppendJson(builder, "total", total);
        builder.Append(',');
        AppendJson(builder, "passed", passed);
        builder.Append(',');
        AppendJson(builder, "failed", failed);
        builder.Append(',');
        AppendJson(builder, "summary", summary);
        builder.Append(',');
        AppendJson(builder, "details", details);
        builder.Append('}');
        return builder.ToString();
    }

    private static void AppendJson(StringBuilder builder, string key, string value)
    {
        builder.Append('"').Append(Escape(key)).Append('"').Append(':');
        builder.Append('"').Append(Escape(value ?? string.Empty)).Append('"');
    }

    private static void AppendJson(StringBuilder builder, string key, bool value)
    {
        builder.Append('"').Append(Escape(key)).Append('"').Append(':');
        builder.Append(value ? "true" : "false");
    }

    private static void AppendJson(StringBuilder builder, string key, int value)
    {
        builder.Append('"').Append(Escape(key)).Append('"').Append(':');
        builder.Append(value);
    }

    private static string Escape(string value)
    {
        return (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }

    private sealed class Callback : ICallbacks
    {
        private readonly string _resultPath;
        private int _total;
        private int _passed;
        private int _failed;

        public Callback(string resultPath)
        {
            _resultPath = resultPath;
        }

        public void RunStarted(ITestAdaptor testsToRun)
        {
            _total = 0;
            _passed = 0;
            _failed = 0;
            File.WriteAllText(_resultPath, BuildJson("running", true, 0, 0, 0, "started", string.Empty));
        }

        public void RunFinished(ITestResultAdaptor result)
        {
            string summary = result != null ? result.TestStatus.ToString() : string.Empty;
            File.WriteAllText(
                _resultPath,
                BuildJson(_failed == 0 ? "passed" : "failed", _failed == 0, _total, _passed, _failed, summary, string.Empty));
        }

        public void TestStarted(ITestAdaptor test)
        {
        }

        public void TestFinished(ITestResultAdaptor result)
        {
            if (result == null || result.HasChildren)
            {
                return;
            }

            _total++;
            if (result.TestStatus == TestStatus.Passed)
            {
                _passed++;
            }
            else
            {
                _failed++;
            }
        }
    }
}
#endif
