using TMPro;
using Sunset.Story;
using UnityEngine;
using UnityEngine.UI;

internal static class PackagePanelRuntimeUiKit
{
    private static readonly Color DefaultOutlineTint = new Color(0.30f, 0.20f, 0.12f, 0.22f);
    private static TMP_FontAsset s_cachedFont;

    public static RectTransform CreateRect(string name, Transform parent)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = 5;

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.localScale = Vector3.one;
        rect.localRotation = Quaternion.identity;
        rect.anchoredPosition3D = Vector3.zero;
        return rect;
    }

    public static void Stretch(RectTransform rect, float left, float right, float top, float bottom)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    public static void SetAnchors(
        RectTransform rect,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        Vector2 pivot)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
        rect.pivot = pivot;
    }

    public static Image AddImage(GameObject go, Color color, bool raycastTarget = false)
    {
        Image image = go.GetComponent<Image>();
        if (image == null)
        {
            image = go.AddComponent<Image>();
        }

        image.color = color;
        image.raycastTarget = raycastTarget;
        return image;
    }

    public static Outline AddOutline(GameObject go, Color color, Vector2 distance)
    {
        Outline outline = go.GetComponent<Outline>();
        if (outline == null)
        {
            outline = go.AddComponent<Outline>();
        }

        outline.effectColor = color;
        outline.effectDistance = distance;
        return outline;
    }

    public static RectTransform CreatePanel(
        string name,
        Transform parent,
        Color color,
        bool raycastTarget = false,
        Color? outlineColor = null,
        Vector2? outlineDistance = null)
    {
        RectTransform rect = CreateRect(name, parent);
        AddImage(rect.gameObject, color, raycastTarget);
        AddOutline(rect.gameObject, outlineColor ?? DefaultOutlineTint, outlineDistance ?? new Vector2(1f, -1f));
        return rect;
    }

    public static TextMeshProUGUI CreateText(
        string name,
        Transform parent,
        string value,
        float fontSize,
        Color color,
        FontStyles fontStyle = FontStyles.Bold,
        TextAlignmentOptions alignment = TextAlignmentOptions.Left,
        bool wordWrap = true,
        TextOverflowModes overflow = TextOverflowModes.Ellipsis)
    {
        RectTransform rect = CreateRect(name, parent);
        TextMeshProUGUI text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        text.text = value ?? string.Empty;
        text.fontSize = fontSize;
        text.fontStyle = fontStyle;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = wordWrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
        text.overflowMode = overflow;
        text.raycastTarget = false;
        text.margin = Vector4.zero;
        text.extraPadding = false;

        TMP_FontAsset resolvedFont = ResolveFont(value);
        if (resolvedFont != null)
        {
            text.font = resolvedFont;
        }

        return text;
    }

    public static Button AddButton(GameObject go, Image targetGraphic)
    {
        Button button = go.GetComponent<Button>();
        if (button == null)
        {
            button = go.AddComponent<Button>();
        }

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = Color.white;
        colors.pressedColor = new Color(0.92f, 0.92f, 0.92f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(0.80f, 0.80f, 0.80f, 0.72f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.08f;
        button.colors = colors;
        button.targetGraphic = targetGraphic;
        return button;
    }

    public static void DestroyChildren(Transform parent)
    {
        if (parent == null)
        {
            return;
        }

        for (int index = parent.childCount - 1; index >= 0; index--)
        {
            Transform child = parent.GetChild(index);
            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    public static TMP_FontAsset ResolveFont(string sampleText = null)
    {
        if (s_cachedFont != null)
        {
            TMP_FontAsset cachedResolved = DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(sampleText, s_cachedFont);
            if (cachedResolved != null)
            {
                return cachedResolved;
            }
        }

        DialogueChineseFontRuntimeBootstrap.EnsureRuntimeFontReady();
        TMP_FontAsset resolved = DialogueChineseFontRuntimeBootstrap.ResolveBestFontForText(sampleText);
        if (resolved != null)
        {
            s_cachedFont = resolved;
        }

        return resolved;
    }
}
