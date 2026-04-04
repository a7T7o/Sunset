using System;
using UnityEngine;

namespace Sunset.Story
{
    public static class InteractionHintDisplaySettings
    {
        private const string PlayerPrefsKey = "Sunset.UI.InteractionHintsVisible";

        public static event Action<bool> VisibilityChanged;

        public static bool AreHintsVisible => PlayerPrefs.GetInt(PlayerPrefsKey, 1) == 1;

        public static void SetHintsVisible(bool visible)
        {
            if (AreHintsVisible == visible)
            {
                return;
            }

            PlayerPrefs.SetInt(PlayerPrefsKey, visible ? 1 : 0);
            PlayerPrefs.Save();
            VisibilityChanged?.Invoke(visible);
        }

        public static void ToggleHintsVisible()
        {
            SetHintsVisible(!AreHintsVisible);
        }
    }
}
