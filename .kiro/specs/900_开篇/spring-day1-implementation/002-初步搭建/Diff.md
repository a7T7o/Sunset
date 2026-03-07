  第二步：核心代码 Diff / 伪代码方案（供最终审核）

  1) DialogueManager：StringBuilder + EventBus 核心播放协程

  diff --git a/Assets/YYY_Scripts/Story/Managers/DialogueManager.cs b/Assets/YYY_Scripts/Story/Managers/DialogueManager.cs
  new file mode 100644
  --- /dev/null
  +++ b/Assets/YYY_Scripts/Story/Managers/DialogueManager.cs
  @@
  +using System.Collections;
  +using System.Text;
  +using UnityEngine;
  +using Sunset.Events;
  +
  +public class DialogueManager : MonoBehaviour
  +{
  +    public static DialogueManager Instance { get; private set; }
  +    public bool IsDialogueActive { get; private set; }
  +
  +    private readonly StringBuilder _textBuilder = new StringBuilder(512);
  +    private DialogueSequenceSO _currentSequence;
  +    private Coroutine _playRoutine;
  +    private bool _advanceRequested;
  +    private bool _skipTypingRequested;
  +
  +    private void Awake()
  +    {
  +        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
  +        Instance = this;
  +        DontDestroyOnLoad(gameObject);
  +    }
  +
  +    public void PlayDialogue(DialogueSequenceSO sequence)
  +    {
  +        if (sequence == null || sequence.nodes == null || sequence.nodes.Count == 0) return;
  +        if (_playRoutine != null) StopCoroutine(_playRoutine);
  +        _currentSequence = sequence;
  +        _playRoutine = StartCoroutine(PlaySequenceRoutine(sequence));
  +    }
  +
  +    public void AdvanceDialogue()
  +    {
  +        _skipTypingRequested = true;   // 正在打字时：先一键补全文本
  +        _advanceRequested = true;      // 打字结束后：推进下一节点
  +    }
  +
  +    public void StopDialogue()
  +    {
  +        IsDialogueActive = false;
  +        EventBus.Publish(new DialogueEndEvent { SequenceId = _currentSequence?.sequenceId });
  +        _currentSequence = null;
  +        _playRoutine = null;
  +        _advanceRequested = false;
  +        _skipTypingRequested = false;
  +    }
  +
  +    private IEnumerator PlaySequenceRoutine(DialogueSequenceSO sequence)
  +    {
  +        IsDialogueActive = true;
  +        EventBus.Publish(new DialogueStartEvent { SequenceId = sequence.sequenceId });
  +
  +        for (int i = 0; i < sequence.nodes.Count; i++)
  +        {
  +            var node = sequence.nodes[i];
  +            _advanceRequested = false;
  +            _skipTypingRequested = false;
  +
  +            EventBus.Publish(new DialogueNodeChangedEvent
  +            {
  +                SequenceId = sequence.sequenceId,
  +                NodeIndex = i,
  +                Node = node
  +            });
  +
  +            float cps = node.typingSpeedOverride > 0f ? node.typingSpeedOverride : sequence.defaultTypingSpeed;
  +            yield return StartCoroutine(TypeNodeTextRoutine(node.text, Mathf.Max(1f, cps)));
  +
  +            yield return new WaitUntil(() => _advanceRequested);
  +        }
  +
  +        StopDialogue();
  +    }
  +
  +    private IEnumerator TypeNodeTextRoutine(string fullText, float charsPerSecond)
  +    {
  +        _textBuilder.Clear();
  +        float interval = 1f / charsPerSecond;
  +
  +        for (int i = 0; i < fullText.Length; i++)
  +        {
  +            if (_skipTypingRequested)
  +            {
  +                _textBuilder.Clear();
  +                _textBuilder.Append(fullText);
  +                EventBus.Publish(new DialogueTypingProgressEvent { FullText = _textBuilder.ToString(), IsCompleted = true });
  +                yield break;
  +            }
  +
  +            _textBuilder.Append(fullText[i]);
  +            EventBus.Publish(new DialogueTypingProgressEvent { FullText = _textBuilder.ToString(), IsCompleted = false });
  +            yield return new WaitForSeconds(interval);
  +        }
  +
  +        EventBus.Publish(new DialogueTypingProgressEvent { FullText = _textBuilder.ToString(), IsCompleted = true });
  +    }
  +}

  说明：DialogueTypingProgressEvent 可作为 UI 逐字刷新事件；若你希望继续保持“节点变更事件直接带全文本”，也可把逐字刷新改为 DialogueUI     
  直接调用 DialogueManager 查询当前缓存文本（两种都可）。

  ---
  2) GameInputManager：监听 DialogueStartEvent 并锁输入（接入主链路）

  基于现有 HandleMovement()（Assets/YYY_Scripts/Controller/Input/GameInputManager.cs:437）做最小改动：

  diff --git a/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs b/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs
  --- a/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs
  +++ b/Assets/YYY_Scripts/Controller/Input/GameInputManager.cs
  @@
   using FarmGame.Farm;
  +using Sunset.Events;
  @@
   public class GameInputManager : MonoBehaviour
   {
  +    private bool _dialogueInputLocked = false;
  @@
       void Awake()
       {
  @@
           if (worldCamera == null) worldCamera = Camera.main;
  +
  +        EventBus.Subscribe<DialogueStartEvent>(OnDialogueStart, owner: this);
  +        EventBus.Subscribe<DialogueEndEvent>(OnDialogueEnd, owner: this);
       }
  @@
       void OnDestroy()
       {
  +        EventBus.UnsubscribeAll(this);
           if (s_instance == this)
           {
               s_instance = null;
           }
       }
  +
  +    private void OnDialogueStart(DialogueStartEvent _)
  +    {
  +        _dialogueInputLocked = true;
  +        if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
  +    }
  +
  +    private void OnDialogueEnd(DialogueEndEvent _)
  +    {
  +        _dialogueInputLocked = false;
  +    }
  @@
       void HandleMovement()
       {
  +        if (_dialogueInputLocked)
  +        {
  +            if (playerMovement != null) playerMovement.SetMovementInput(Vector2.zero, false);
  +            return;
  +        }
  +
           // 背包或箱子UI打开时禁用移动输入
           bool uiOpen = IsAnyPanelOpen();

  PlayerMovement 侧无需改结构，继续作为输入消费端（SetMovementInput(...)）即可：Assets/YYY_Scripts/Service/Player/PlayerMovement.cs:124。