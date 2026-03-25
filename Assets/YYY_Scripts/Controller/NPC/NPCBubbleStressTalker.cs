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
    [SerializeField] private bool startOnEnable = false;
    [SerializeField] private bool disableRoamWhileTesting = true;
    [SerializeField] private bool sequentialPlayback = true;
    [SerializeField] private float minGapSeconds = 1.8f;
    [SerializeField] private float maxGapSeconds = 2.6f;
    [SerializeField] private float minDuration = 2.6f;
    [SerializeField] private float maxDuration = 6.2f;
    [SerializeField] private string[] testLines =
    {
        "嗯。",
        "先缓一缓。",
        "这边有点舒服。",
        "这边风还挺舒服的。",
        "等会儿再往前走吧。",
        "刚刚那边有点吵，这里安静了一点。",
        "我想先把脚步和节奏重新放稳。",
        "今天事情不少，先把眼前几步走顺再说。",
        "要是待会儿大家都往这边经过，我得提前留点位置。",
        "我刚才一路走过来，发现这边光线和空地都还不错。",
        "有时候我会故意慢一点，因为太着急反而容易撞到别人。",
        "我想测试一下长一点的话时，气泡会不会按十个字一行去换行。"
    };

    private int _showCount;
    private string _lastLine = string.Empty;
    private bool _lastShowSucceeded;
    private float _nextSpeakAt;
    private int _nextLineIndex;

    public int ShowCount => _showCount;
    public string LastLine => _lastLine;
    public bool LastShowSucceeded => _lastShowSucceeded;
    public NPCBubblePresenter BubblePresenter => bubblePresenter;
    public NPCAutoRoamController RoamController => roamController;
    public bool TestModeEnabled => startOnEnable;
    public bool StartOnEnable => startOnEnable;
    public bool DisableRoamWhileTesting => disableRoamWhileTesting;
    public bool DisableRoamInTestMode => disableRoamWhileTesting;
    public bool SequentialPlayback => sequentialPlayback;
    public int NextLineIndex => _nextLineIndex;

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
        _nextLineIndex = 0;
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

    public void ConfigureMode(bool enableOnStart, bool disableRoamDuringTest = true)
    {
        startOnEnable = enableOnStart;
        disableRoamWhileTesting = disableRoamDuringTest;
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

        string line = GetNextLine();
        float duration = Mathf.Clamp(minDuration + (line.Length * 0.05f), minDuration, maxDuration);
        _showCount++;
        _lastLine = line;
        _lastShowSucceeded = bubblePresenter.ShowText(line, duration);
        return _lastShowSucceeded;
    }

    private string GetNextLine()
    {
        if (testLines == null || testLines.Length == 0)
        {
            return string.Empty;
        }

        if (!sequentialPlayback)
        {
            return testLines[Random.Range(0, testLines.Length)];
        }

        int resolvedIndex = Mathf.Clamp(_nextLineIndex, 0, testLines.Length - 1);
        string line = testLines[resolvedIndex];
        _nextLineIndex = (_nextLineIndex + 1) % testLines.Length;
        return line;
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
