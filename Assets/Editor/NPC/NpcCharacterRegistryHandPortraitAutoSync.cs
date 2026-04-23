using System;
using System.Collections.Generic;
using System.IO;
using Sunset.Story;
using UnityEditor;
using UnityEngine;

namespace Sunset.Editor.Npc
{
    [InitializeOnLoad]
    internal sealed class NpcCharacterRegistryHandPortraitAutoSync : AssetPostprocessor
    {
        private const string RegistryAssetPath = "Assets/Resources/Story/NpcCharacterRegistry.asset";
        private const string MenuPath = "Tools/Sunset/NPC/Sync Hand Portrait Registry";

        private static bool _syncQueued;
        private static bool _syncInProgress;

        static NpcCharacterRegistryHandPortraitAutoSync()
        {
            QueueSync();
        }

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (ContainsRelevantPath(importedAssets)
                || ContainsRelevantPath(deletedAssets)
                || ContainsRelevantPath(movedAssets)
                || ContainsRelevantPath(movedFromAssetPaths))
            {
                QueueSync();
            }
        }

        [MenuItem(MenuPath)]
        private static void SyncFromMenu()
        {
            bool changed = TrySyncRegistryAsset();
            Debug.Log(changed
                ? "[NpcCharacterRegistryHandPortraitAutoSync] 已将 NPC_Hand 头像同步进主表。"
                : "[NpcCharacterRegistryHandPortraitAutoSync] 主表头像已是最新，无需额外同步。");
        }

        [MenuItem(MenuPath, true)]
        private static bool ValidateSyncFromMenu()
        {
            return !EditorApplication.isCompiling && !EditorApplication.isUpdating;
        }

        internal static bool TrySyncRegistryAsset()
        {
            if (_syncInProgress)
            {
                return false;
            }

            NpcCharacterRegistry registry = AssetDatabase.LoadAssetAtPath<NpcCharacterRegistry>(RegistryAssetPath);
            if (registry == null)
            {
                return false;
            }

            _syncInProgress = true;
            try
            {
                Dictionary<string, Sprite> handPortraitLookup = BuildHandPortraitLookup();
                bool changed = ApplyLookupToRegistry(registry, handPortraitLookup);
                if (changed)
                {
                    registry.InvalidateCachedLookups();
                    EditorUtility.SetDirty(registry);
                    AssetDatabase.SaveAssets();
                }

                return changed;
            }
            finally
            {
                _syncInProgress = false;
            }
        }

        private static void QueueSync()
        {
            if (_syncQueued)
            {
                return;
            }

            _syncQueued = true;
            EditorApplication.delayCall -= RunQueuedSync;
            EditorApplication.delayCall += RunQueuedSync;
        }

        private static void RunQueuedSync()
        {
            EditorApplication.delayCall -= RunQueuedSync;
            _syncQueued = false;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
            {
                QueueSync();
                return;
            }

            TrySyncRegistryAsset();
        }

        private static bool ApplyLookupToRegistry(NpcCharacterRegistry registry, Dictionary<string, Sprite> lookup)
        {
            bool changed = false;
            NpcCharacterRegistry.Entry[] entries = registry.Entries;
            for (int index = 0; index < entries.Length; index++)
            {
                NpcCharacterRegistry.Entry entry = entries[index];
                if (entry == null)
                {
                    continue;
                }

                string normalizedNpcId = Normalize(entry.npcId);
                Sprite expectedPortrait = null;
                if (normalizedNpcId.Length > 0)
                {
                    lookup.TryGetValue(normalizedNpcId, out expectedPortrait);
                }

                if (entry.handPortrait == expectedPortrait)
                {
                    continue;
                }

                entry.handPortrait = expectedPortrait;
                changed = true;
            }

            return changed;
        }

        private static Dictionary<string, Sprite> BuildHandPortraitLookup()
        {
            var result = new Dictionary<string, Sprite>(StringComparer.Ordinal);
            if (!AssetDatabase.IsValidFolder(NpcCharacterRegistry.HandPortraitFolderPath))
            {
                return result;
            }

            string[] spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { NpcCharacterRegistry.HandPortraitFolderPath });
            for (int index = 0; index < spriteGuids.Length; index++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(spriteGuids[index]);
                if (string.IsNullOrWhiteSpace(assetPath))
                {
                    continue;
                }

                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                if (sprite == null)
                {
                    continue;
                }

                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                string normalizedNpcId = Normalize(fileName);
                if (normalizedNpcId.Length == 0)
                {
                    continue;
                }

                result[normalizedNpcId] = sprite;
            }

            return result;
        }

        private static bool ContainsRelevantPath(string[] assetPaths)
        {
            if (assetPaths == null || assetPaths.Length == 0)
            {
                return false;
            }

            for (int index = 0; index < assetPaths.Length; index++)
            {
                string path = assetPaths[index];
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                if (path.StartsWith(NpcCharacterRegistry.HandPortraitFolderPath, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(path, RegistryAssetPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToLowerInvariant();
        }
    }
}
