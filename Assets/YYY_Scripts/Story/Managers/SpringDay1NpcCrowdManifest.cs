using System;
using UnityEngine;

namespace Sunset.Story
{
    [CreateAssetMenu(fileName = "SpringDay1NpcCrowdManifest", menuName = "Sunset/Story/Spring Day1 NPC Crowd Manifest", order = 260)]
    public sealed class SpringDay1NpcCrowdManifest : ScriptableObject
    {
        [Serializable]
        public sealed class Entry
        {
            public string npcId = string.Empty;
            public string displayName = string.Empty;
            [TextArea] public string roleSummary = string.Empty;
            public GameObject prefab;
            [Tooltip("支持用 | 分隔多个候选锚点名，运行时会按顺序查找。")]
            public string anchorObjectName = string.Empty;
            public Vector2 spawnOffset = Vector2.zero;
            public Vector2 fallbackWorldPosition = Vector2.zero;
            public Vector2 initialFacing = Vector2.down;
            public StoryPhase minPhase = StoryPhase.EnterVillage;
            public StoryPhase maxPhase = StoryPhase.DayEnd;

            public bool CanAppear(StoryPhase phase)
            {
                if (phase == StoryPhase.None)
                {
                    return false;
                }

                return phase >= minPhase && phase <= maxPhase;
            }
        }

        [SerializeField] private Entry[] entries = Array.Empty<Entry>();

        public Entry[] Entries => entries ?? Array.Empty<Entry>();

#if UNITY_EDITOR
        public void SetEntries(Entry[] newEntries)
        {
            entries = newEntries ?? Array.Empty<Entry>();
        }
#endif
    }
}
