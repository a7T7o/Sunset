#if UNITY_EDITOR
using System;
using System.Reflection;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sunset.Editor.Story
{
    internal static class SunsetValidationSessionCleanupMenu
    {
        private const string MenuPath = "Sunset/Story/Validation/Clear Validation Session Residue";

        private static readonly string[] EditorPrefKeys =
        {
            "Sunset.NavigationLiveValidation.PendingAction",
            "Sunset.NpcInformalChatValidation.Active",
            "Sunset.NavigationStaticValidation.PendingAction",
            "Sunset.NavigationStaticValidation.ConsoleErrorPauseSnapshot",
            "Sunset.NavigationStaticValidation.ConsoleErrorPauseOverrideActive"
        };

        private static readonly string[] SessionStateKeys =
        {
            "Sunset.SpringDay1DirectorPrimaryRehearsalBake.Queued",
            "Sunset.PlacementSecondBlade.PendingRunScope"
        };

        private static readonly string[] PendingFiles =
        {
            "Library/NavStaticPointValidation.pending",
            "Library/NavigationLiveValidation/pending_action.txt"
        };
        private static readonly MethodInfo ClearConsoleMethod =
            Type.GetType("UnityEditor.LogEntries, UnityEditor")
                ?.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        [MenuItem(MenuPath)]
        private static void ClearValidationSessionResidue()
        {
            int clearedEditorPrefs = 0;
            int clearedSessionStates = 0;
            int deletedFiles = 0;
            bool clearedConsole = false;

            foreach (string key in EditorPrefKeys)
            {
                if (!EditorPrefs.HasKey(key))
                {
                    continue;
                }

                EditorPrefs.DeleteKey(key);
                clearedEditorPrefs++;
            }

            foreach (string key in SessionStateKeys)
            {
                if (SessionState.GetBool(key, false))
                {
                    SessionState.SetBool(key, false);
                    clearedSessionStates++;
                    continue;
                }

                int currentInt = SessionState.GetInt(key, int.MinValue);
                if (currentInt != int.MinValue)
                {
                    SessionState.EraseInt(key);
                    clearedSessionStates++;
                }
            }

            string projectRoot = Directory.GetCurrentDirectory();
            foreach (string relativePath in PendingFiles)
            {
                string absolutePath = Path.Combine(projectRoot, relativePath);
                if (!File.Exists(absolutePath))
                {
                    continue;
                }

                File.Delete(absolutePath);
                deletedFiles++;
            }

            if (ClearConsoleMethod != null)
            {
                ClearConsoleMethod.Invoke(null, null);
                clearedConsole = true;
            }

            Debug.Log(
                $"[SunsetValidationSessionCleanup] 已清理 residue：EditorPrefs={clearedEditorPrefs}, SessionState={clearedSessionStates}, Files={deletedFiles}, Console={clearedConsole}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateClearValidationSessionResidue()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }
    }
}
#endif
