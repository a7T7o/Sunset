using Sunset.Events;
using Sunset.Story;
using UnityEngine;

[DisallowMultipleComponent]
[DefaultExecutionOrder(120)]
public class PlayerNpcNearbyFeedbackService : MonoBehaviour
{
    [SerializeField] private float probeInterval = 0.4f;
    [SerializeField] private float triggerDistance = 1.65f;
    [SerializeField] private float globalCooldown = 5.5f;
    [SerializeField] private float sameNpcRepeatCooldown = 12f;
    [SerializeField] private float bubbleDuration = 2.6f;
    [SerializeField] private bool requirePlayerIdle = true;

    private float nextProbeAtTime;
    private float nextAllowedFeedbackTime;
    private int lastNpcInstanceId;
    private float lastNpcFeedbackTime = float.NegativeInfinity;
    private PlayerInteraction playerInteraction;
    private bool suppressWhileDialogueActive;
    private NPCBubblePresenter activeNearbyBubblePresenter;
    private string lastNearbyNpcName = string.Empty;
    private string lastNearbyBubbleText = string.Empty;

    public string DebugSummary
    {
        get
        {
            StoryPhase phase = NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
            bool formalPriority = NpcInteractionPriorityPolicy.IsFormalPriorityPhase(phase);
            bool nearbySuppressed = ShouldSuppressNearbyFeedbackForCurrentStory();
            bool activeVisible = activeNearbyBubblePresenter != null && activeNearbyBubblePresenter.IsBubbleVisible;
            string activeNpcName = activeVisible ? activeNearbyBubblePresenter.name : "none";
            string activeBubbleText = activeVisible ? Sanitize(activeNearbyBubblePresenter.CurrentBubbleText) : "none";
            string lastNpcName = string.IsNullOrWhiteSpace(lastNearbyNpcName) ? "none" : Sanitize(lastNearbyNpcName);
            string lastBubble = string.IsNullOrWhiteSpace(lastNearbyBubbleText) ? "none" : Sanitize(lastNearbyBubbleText);
            return $"phase={phase}|formalPriority={formalPriority}|nearbySuppressed={nearbySuppressed}|dialogueSuppressed={suppressWhileDialogueActive}|activeNpc={activeNpcName}|activeBubble={activeBubbleText}|lastNpc={lastNpcName}|lastBubble={lastBubble}";
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        AttachToPlayerRootIfNeeded();
    }

    private void Awake()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<DialogueStartEvent>(HandleDialogueStarted, owner: this);
        EventBus.Subscribe<DialogueEndEvent>(HandleDialogueEnded, owner: this);
        SyncDialogueSuppressionState();
    }

    private void OnDisable()
    {
        HideActiveNearbyBubble();
        EventBus.UnsubscribeAll(this);
        suppressWhileDialogueActive = false;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        SyncDialogueSuppressionState();
        if (activeNearbyBubblePresenter != null &&
            activeNearbyBubblePresenter.TryGetComponent(out NPCAutoRoamController activeController) &&
            activeController.IsResidentScriptedControlActive)
        {
            HideActiveNearbyBubble();
        }

        if (ShouldSuppressNearbyFeedbackForCurrentStory())
        {
            HideActiveNearbyBubble();
            return;
        }

        if (Time.time < nextProbeAtTime)
        {
            return;
        }

        nextProbeAtTime = Time.time + probeInterval;
        TryPlayNearbyFeedback();
    }

