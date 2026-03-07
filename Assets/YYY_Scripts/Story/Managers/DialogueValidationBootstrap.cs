using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sunset.Story
{
    public class DialogueValidationBootstrap : MonoBehaviour
    {
        #region 序列化字段
        [Header("验证配置")]
        [SerializeField] private bool playOnStart = true;
        [SerializeField] private bool startDecoded = false;
        #endregion

        #region 私有字段
        private DialogueSequenceSO _runtimeSequence;
        private Sprite _runtimePortrait;
        #endregion

        #region Unity生命周期
        private void Start()
        {
            if (!IsValidationSceneActive())
            {
                return;
            }

            if (!playOnStart)
            {
                return;
            }

            PlaySampleDialogue(startDecoded);
        }

        private void OnDestroy()
        {
            if (_runtimePortrait != null)
            {
                Destroy(_runtimePortrait.texture);
                Destroy(_runtimePortrait);
                _runtimePortrait = null;
            }

            if (_runtimeSequence != null)
            {
                Destroy(_runtimeSequence);
                _runtimeSequence = null;
            }
        }
        #endregion

        #region 公共方法
        [ContextMenu("Play Sample Dialogue (Undecoded)")]
        public void PlayUndecodedSampleDialogue()
        {
            PlaySampleDialogue(false);
        }

        [ContextMenu("Play Sample Dialogue (Decoded)")]
        public void PlayDecodedSampleDialogue()
        {
            PlaySampleDialogue(true);
        }
        #endregion

        #region 私有方法
        private void PlaySampleDialogue(bool isDecoded)
        {
            if (!IsValidationSceneActive())
            {
                return;
            }

            DialogueManager manager = DialogueManager.Instance;
            if (manager == null)
            {
                Debug.LogError("[DialogueValidationBootstrap] DialogueManager.Instance is null.");
                return;
            }

            manager.IsLanguageDecoded = isDecoded;
            manager.PlayDialogue(GetOrCreateRuntimeSequence());
        }

        private DialogueSequenceSO GetOrCreateRuntimeSequence()
        {
            if (_runtimeSequence != null)
            {
                return _runtimeSequence;
            }

            _runtimeSequence = ScriptableObject.CreateInstance<DialogueSequenceSO>();
            _runtimeSequence.sequenceId = "validation-sample";
            _runtimeSequence.defaultTypingSpeed = 60f;
            _runtimeSequence.canSkip = true;

            Sprite portrait = GetOrCreateRuntimePortrait();

            _runtimeSequence.nodes.Add(new DialogueNode
            {
                speakerName = "陌生旅人",
                text = "欢迎，你现在已经能听懂这句正式文本了。",
                isGarbled = true,
                garbledText = "▣◇※……咔……嘶……",
                speakerPortrait = portrait,
                isInnerMonologue = false,
                isBubble = false,
                typingSpeedOverride = 0f
            });

            _runtimeSequence.nodes.Add(new DialogueNode
            {
                speakerName = "艾拉",
                text = "这句会始终走正常文本链路，用来验证普通对话显示。",
                isGarbled = false,
                garbledText = string.Empty,
                speakerPortrait = portrait,
                isInnerMonologue = false,
                isBubble = false,
                typingSpeedOverride = 0f
            });

            _runtimeSequence.nodes.Add(new DialogueNode
            {
                speakerName = string.Empty,
                text = "（内心独白：按钮继续可推进，根节点隐藏不会让事件退订。）",
                isGarbled = false,
                garbledText = string.Empty,
                speakerPortrait = null,
                isInnerMonologue = true,
                isBubble = false,
                typingSpeedOverride = 0f
            });

            return _runtimeSequence;
        }

        private Sprite GetOrCreateRuntimePortrait()
        {
            if (_runtimePortrait != null)
            {
                return _runtimePortrait;
            }

            Texture2D texture = new Texture2D(32, 32, TextureFormat.RGBA32, false);
            Color fillColor = new Color(0.38f, 0.67f, 0.95f, 1f);
            Color[] pixels = new Color[32 * 32];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = fillColor;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            _runtimePortrait = Sprite.Create(texture, new Rect(0f, 0f, 32f, 32f), new Vector2(0.5f, 0.5f));
            _runtimePortrait.name = "DialogueValidationPortrait";
            return _runtimePortrait;
        }

        private bool IsValidationSceneActive()
        {
            return SceneManager.GetActiveScene().name == "DialogueValidation";
        }
        #endregion
    }
}
