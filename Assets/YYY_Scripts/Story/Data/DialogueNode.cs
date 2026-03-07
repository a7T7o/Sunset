using System;
using UnityEngine;

namespace Sunset.Story
{
    [Serializable]
    public class DialogueNode
    {
        [Header("内容")]
        public string speakerName;

        [TextArea(3, 10)]
        public string text;

        [Header("乱码设定")]
        [Tooltip("是否为无法听懂的乱码语言")]
        public bool isGarbled;

        [TextArea(3, 10)]
        [Tooltip("乱码状态下显示的文本内容")]
        public string garbledText;

        [Header("表现")]
        public Sprite speakerPortrait;
        public bool isInnerMonologue;
        public bool isBubble;

        [Header("打字机")]
        [Tooltip("<=0 时使用序列默认速度")]
        public float typingSpeedOverride;
    }
}
