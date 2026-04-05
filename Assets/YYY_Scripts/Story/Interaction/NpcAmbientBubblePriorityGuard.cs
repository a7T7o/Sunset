using System;
using System.Reflection;
using UnityEngine;

namespace Sunset.Story
{
    /// <summary>
    /// 在正式剧情阶段兜底压住 NPC own 的环境气泡，
    /// 避免 ambient / pair bubble 抢到 formal 主语义前面。
    /// </summary>
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(185)]
    public sealed class NpcAmbientBubblePriorityGuard : MonoBehaviour
    {
        private static readonly FieldInfo ChannelPriorityField =
            typeof(NPCBubblePresenter).GetField("_channelPriority", BindingFlags.Instance | BindingFlags.NonPublic);

        private static NpcAmbientBubblePriorityGuard _instance;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void BootstrapRuntime()
        {
            EnsureRuntime();
        }

        public static void EnsureRuntime()
        {
            if (_instance != null)
            {
                return;
            }

            NpcAmbientBubblePriorityGuard existing =
                FindFirstObjectByType<NpcAmbientBubblePriorityGuard>(FindObjectsInactive.Include);
            if (existing != null)
            {
                _instance = existing;
                return;
            }

            GameObject runtimeObject = new GameObject(nameof(NpcAmbientBubblePriorityGuard));
            _instance = runtimeObject.AddComponent<NpcAmbientBubblePriorityGuard>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        private void LateUpdate()
        {
            if (!Application.isPlaying ||
                !NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory())
            {
                return;
            }

            SuppressVisibleAmbientBubbles();
        }

        private static void SuppressVisibleAmbientBubbles()
        {
            NPCBubblePresenter[] presenters = FindObjectsByType<NPCBubblePresenter>(FindObjectsSortMode.None);
            for (int index = 0; index < presenters.Length; index++)
            {
                NPCBubblePresenter presenter = presenters[index];
                if (presenter == null || !presenter.IsBubbleVisible || !IsAmbientBubble(presenter))
                {
                    continue;
                }

                presenter.HideBubble();
            }
        }

        private static bool IsAmbientBubble(NPCBubblePresenter presenter)
        {
            if (presenter == null || ChannelPriorityField == null)
            {
                return false;
            }

            object priorityValue = ChannelPriorityField.GetValue(presenter);
            return string.Equals(priorityValue?.ToString(), "Ambient", StringComparison.Ordinal);
        }
    }
}
