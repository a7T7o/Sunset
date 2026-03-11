using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCDialogueInteractable : MonoBehaviour, IInteractable
    {
        #region 序列化字段
        [Header("对话配置")]
        [SerializeField] private DialogueSequenceSO dialogueSequence;
        [SerializeField] private string interactionHint = "对话";
        [SerializeField] private float interactionDistance = 1.5f;
        [SerializeField] private int interactionPriority = 30;
        [SerializeField] private bool playOnlyOnce = false;
        #endregion

        #region 私有字段
        private bool _hasPlayed;
        #endregion

        #region 公共属性
        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        #endregion

        #region 公共方法
        public bool CanInteract(InteractionContext context)
        {
            if (dialogueSequence == null)
            {
                return false;
            }

            if (playOnlyOnce && _hasPlayed)
            {
                return false;
            }

            DialogueManager manager = DialogueManager.Instance;
            return manager != null && !manager.IsDialogueActive;
        }

        public void OnInteract(InteractionContext context)
        {
            if (dialogueSequence == null)
            {
                Debug.LogWarning($"[NPCDialogueInteractable] {name} 未配置 dialogueSequence。");
                return;
            }

            DialogueManager manager = DialogueManager.Instance;
            if (manager == null)
            {
                Debug.LogError($"[NPCDialogueInteractable] {name} 找不到 DialogueManager.Instance。");
                return;
            }

            if (manager.IsDialogueActive)
            {
                return;
            }

            _hasPlayed = true;
            manager.PlayDialogue(dialogueSequence);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }
        #endregion
    }
}
