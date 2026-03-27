using UnityEngine;

[DisallowMultipleComponent]
public class PlayerToolFeedbackService : MonoBehaviour
{
    private const string BubbleKeyAxeTierInsufficient = "axe-tier-insufficient";
    private const string BubbleKeyAxeTierRecovered = "axe-tier-recovered";
    private const string BubbleKeyToolBroken = "tool-broken";
    private const string BubbleKeyWaterEmpty = "watering-empty";

    private static readonly string[] ToolBrokenLines =
    {
        "这么不耐用吗？",
        "看来得换个好点的了。",
        "哎哟，搞什么鬼？",
        "诶，好吧…",
        "旧的不去新的不来！",
        "碎碎平安~"
    };

    private static readonly string[] AxeTierInsufficientLines =
    {
        "这棵树我现在还砍不动吗？",
        "看来还得换把更锋利的斧头。",
        "这棵树现在还不是我能动的。"
    };

    private static readonly string[] WaterEmptyLines =
    {
        "没水了，先去装满再来吧。",
        "壶里已经一滴都不剩了。",
        "先补点水，再继续浇。"
    };

    [SerializeField] private PlayerThoughtBubblePresenter thoughtBubblePresenter;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private float feedbackSoundVolume = 0.85f;
    [SerializeField] private float burstHeight = 1.25f;

    private string activeBubbleKey;
    private int feedbackSoundDispatchCount;

    public int FeedbackSoundDispatchCount => feedbackSoundDispatchCount;

    public static PlayerToolFeedbackService ResolveForPlayer(GameObject playerRoot)
    {
        GameObject root = playerRoot;
        if (root == null)
        {
            PlayerInteraction interaction = FindFirstObjectByType<PlayerInteraction>();
            if (interaction != null)
            {
                root = interaction.gameObject;
            }
        }

        if (root == null)
        {
            PlayerMovement movement = FindFirstObjectByType<PlayerMovement>();
            if (movement != null)
            {
                root = movement.gameObject;
            }
        }

        if (root == null)
        {
            return null;
        }

        PlayerToolFeedbackService service = root.GetComponent<PlayerToolFeedbackService>();
        if (service == null)
        {
            service = root.AddComponent<PlayerToolFeedbackService>();
        }

        if (root.GetComponent<PlayerNpcNearbyFeedbackService>() == null)
        {
            root.AddComponent<PlayerNpcNearbyFeedbackService>();
        }

        return service;
    }

    private void Awake()
    {
        EnsurePresenter();
    }

    private void OnEnable()
    {
        EnsurePresenter();
        if (thoughtBubblePresenter != null)
        {
            thoughtBubblePresenter.Hidden -= HandleBubbleHidden;
            thoughtBubblePresenter.Hidden += HandleBubbleHidden;
        }
    }

    private void OnDisable()
    {
        if (thoughtBubblePresenter != null)
        {
            thoughtBubblePresenter.Hidden -= HandleBubbleHidden;
        }
    }

    public void HandleToolBroken(FarmGame.Data.ToolData toolData, int slotIndex)
    {
        PlayRejectShake(slotIndex);
        PlayFeedbackSound(toolData != null ? toolData.breakSound : null);
        SpawnBurst(new Color(1f, 0.72f, 0.26f, 1f), 18, 1.1f);
        ShowBubble(BubbleKeyToolBroken, ToolBrokenLines[Random.Range(0, ToolBrokenLines.Length)], 3f, ignoreIfSameKey: false);
    }

    public void HandleWateringCanEmpty(FarmGame.Data.ToolData toolData, int slotIndex)
    {
        bool isRepeatedVisibleBubble = thoughtBubblePresenter != null &&
                                       thoughtBubblePresenter.IsVisible &&
                                       activeBubbleKey == BubbleKeyWaterEmpty;
        if (!isRepeatedVisibleBubble)
        {
            PlayRejectShake(slotIndex);
            PlayFeedbackSound(toolData != null ? toolData.emptyUseSound : null);
            SpawnBurst(new Color(0.36f, 0.74f, 1f, 1f), 14, 0.9f);
        }

        ShowBubble(BubbleKeyWaterEmpty, WaterEmptyLines[Random.Range(0, WaterEmptyLines.Length)], 3f, ignoreIfSameKey: true);
    }

    public void HandleAxeTierInsufficient()
    {
        ShowBubble(BubbleKeyAxeTierInsufficient, AxeTierInsufficientLines[Random.Range(0, AxeTierInsufficientLines.Length)], 5f, ignoreIfSameKey: true);
    }

    public void HandleAxeTierRecovered()
    {
        ShowBubble(BubbleKeyAxeTierRecovered, "还是这把斧头锋利！", 3f, ignoreIfSameKey: true);
    }

    public void PlayTierInsufficientSound(AudioClip clip, Vector3 position, float volume)
    {
        feedbackSoundDispatchCount++;
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
            return;
        }

        PlacementManager.Instance?.PlayFailFeedbackSound();
    }

    private void HandleBubbleHidden()
    {
        activeBubbleKey = string.Empty;
    }

    private void EnsurePresenter()
    {
        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (thoughtBubblePresenter == null)
        {
            thoughtBubblePresenter = GetComponent<PlayerThoughtBubblePresenter>();
        }

        if (thoughtBubblePresenter == null)
        {
            thoughtBubblePresenter = gameObject.AddComponent<PlayerThoughtBubblePresenter>();
        }
    }

    private void ShowBubble(string bubbleKey, string content, float totalDuration, bool ignoreIfSameKey)
    {
        EnsurePresenter();
        if (thoughtBubblePresenter == null)
        {
            return;
        }

        bool isSameKey = thoughtBubblePresenter.IsVisible && activeBubbleKey == bubbleKey;
        if (isSameKey && ignoreIfSameKey)
        {
            return;
        }

        bool restartFadeIn = !thoughtBubblePresenter.IsVisible;
        activeBubbleKey = bubbleKey;
        thoughtBubblePresenter.ShowText(content, totalDuration, restartFadeIn);
    }

    private void PlayRejectShake(int slotIndex)
    {
        if (slotIndex < 0)
        {
            return;
        }

        ToolbarSlotUI.PlayRejectShakeAt(slotIndex);
        InventorySlotUI.PlayRejectShakeAt(slotIndex);
    }

    private void PlayFeedbackSound(AudioClip clip)
    {
        feedbackSoundDispatchCount++;
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, feedbackSoundVolume);
            return;
        }

        PlacementManager.Instance?.PlayFailFeedbackSound();
    }

    private void SpawnBurst(Color color, int particleCount, float speed)
    {
        Vector3 position = transform.position + Vector3.up * burstHeight;
        GameObject burstObject = new GameObject("PlayerToolFeedbackBurst");
        burstObject.SetActive(false);
        burstObject.transform.position = position;

        ParticleSystem particleSystem = burstObject.AddComponent<ParticleSystem>();
        var main = particleSystem.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.4f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.22f, 0.38f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * 0.7f, speed * 1.2f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.04f, 0.10f);
        main.startColor = color;
        main.maxParticles = particleCount;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = particleSystem.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)particleCount) });

        var shape = particleSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.08f;

        burstObject.SetActive(true);
        particleSystem.Play();
        Destroy(burstObject, 1.2f);
    }
}
