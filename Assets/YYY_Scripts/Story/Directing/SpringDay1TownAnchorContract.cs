using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sunset.Story
{
    [Serializable]
    public sealed class SpringDay1TownAnchorContractRecord
    {
        public string anchorName = string.Empty;
        public Vector2 worldPosition = Vector2.zero;
    }

    [Serializable]
    public sealed class SpringDay1TownAnchorContract
    {
        public string version = "1.0";
        public SpringDay1TownAnchorContractRecord[] anchors = Array.Empty<SpringDay1TownAnchorContractRecord>();

        public bool TryResolveAnchor(string anchorName, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            if (anchors == null || string.IsNullOrWhiteSpace(anchorName))
            {
                return false;
            }

            string trimmed = anchorName.Trim();
            for (int index = 0; index < anchors.Length; index++)
            {
                SpringDay1TownAnchorContractRecord record = anchors[index];
                if (record == null || !string.Equals(record.anchorName?.Trim(), trimmed, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                worldPosition = new Vector3(record.worldPosition.x, record.worldPosition.y, 0f);
                return true;
            }

            return false;
        }

        public void EnsureDefaults()
        {
            version ??= "1.0";
            anchors ??= Array.Empty<SpringDay1TownAnchorContractRecord>();
            for (int index = 0; index < anchors.Length; index++)
            {
                anchors[index] ??= new SpringDay1TownAnchorContractRecord();
                anchors[index].anchorName ??= string.Empty;
            }
        }
    }

    public static class SpringDay1TownAnchorContractDatabase
    {
        private const string ResourcePath = "Story/SpringDay1/Directing/SpringDay1TownAnchorContract";
        private static SpringDay1TownAnchorContract s_cachedContract;
        private static Dictionary<string, Vector3> s_cachedLookup;

        public static SpringDay1TownAnchorContract Load(bool forceReload = false)
        {
            if (!forceReload && s_cachedContract != null)
            {
                return s_cachedContract;
            }

            TextAsset asset = Resources.Load<TextAsset>(ResourcePath);
            if (asset == null || string.IsNullOrWhiteSpace(asset.text))
            {
                s_cachedContract = null;
                s_cachedLookup = null;
                return null;
            }

            try
            {
                s_cachedContract = JsonUtility.FromJson<SpringDay1TownAnchorContract>(asset.text);
                s_cachedContract?.EnsureDefaults();
                s_cachedLookup = BuildLookup(s_cachedContract);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SpringDay1TownAnchorContractDatabase] 解析失败: {ex.Message}");
                s_cachedContract = null;
                s_cachedLookup = null;
            }

            return s_cachedContract;
        }

        public static bool TryResolveAnchor(string anchorName, out Vector3 worldPosition)
        {
            worldPosition = Vector3.zero;
            if (string.IsNullOrWhiteSpace(anchorName))
            {
                return false;
            }

            if (s_cachedLookup == null && Load() == null)
            {
                return false;
            }

            return s_cachedLookup != null && s_cachedLookup.TryGetValue(anchorName.Trim(), out worldPosition);
        }

        private static Dictionary<string, Vector3> BuildLookup(SpringDay1TownAnchorContract contract)
        {
            if (contract?.anchors == null)
            {
                return null;
            }

            Dictionary<string, Vector3> lookup = new Dictionary<string, Vector3>(StringComparer.OrdinalIgnoreCase);
            for (int index = 0; index < contract.anchors.Length; index++)
            {
                SpringDay1TownAnchorContractRecord record = contract.anchors[index];
                if (record == null || string.IsNullOrWhiteSpace(record.anchorName))
                {
                    continue;
                }

                string trimmed = record.anchorName.Trim();
                if (!lookup.ContainsKey(trimmed))
                {
                    lookup[trimmed] = new Vector3(record.worldPosition.x, record.worldPosition.y, 0f);
                }
            }

            return lookup;
        }
    }
}
