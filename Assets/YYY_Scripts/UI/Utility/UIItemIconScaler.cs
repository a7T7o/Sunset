using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 物品图标缩放适配工具
/// 用于统一处理背包/快捷栏/装备栏中的物品图标显示
/// 确保不同大小的Sprite都能等比例适配显示区域
/// 支持 45 度旋转显示，与世界物品视觉风格一致
/// ★ 修复：旋转后使用实际边界尺寸，而非原始尺寸
/// </summary>
public static class UIItemIconScaler
{
    #region 常量配置
    
    // 槽位配置
    private const float SLOT_SIZE = 64f;           // 槽位总大小（像素）
    private const float BORDER_SIZE = 4f;          // 边框大小（像素）
    private const float DISPLAY_AREA = 56f;        // 实际显示区域（56x56）
    private const float PADDING = 2f;              // 内边距（像素）
    private const float PIXELS_PER_UNIT = 16f;     // 所有sprite的PPU统一为16
    
    // 图标旋转配置
    private const float ICON_ROTATION_Z = 45f;     // 图标 Z 轴旋转角度（与世界物品一致）
    
    #endregion
    
    /// <summary>
    /// 为Image组件设置sprite并自动缩放适配（含 45 度旋转）
    /// ★ 修复：RectTransform 尺寸使用旋转后的实际边界尺寸
    /// </summary>
    /// <param name="image">目标Image组件</param>
    /// <param name="sprite">要显示的sprite（可为null）</param>
    public static void SetIconWithAutoScale(Image image, Sprite sprite)
    {
        if (image == null) return;
        
        // 设置sprite
        image.sprite = sprite;
        
        if (sprite == null)
        {
            image.enabled = false;
            return;
        }
        
        image.enabled = true;
        
        // 重置Image的基本设置
        image.preserveAspect = true;  // 保持宽高比
        image.type = Image.Type.Simple;
        
        // 计算sprite的像素尺寸
        Rect rect = sprite.rect;
        float spriteWidthInPixels = rect.width;
        float spriteHeightInPixels = rect.height;
        
        // ★ 计算旋转后的边界框尺寸（像素）
        float rotRad = ICON_ROTATION_Z * Mathf.Deg2Rad;
        float cos = Mathf.Abs(Mathf.Cos(rotRad));
        float sin = Mathf.Abs(Mathf.Sin(rotRad));
        float rotatedWidthInPixels = spriteWidthInPixels * cos + spriteHeightInPixels * sin;
        float rotatedHeightInPixels = spriteWidthInPixels * sin + spriteHeightInPixels * cos;
        
        // 可用显示区域（减去内边距）
        float availableArea = DISPLAY_AREA - PADDING * 2;  // 56 - 4 = 52 像素
        
        // ★ 使用旋转后边界框计算缩放比例
        float scaleX = availableArea / rotatedWidthInPixels;
        float scaleY = availableArea / rotatedHeightInPixels;
        float scale = Mathf.Min(scaleX, scaleY);
        
        // 应用缩放到RectTransform
        RectTransform rt = image.rectTransform;
        
        // 设置为居中锚点
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        
        // ★ 应用 45 度旋转（与世界物品视觉风格一致）
        rt.localRotation = Quaternion.Euler(0f, 0f, ICON_ROTATION_Z);
        
        // ★ 关键修复：RectTransform 尺寸应该是旋转后的边界尺寸
        // 这样旋转后的图标才能正确填充槽位
        float finalWidth = rotatedWidthInPixels * scale;
        float finalHeight = rotatedHeightInPixels * scale;
        
        // 设置RectTransform的sizeDelta（像素单位）
        rt.sizeDelta = new Vector2(finalWidth, finalHeight);
        
        // 居中显示
        rt.anchoredPosition = Vector2.zero;
    }
    
    /// <summary>
    /// 批量处理：为多个槽位设置图标
    /// </summary>
    public static void SetIconsWithAutoScale(Image[] images, Sprite[] sprites)
    {
        if (images == null || sprites == null) return;
        
        int count = Mathf.Min(images.Length, sprites.Length);
        for (int i = 0; i < count; i++)
        {
            SetIconWithAutoScale(images[i], sprites[i]);
        }
    }
    
    /// <summary>
    /// 获取推荐的槽位尺寸配置（用于调试和文档）
    /// </summary>
    public static string GetSlotConfiguration()
    {
        return $"槽位配置:\n" +
               $"- 槽位总大小: {SLOT_SIZE}x{SLOT_SIZE} 像素\n" +
               $"- 边框大小: {BORDER_SIZE} 像素\n" +
               $"- 实际显示区域: {DISPLAY_AREA}x{DISPLAY_AREA} 像素\n" +
               $"- 内边距: {PADDING} 像素\n" +
               $"- 可用区域: {DISPLAY_AREA - PADDING * 2}x{DISPLAY_AREA - PADDING * 2} 像素\n" +
               $"- Sprite PPU: {PIXELS_PER_UNIT}\n" +
               $"- 图标旋转角度: {ICON_ROTATION_Z}°";
    }
}