    private bool TryPlayNearbyFeedback()
    {
        if (ShouldSuppressNearbyFeedbackForCurrentStory())
        {
            return false;
        }

        if (Time.time < nextAllowedFeedbackTime)
        {
            return false;
        }

        if (requirePlayerIdle && playerInteraction != null && playerInteraction.IsPerformingAction())
        {
            return false;
        }

        NPCAutoRoamController candidate = FindNearestCandidate();
        if (candidate == null)
        {
            return false;
        }

        NPCRoamProfile roamProfile = candidate.RoamProfile;
        if (roamProfile == null)
        {
            return false;
        }

        string npcId = roamProfile.ResolveNpcId(candidate.name);
        NPCRelationshipStage relationshipStage = PlayerNpcRelationshipService.GetStage(npcId);
        StoryPhase storyPhase = NpcInteractionPriorityPolicy.ResolveCurrentStoryPhase();
        string[] lines = roamProfile.GetPlayerNearbyLines(relationshipStage, storyPhase);
        if (!HasAnyLines(lines))
        {
            return false;
        }

        NPCBubblePresenter bubblePresenter = candidate.GetComponent<NPCBubblePresenter>();
        if (bubblePresenter == null || bubblePresenter.IsBubbleVisible)
        {
            return false;
        }

        int npcInstanceId = candidate.GetInstanceID();
        if (npcInstanceId == lastNpcInstanceId && Time.time - lastNpcFeedbackTime < sameNpcRepeatCooldown)
        {
            return false;
        }

        string line = lines[Random.Range(0, lines.Length)];
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        if (!bubblePresenter.ShowText(line, bubbleDuration))
        {
            return false;
        }

        activeNearbyBubblePresenter = bubblePresenter;
        lastNearbyNpcName = candidate.name;
        lastNearbyBubbleText = line;
        lastNpcInstanceId = npcInstanceId;
        lastNpcFeedbackTime = Time.time;
        nextAllowedFeedbackTime = Time.time + globalCooldown;
        return true;
    }

    private bool ShouldSuppressNearbyFeedbackForCurrentStory()
    {
        return suppressWhileDialogueActive ||
               NpcInteractionPriorityPolicy.ShouldSuppressAmbientBubbleForCurrentStory();
    }

    private NPCAutoRoamController FindNearestCandidate()
    {
        Vector3 origin = transform.position;
        float bestDistanceSqr = triggerDistance * triggerDistance;
        NPCAutoRoamController bestCandidate = null;
        NPCAutoRoamController[] controllers = FindObjectsByType<NPCAutoRoamController>(FindObjectsSortMode.None);

        for (int index = 0; index < controllers.Length; index++)
        {
            NPCAutoRoamController candidate = controllers[index];
            if (candidate == null || !candidate.isActiveAndEnabled || candidate.IsResidentScriptedControlActive)
            {
                continue;
            }

            float distanceSqr = (candidate.transform.position - origin).sqrMagnitude;
            if (distanceSqr > bestDistanceSqr)
            {
                continue;
            }

            bestDistanceSqr = distanceSqr;
            bestCandidate = candidate;
        }

        return bestCandidate;
    }

    private static bool HasAnyLines(string[] lines)
    {
        if (lines == null)
        {
            return false;
        }

        for (int index = 0; index < lines.Length; index++)
        {
            if (!string.IsNullOrWhiteSpace(lines[index]))
            {
                return true;
            }
        }

        return false;
    }

    private static void AttachToPlayerRootIfNeeded()
    {
        GameObject playerRoot = FindPlayerRoot();
        if (playerRoot == null || playerRoot.GetComponent<PlayerNpcNearbyFeedbackService>() != null)
        {
            return;
        }

        playerRoot.AddComponent<PlayerNpcNearbyFeedbackService>();
    }

    private static GameObject FindPlayerRoot()
    {
        PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
        if (interaction != null)
        {
            return interaction.gameObject;
        }

        PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
        if (movement != null)
        {
            return movement.gameObject;
        }

        return null;
    }

    private void HandleDialogueStarted(DialogueStartEvent _)
    {
        SetDialogueSuppressed(true);
    }

    private void HandleDialogueEnded(DialogueEndEvent _)
    {
        SetDialogueSuppressed(false);
    }

    private void SyncDialogueSuppressionState()
    {
        bool shouldSuppress = DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive;
        SetDialogueSuppressed(shouldSuppress);

        if (!shouldSuppress && activeNearbyBubblePresenter != null && !activeNearbyBubblePresenter.IsBubbleVisible)
        {
            activeNearbyBubblePresenter = null;
        }
    }

    private void SetDialogueSuppressed(bool shouldSuppress)
    {
        if (suppressWhileDialogueActive == shouldSuppress)
        {
            return;
        }

        suppressWhileDialogueActive = shouldSuppress;
        if (suppressWhileDialogueActive)
        {
            HideActiveNearbyBubble();
        }
    }

    private void HideActiveNearbyBubble()
    {
        if (activeNearbyBubblePresenter == null)
        {
            return;
        }

        if (activeNearbyBubblePresenter.IsBubbleVisible)
        {
            activeNearbyBubblePresenter.HideBubble();
        }

        activeNearbyBubblePresenter = null;
    }

    private static string Sanitize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value
            .Replace('\n', ' ')
            .Replace('\r', ' ')
            .Trim();
    }
}
