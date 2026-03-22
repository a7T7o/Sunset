using UnityEngine;

/// <summary>
/// 仅用于压测 NPC 气泡布局。
/// 挂在指定测试 NPC 上后，会持续随机播报不同长度的话术。
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Sunset/NPC/Testing/Bubble Stress Talker")]
public class NPCBubbleStressTalker : MonoBehaviour
{
    [SerializeField] private NPCBubblePresenter bubblePresenter;
    [SerializeField] private NPCAutoRoamController roamController;
    [SerializeField] private bool startOnEnable = true;
    [SerializeField] private bool disableRoamWhileTesting = true;
    [SerializeField] private float minGapSeconds = 0.2f;
    [SerializeField] private float maxGapSeconds = 0.7f;
    [SerializeField] private float minDuration = 1.8f;
    [SerializeField] private float maxDuration = 5f;
    [SerializeField] private string[] testLines =
    {
        "嗯。",
        "先缓一缓。",
        "这边风挺舒服的。",
        "我想先把今天的路线再过一遍。",
        "刚刚那边有点吵，这里倒是安静下来了一点。",
        "等会儿再往前走，我想先把脚步和节奏重新放稳。",
        "今天的事情不算少，但也没到慌的时候，先把眼前这几步走顺再说。",
        "要是待会儿大家都往这边经过，我得提前留一点位置，不然又会挤成一团。",
        "我刚才一路走过来，发现这边的光线和空地都还不错，适合停一会儿再决定下一步。",
        "有时候我会故意慢一点，因为太着急反而容易撞到别人，稳住节奏之后整条路都会顺很多。",
        "如果一会儿旁边又有人经过，我希望自己别再像木桩一样卡住，至少得看起来像真的会思考一下要不要让路。",
        "我想测试一下自己说长一点的话时，气泡会不会只是横着变长，而是更自然地同时长高一点，再顺手把文字好好包进去。"
    };

    private int _showCount;
    private string _lastLine = string.Empty;
    private bool _lastShowSucceeded;
    private float _nextSpeakAt;

    public int ShowCount => _showCount;
    public string LastLine => _lastLine;
    public bool LastShowSucceeded => _lastShowSucceeded;
    public NPCBubblePresenter BubblePresenter => bubblePresenter;
    public NPCAutoRoamController RoamController => roamController;
    public bool TestModeEnabled => startOnEnable;
    public bool DisableRoamInTestMode => disableRoamWhileTesting;

    private void Reset()
    {
        bubblePresenter = GetComponent<NPCBubblePresenter>();
        roamController = GetComponent<NPCAutoRoamController>();
    }

    private void Awake()
    {
        CacheComponents();
    }

    private void OnValidate()
    {
        CacheComponents();
        minGapSeconds = Mathf.Max(0.05f, minGapSeconds);
        maxGapSeconds = Mathf.Max(minGapSeconds, maxGapSeconds);
        minDuration = Mathf.Max(0.5f, minDuration);
        maxDuration = Mathf.Max(minDuration, maxDuration);
        testLines ??= System.Array.Empty<string>();
    }

    private void OnEnable()
    {
        if (!Application.isPlaying || !startOnEnable)
        {
            return;
        }

        if (disableRoamWhileTesting && roamController != null)
        {
            roamController.enabled = false;
        }
        _nextSpeakAt = Time.unscaledTime + 0.05f;
    }

    private void OnDisable()
    {
        if (Application.isPlaying && disableRoamWhileTesting && roamController != null)
        {
            roamController.enabled = true;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying || !startOnEnable)
        {
            return;
        }

        if (Time.unscaledTime < _nextSpeakAt)
        {
            return;
        }

        if (bubblePresenter == null || testLines == null || testLines.Length == 0)
        {
            _nextSpeakAt = Time.unscaledTime + 0.5f;
            return;
        }

        TrySpeakOnce();
        _nextSpeakAt = Time.unscaledTime + Random.Range(minGapSeconds, maxGapSeconds);
    }

    public void RebindReferences()
    {
        CacheComponents();
    }

    public bool TrySpeakOnce()
    {
        CacheComponents();
        if (bubblePresenter == null || testLines == null || testLines.Length == 0)
        {
            _lastShowSucceeded = false;
            return false;
        }

        string line = testLines[Random.Range(0, testLines.Length)];
        float duration = Mathf.Clamp(minDuration + (line.Length * 0.05f), minDuration, maxDuration);
        _showCount++;
        _lastLine = line;
        _lastShowSucceeded = bubblePresenter.ShowText(line, duration);
        return _lastShowSucceeded;
    }

    private void CacheComponents()
    {
        if (bubblePresenter == null)
        {
            bubblePresenter = GetComponent<NPCBubblePresenter>();
        }

        if (roamController == null)
        {
            roamController = GetComponent<NPCAutoRoamController>();
        }
    }
}
