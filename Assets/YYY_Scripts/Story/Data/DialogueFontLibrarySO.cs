using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Sunset.Story
{
    [CreateAssetMenu(fileName = "DialogueFontLibrary", menuName = "Story/Dialogue Font Library")]
    public class DialogueFontLibrarySO : ScriptableObject
    {
        #region Nested Types
        [Serializable]
        public class FontEntry
        {
            [Header("Key")]
            public string key;

            [Header("Font")]
            public TMP_FontAsset fontAsset;

            [Header("Offsets")]
            public float fontSizeOffset;
            public float lineSpacingOffset;
        }
        #endregion

        #region Serialized Fields
        [Header("Default Key")]
        [SerializeField] private string defaultKey = "default";

        [Header("Entries")]
        [SerializeField] private List<FontEntry> entries = new();
        #endregion

        #region Public Properties
        public string DefaultKey => defaultKey;
        public IReadOnlyList<FontEntry> Entries => entries;
        #endregion

        #region Public Methods
        public bool TryGetEntry(string key, out FontEntry entry)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                for (int index = 0; index < entries.Count; index++)
                {
                    FontEntry candidate = entries[index];
                    if (candidate == null || string.IsNullOrWhiteSpace(candidate.key))
                    {
                        continue;
                    }

                    if (string.Equals(candidate.key, key, StringComparison.OrdinalIgnoreCase))
                    {
                        entry = candidate;
                        return true;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(defaultKey))
            {
                for (int index = 0; index < entries.Count; index++)
                {
                    FontEntry candidate = entries[index];
                    if (candidate == null || string.IsNullOrWhiteSpace(candidate.key))
                    {
                        continue;
                    }

                    if (string.Equals(candidate.key, defaultKey, StringComparison.OrdinalIgnoreCase))
                    {
                        entry = candidate;
                        return true;
                    }
                }
            }

            entry = null;
            return false;
        }
        #endregion
    }
}
