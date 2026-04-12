using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sunset.Story
{
    [CreateAssetMenu(fileName = "NpcCharacterRegistry", menuName = "Sunset/Story/NPC Character Registry", order = 255)]
    public sealed class NpcCharacterRegistry : ScriptableObject
    {
        public const string ResourcesPath = "Story/NpcCharacterRegistry";
        public const string HandPortraitFolderPath = "Assets/Sprites/NPC_Hand";

        [Serializable]
        public sealed class RelationshipBeatSemantic
        {
            public string beatKey = string.Empty;
            public SpringDay1CrowdResidentPresenceLevel presenceLevel = SpringDay1CrowdResidentPresenceLevel.None;
            [TextArea] public string note = string.Empty;
        }

        [Serializable]
        public sealed class Entry
        {
            public string npcId = string.Empty;
            public string canonicalName = string.Empty;
            public string relationshipDisplayName = string.Empty;
            [TextArea] public string roleSummary = string.Empty;
            public string[] speakerAliases = Array.Empty<string>();
            public GameObject prefab;
            public Sprite handPortrait;
            public bool showInRelationshipPanel = true;
            public SpringDay1CrowdResidentBaseline relationshipBaseline = SpringDay1CrowdResidentBaseline.PeripheralResident;
            public RelationshipBeatSemantic[] relationshipBeatSemantics = Array.Empty<RelationshipBeatSemantic>();

            public string RelationshipDisplayNameOrCanonical
            {
                get
                {
                    if (!string.IsNullOrWhiteSpace(relationshipDisplayName))
                    {
                        return relationshipDisplayName.Trim();
                    }

                    return canonicalName?.Trim() ?? string.Empty;
                }
            }

            public bool MatchesSpeaker(string speakerName)
            {
                if (IsNormalizedMatch(speakerName, canonicalName) || IsNormalizedMatch(speakerName, relationshipDisplayName))
                {
                    return true;
                }

                string[] aliases = speakerAliases ?? Array.Empty<string>();
                for (int index = 0; index < aliases.Length; index++)
                {
                    if (IsNormalizedMatch(speakerName, aliases[index]))
                    {
                        return true;
                    }
                }

                return false;
            }

            public bool TryGetRelationshipBeatSemantic(string beatKey, out RelationshipBeatSemantic semantic)
            {
                semantic = null;
                if (relationshipBeatSemantics == null || string.IsNullOrWhiteSpace(beatKey))
                {
                    return false;
                }

                string normalizedBeatKey = Normalize(beatKey);
                for (int index = 0; index < relationshipBeatSemantics.Length; index++)
                {
                    RelationshipBeatSemantic candidate = relationshipBeatSemantics[index];
                    if (candidate == null || Normalize(candidate.beatKey) != normalizedBeatKey)
                    {
                        continue;
                    }

                    semantic = candidate;
                    return true;
                }

                return false;
            }

            public Sprite ResolveDialoguePortrait()
            {
                if (handPortrait != null)
                {
                    return handPortrait;
                }

                return ResolvePrefabPortrait(prefab);
            }

            public Sprite ResolveRelationshipPortrait()
            {
                if (handPortrait != null)
                {
                    return handPortrait;
                }

                return ResolvePrefabPortrait(prefab);
            }

            private static bool IsNormalizedMatch(string left, string right)
            {
                string normalizedLeft = Normalize(left);
                if (normalizedLeft.Length == 0)
                {
                    return false;
                }

                return normalizedLeft == Normalize(right);
            }
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();
        [NonSerialized] private Dictionary<string, Entry> _entryByNpcId;
        [NonSerialized] private Dictionary<string, Entry> _entryBySpeaker;
        [NonSerialized] private Dictionary<string, Sprite> _handPortraitByNpcId;
        [NonSerialized] private Entry[] _relationshipPanelEntries;

        public Entry[] Entries => entries ?? Array.Empty<Entry>();

        private void OnEnable()
        {
            InvalidateCachedLookups();
        }

        private void OnValidate()
        {
            InvalidateCachedLookups();
        }

        public static NpcCharacterRegistry LoadRuntime()
        {
            NpcCharacterRegistry registry = Resources.Load<NpcCharacterRegistry>(ResourcesPath);
            if (registry != null)
            {
                registry.EnsureLookupCache();
            }

            return registry;
        }

        public void InvalidateCachedLookups()
        {
            _entryByNpcId = null;
            _entryBySpeaker = null;
            _handPortraitByNpcId = null;
            _relationshipPanelEntries = null;
        }

        public bool TryGetEntryByNpcId(string npcId, out Entry entry)
        {
            entry = null;
            string normalizedNpcId = Normalize(npcId);
            if (normalizedNpcId.Length == 0)
            {
                return false;
            }

            EnsureLookupCache();
            return _entryByNpcId != null && _entryByNpcId.TryGetValue(normalizedNpcId, out entry) && entry != null;
        }

        public bool TryResolveSpeaker(string speakerName, out Entry entry)
        {
            entry = null;
            string normalizedSpeaker = Normalize(speakerName);
            if (normalizedSpeaker.Length == 0)
            {
                return false;
            }

            EnsureLookupCache();
            return _entryBySpeaker != null && _entryBySpeaker.TryGetValue(normalizedSpeaker, out entry) && entry != null;
        }

        public bool TryResolveNpcId(string rawValue, out string npcId)
        {
            npcId = string.Empty;
            if (TryGetEntryByNpcId(rawValue, out Entry entry) || TryResolveSpeaker(rawValue, out entry))
            {
                npcId = entry != null && !string.IsNullOrWhiteSpace(entry.npcId)
                    ? entry.npcId.Trim()
                    : string.Empty;
                return npcId.Length > 0;
            }

            return false;
        }

        public bool TryResolveHandPortrait(string rawValue, out Sprite portrait)
        {
            portrait = null;
            if (!TryResolveNpcId(rawValue, out string npcId))
            {
                return false;
            }

            EnsureLookupCache();
            string normalizedNpcId = Normalize(npcId);
            return normalizedNpcId.Length > 0
                && _handPortraitByNpcId != null
                && _handPortraitByNpcId.TryGetValue(normalizedNpcId, out portrait)
                && portrait != null;
        }

        public Entry[] GetRelationshipPanelEntries()
        {
            EnsureLookupCache();
            return _relationshipPanelEntries ?? Array.Empty<Entry>();
        }

        public bool TryResolveDialoguePortrait(string speakerName, out Sprite portrait)
        {
            portrait = null;
            if (!TryResolveSpeaker(speakerName, out Entry entry) || entry == null)
            {
                return false;
            }

            portrait = entry.ResolveDialoguePortrait();
            return portrait != null;
        }

        public static Sprite ResolvePrefabPortrait(GameObject prefab)
        {
            if (prefab == null)
            {
                return null;
            }

            SpriteRenderer[] renderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
            Sprite bestSprite = null;
            float bestArea = -1f;
            for (int index = 0; index < renderers.Length; index++)
            {
                SpriteRenderer renderer = renderers[index];
                if (renderer == null || renderer.sprite == null)
                {
                    continue;
                }

                Rect rect = renderer.sprite.rect;
                float area = rect.width * rect.height;
                if (area <= bestArea)
                {
                    continue;
                }

                bestArea = area;
                bestSprite = renderer.sprite;
            }

            return bestSprite;
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim().ToLowerInvariant();
        }

        private void EnsureLookupCache()
        {
            if (_entryByNpcId != null
                && _entryBySpeaker != null
                && _handPortraitByNpcId != null
                && _relationshipPanelEntries != null)
            {
                return;
            }

            _entryByNpcId = new Dictionary<string, Entry>(StringComparer.Ordinal);
            _entryBySpeaker = new Dictionary<string, Entry>(StringComparer.Ordinal);
            _handPortraitByNpcId = new Dictionary<string, Sprite>(StringComparer.Ordinal);

            Entry[] currentEntries = Entries;
            Entry[] filtered = new Entry[currentEntries.Length];
            int filteredCount = 0;
            for (int index = 0; index < currentEntries.Length; index++)
            {
                Entry candidate = currentEntries[index];
                if (candidate == null)
                {
                    continue;
                }

                string normalizedNpcId = Normalize(candidate.npcId);
                if (normalizedNpcId.Length > 0 && !_entryByNpcId.ContainsKey(normalizedNpcId))
                {
                    _entryByNpcId.Add(normalizedNpcId, candidate);
                }

                if (candidate.handPortrait != null && normalizedNpcId.Length > 0)
                {
                    _handPortraitByNpcId[normalizedNpcId] = candidate.handPortrait;
                }

                RegisterSpeakerKey(candidate, candidate.npcId);
                RegisterSpeakerKey(candidate, candidate.canonicalName);
                RegisterSpeakerKey(candidate, candidate.relationshipDisplayName);

                string[] aliases = candidate.speakerAliases ?? Array.Empty<string>();
                for (int aliasIndex = 0; aliasIndex < aliases.Length; aliasIndex++)
                {
                    RegisterSpeakerKey(candidate, aliases[aliasIndex]);
                }

                if (candidate.showInRelationshipPanel && normalizedNpcId.Length > 0)
                {
                    filtered[filteredCount++] = candidate;
                }
            }

            if (filteredCount <= 0)
            {
                _relationshipPanelEntries = Array.Empty<Entry>();
                return;
            }

            _relationshipPanelEntries = new Entry[filteredCount];
            Array.Copy(filtered, _relationshipPanelEntries, filteredCount);
        }

        private void RegisterSpeakerKey(Entry entry, string rawValue)
        {
            if (entry == null)
            {
                return;
            }

            string normalized = Normalize(rawValue);
            if (normalized.Length == 0 || _entryBySpeaker.ContainsKey(normalized))
            {
                return;
            }

            _entryBySpeaker.Add(normalized, entry);
        }
    }
}
