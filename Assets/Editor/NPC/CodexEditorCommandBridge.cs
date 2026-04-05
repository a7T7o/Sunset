#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;

[InitializeOnLoad]
internal static class CodexEditorCommandBridge
{
    private const string CommandDirectoryName = "Library/CodexEditorCommands";
    private const string RequestDirectoryName = "requests";
    private const string ArchiveDirectoryName = "archive";
    private const string StatusFileName = "status.json";

    private static readonly string CommandRoot;
    private static readonly string RequestRoot;
    private static readonly string ArchiveRoot;
    private static readonly string StatusPath;

    static CodexEditorCommandBridge()
    {
        string projectRoot = Directory.GetCurrentDirectory();
        CommandRoot = Path.Combine(projectRoot, CommandDirectoryName);
        RequestRoot = Path.Combine(CommandRoot, RequestDirectoryName);
        ArchiveRoot = Path.Combine(CommandRoot, ArchiveDirectoryName);
        StatusPath = Path.Combine(CommandRoot, StatusFileName);

        Directory.CreateDirectory(RequestRoot);
        Directory.CreateDirectory(ArchiveRoot);

        EditorApplication.update -= Tick;
        EditorApplication.update += Tick;
        EditorApplication.playModeStateChanged -= HandlePlayModeStateChanged;
        EditorApplication.playModeStateChanged += HandlePlayModeStateChanged;
        CompilationPipeline.compilationFinished -= HandleCompilationFinished;
        CompilationPipeline.compilationFinished += HandleCompilationFinished;

        WriteStatus("initialized", success: true);
    }

    private static void Tick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            WriteStatus("waiting-editor-busy", success: true);
            return;
        }

        string[] requests = Directory.GetFiles(RequestRoot, "*.cmd");
        if (requests.Length == 0)
        {
            return;
        }

        Array.Sort(requests, StringComparer.OrdinalIgnoreCase);
        string requestPath = requests[0];
        string commandText = SafeReadAllText(requestPath).Trim();
        if (string.IsNullOrWhiteSpace(commandText))
        {
            ArchiveRequest(requestPath, "empty");
            WriteStatus("empty-command", success: false);
            return;
        }

        try
        {
            if (string.Equals(commandText, "PLAY", StringComparison.OrdinalIgnoreCase))
            {
                if (!EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = true;
                }

                ArchiveRequest(requestPath, "play");
                WriteStatus("play-requested", success: true);
                return;
            }

            if (string.Equals(commandText, "STOP", StringComparison.OrdinalIgnoreCase))
            {
                if (EditorApplication.isPlaying)
                {
                    EditorApplication.isPlaying = false;
                }

                ArchiveRequest(requestPath, "stop");
                WriteStatus("stop-requested", success: true);
                return;
            }

            const string menuPrefix = "MENU=";
            if (commandText.StartsWith(menuPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string menuPath = commandText.Substring(menuPrefix.Length).Trim();
                bool executed = EditorApplication.ExecuteMenuItem(menuPath);
                ArchiveRequest(requestPath, executed ? "menu-ok" : "menu-fail");
                WriteStatus($"menu:{menuPath}", executed);
                return;
            }

            ArchiveRequest(requestPath, "unknown");
            WriteStatus($"unknown:{commandText}", success: false);
        }
        catch (Exception ex)
        {
            ArchiveRequest(requestPath, "exception");
            WriteStatus($"{commandText} | {ex.GetType().Name}: {ex.Message}", success: false);
        }
    }

    private static void HandlePlayModeStateChanged(PlayModeStateChange state)
    {
        WriteStatus($"playmode:{state}", success: true);
    }

    private static void HandleCompilationFinished(object _)
    {
        WriteStatus("compilation-finished", success: true);
    }

    private static void ArchiveRequest(string requestPath, string suffix)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
        string fileName = Path.GetFileNameWithoutExtension(requestPath);
        string archivePath = Path.Combine(ArchiveRoot, $"{timestamp}_{fileName}_{suffix}.cmd");
        if (File.Exists(archivePath))
        {
            File.Delete(archivePath);
        }

        File.Move(requestPath, archivePath);
    }

    private static string SafeReadAllText(string path)
    {
        using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using StreamReader reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static void WriteStatus(string lastCommand, bool success)
    {
        string payload =
            "{" +
            $"\"timestamp\":\"{DateTime.Now:O}\"," +
            $"\"isPlaying\":{EditorApplication.isPlaying.ToString().ToLowerInvariant()}," +
            $"\"isCompiling\":{EditorApplication.isCompiling.ToString().ToLowerInvariant()}," +
            $"\"lastCommand\":\"{Escape(lastCommand)}\"," +
            $"\"success\":{success.ToString().ToLowerInvariant()}" +
            "}";
        File.WriteAllText(StatusPath, payload);
    }

    private static string Escape(string value)
    {
        return (value ?? string.Empty)
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"");
    }
}
#endif
