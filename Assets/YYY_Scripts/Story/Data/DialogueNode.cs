using System;
using UnityEngine;

namespace Sunset.Story
{
    [Serializable]
    public class DialogueNode
    {
        [Header("Content")]
        public string speakerName;

        [TextArea(3, 10)]
        public string text;

        [Header("Garbled")]
        [Tooltip("Whether this node should display garbled text before decode.")]
        public bool isGarbled;

        [TextArea(3, 10)]
        [Tooltip("Text used when the language is not decoded yet.")]
        public string garbledText;

        [Header("Presentation")]
        public Sprite speakerPortrait;
        public bool isInnerMonologue;
        public bool isBubble;

        [Tooltip("Optional font style key from DialogueFontLibrarySO.")]
        public string fontStyleKey;

        [Header("Typing")]
        [Tooltip("Use sequence default speed when <= 0.")]
        public float typingSpeedOverride;
    }
}
