using Sunset.Events;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sunset.Story
{
    [DisallowMultipleComponent]
    public class NPCDialogueInteractable : MonoBehaviour, IInteractable
    {
        #region Serialized Fields
        [Header("Dialogue")]
        [FormerlySerializedAs("dialogueSequence")]
        [SerializeField] private DialogueSequenceSO initialSequence;
        [SerializeField] private DialogueSequenceSO followupSequence;
        [SerializeField] private string interactionHint = "对话";
        [SerializeField] private float interactionDistance = 1.5f;
        [SerializeField] private int interactionPriority = 30;
        [SerializeField] private bool playOnlyOnce = false;

        [Header("NPC 对话接轨")]
        [SerializeField] private bool freezeRoamDuringDialogue = true;
        [SerializeField] private bool facePlayerOnInteract = true;
        [SerializeField] private NPCAutoRoamController autoRoamController;
        [SerializeField] private NPCMotionController motionController;
        #endregion

        #region Private Fields
        private bool _hasPlayed;
        private bool _ownsActiveDialogue;
        private bool _resumeRoamAfterDialogue;
        #endregion

        #region Public Properties
        public int InteractionPriority => interactionPriority;
        public float InteractionDistance => interactionDistance;
        #endregion

        #region Unity Lifecycle
        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<DialogueEndEvent>(HandleDialogueEnded, owner: this);
        }

        private void OnDisable()
        {
            EventBus.UnsubscribeAll(this);
            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private void OnValidate()
        {
            CacheComponents();
        }
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
                Debug.LogWarning($"[NPCDialogueInteractable] {name} 未配置可播放的对话序列。");
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

            EnterDialogueOccupation(context);

            _hasPlayed = true;
            manager.PlayDialogue(sequenceToPlay);
        }

        public string GetInteractionHint(InteractionContext context)
        {
            return CanInteract(context) ? interactionHint : string.Empty;
        }
        #endregion

        #region Private Methods
        private void CacheComponents()
        {
            if (autoRoamController == null)
            {
                autoRoamController = GetComponent<NPCAutoRoamController>();
            }

            if (motionController == null)
            {
                motionController = GetComponent<NPCMotionController>();
            }
        }

        private void EnterDialogueOccupation(InteractionContext context)
        {
            CacheComponents();

            _ownsActiveDialogue = true;
            _resumeRoamAfterDialogue = false;

            if (freezeRoamDuringDialogue)
            {
                if (autoRoamController != null && autoRoamController.IsRoaming)
                {
                    _resumeRoamAfterDialogue = true;
                    autoRoamController.StopRoam();
                }
                else if (motionController != null)
                {
                    motionController.StopMotion();
                }
            }

            if (facePlayerOnInteract && motionController != null && context?.PlayerTransform != null)
            {
                Vector2 facingDirection = context.PlayerTransform.position - transform.position;
                motionController.SetFacingDirection(facingDirection);
            }
        }

        private void HandleDialogueEnded(DialogueEndEvent _)
        {
            if (!_ownsActiveDialogue)
            {
                return;
            }

            if (_resumeRoamAfterDialogue && autoRoamController != null && autoRoamController.isActiveAndEnabled)
            {
                autoRoamController.StartRoam();
            }

            _ownsActiveDialogue = false;
            _resumeRoamAfterDialogue = false;
        }

        private DialogueSequenceSO ResolveDialogueSequence(DialogueManager manager)
        {
            if (!HasPlayableNodes(initialSequence))
            {
                return null;
            }

            DialogueSequenceSO resolvedFollowup = HasPlayableNodes(followupSequence)
                ? followupSequence
                : initialSequence.followupSequence;

            if (!HasPlayableNodes(resolvedFollowup))
            {
                return initialSequence;
            }

            StoryManager storyManager = StoryManager.Instance;
            bool languageAlreadyDecoded = initialSequence.markLanguageDecodedOnComplete && storyManager.IsLanguageDecoded;
            bool phaseAlreadyAdvanced = initialSequence.advanceStoryPhaseOnComplete
                && initialSequence.nextStoryPhase != StoryPhase.None
                && storyManager.HasReachedPhase(initialSequence.nextStoryPhase);
            bool sequenceAlreadyCompleted = manager != null && manager.HasCompletedSequence(initialSequence.sequenceId);

            if (languageAlreadyDecoded || phaseAlreadyAdvanced || sequenceAlreadyCompleted)
            {
                return resolvedFollowup;
            }

            return initialSequence;
        }

        private static bool HasPlayableNodes(DialogueSequenceSO sequence)
        {
            return sequence != null && sequence.nodes != null && sequence.nodes.Count > 0;
        }
        #endregion
    }
}
