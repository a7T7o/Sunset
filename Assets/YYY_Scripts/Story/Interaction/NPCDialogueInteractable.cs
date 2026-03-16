using UnityEngine;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCDialogueInteractable : MonoBehaviour, IInteractable
    {
        #region Serialized Fields
        [Header("对话配置")]
        [SerializeField] private DialogueSequenceSO dialogueSequence;
        [SerializeField] private string interactionHint = "对话";
        [SerializeField] private float interactionDistance = 1.5f;
        [SerializeField] private int interactionPriority = 30;
        [SerializeField] private bool playOnlyOnce = false;
        #endregion

        #region Private Fields
        private bool _hasPlayed;
        #endregion

        #region Public Properties
        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        #endregion

        #region Public Methods
        public bool CanInteract(InteractionContext context)
        {
            DialogueManager manager = DialogueManager.Instance;
            DialogueSequenceSO sequenceToPlay = ResolveDialogueSequence(manager);

            if (sequenceToPlay == null)
            {
                return false;
            }

            if (playOnlyOnce && _hasPlayed)
            {
                return false;
            }

            return manager != null && !manager.IsDialogueActive;
        }

        public void OnInteract(InteractionContext context)
        {
            DialogueManager manager = DialogueManager.Instance;
            DialogueSequenceSO sequenceToPlay = ResolveDialogueSequence(manager);

            if (sequenceToPlay == null)
            {
                Debug.LogWarning($"[NPCDialogueInteractable] {name} 未配置可播放的 dialogueSequence。");
                return;
            }

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
            manager.PlayDialogue(sequenceToPlay);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }
        #endregion

        #region Private Methods
        private DialogueSequenceSO ResolveDialogueSequence(DialogueManager manager)
        {
            if (!HasPlayableNodes(dialogueSequence))
            {
                return null;
            }

            if (manager == null || dialogueSequence.followupSequence == null)
            {
                return dialogueSequence;
            }

            bool hasCompletedInitialSequence = manager.HasCompletedSequence(dialogueSequence.sequenceId);
            bool languageAlreadyDecoded = dialogueSequence.markLanguageDecodedOnComplete && manager.IsLanguageDecoded;

            if ((hasCompletedInitialSequence || languageAlreadyDecoded) && HasPlayableNodes(dialogueSequence.followupSequence))
            {
                return dialogueSequence.followupSequence;
            }

            return dialogueSequence;
        }

        private static bool HasPlayableNodes(DialogueSequenceSO sequence)
        {
            return sequence != null && sequence.nodes != null && sequence.nodes.Count > 0;
        }
        #endregion
    }
}
