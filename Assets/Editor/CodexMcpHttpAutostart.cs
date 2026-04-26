using System;
using System.IO;
using MCPForUnity.Editor.Services;
using UnityEditor;

/// <summary>
/// Codex uses the local HTTP `/mcp` endpoint directly.
/// Do not auto-start the package's websocket bridge here, otherwise it keeps
/// emitting noisy hub-connection errors into the Unity console.
/// </summary>
[InitializeOnLoad]
internal static class CodexMcpHttpAutostart
{
    static CodexMcpHttpAutostart()
    {
        EditorApplication.delayCall += TryStartBridgeOnce;
    }

    private static async void TryStartBridgeOnce()
    {
        try
        {
            WriteStatus("delayCall fired");

            if (!EditorConfigurationCache.Instance.UseHttpTransport)
            {
                WriteStatus("skipped: UseHttpTransport=false");
                return;
            }

            if (MCPServiceLocator.Bridge.IsRunning)
            {
                WriteStatus("skipped: bridge already running");
                return;
            }

            if (!MCPServiceLocator.Server.IsLocalHttpServerReachable())
            {
                WriteStatus("skipped: local HTTP server not reachable");
                return;
            }

            if (MCPServiceLocator.Bridge.IsRunning)
            {
                await MCPServiceLocator.Bridge.StopAsync();
                WriteStatus("stopped: bridge disabled for Codex direct HTTP mode");
                return;
            }

            WriteStatus("skipped: local HTTP `/mcp` is healthy; websocket bridge intentionally not started");
        }
        catch (Exception ex)
        {
            WriteStatus($"exception: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void WriteStatus(string message)
    {
        try
        {
            string projectRoot = Directory.GetCurrentDirectory();
            string runStateDir = Path.Combine(projectRoot, "Library", "MCPForUnity", "RunState");
            Directory.CreateDirectory(runStateDir);
            string statusPath = Path.Combine(runStateDir, "codex_mcp_http_autostart_status.txt");
            File.AppendAllText(statusPath, $"{DateTime.Now:O} | {message}{Environment.NewLine}");
        }
        catch
        {
            // Best-effort diagnostics only.
        }
    }
}
