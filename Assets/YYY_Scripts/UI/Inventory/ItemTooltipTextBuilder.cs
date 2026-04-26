using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.Farm;

/// <summary>
/// Builds tooltip body text from static item data plus optional runtime state.
/// </summary>
public static class ItemTooltipTextBuilder
{
    private const int TooltipDescriptionMaxLines = 3;
    private const int TooltipDescriptionMaxChars = 72;
    private static readonly Regex RichTextTagRegex = new Regex("<.*?>", RegexOptions.Compiled);

    public static string Build(ItemData itemData, InventoryItem runtimeItem)
    {
        if (itemData == null)
        {
            return string.Empty;
        }

        string baseText = BuildDescriptionText(itemData);
        string runtimeText = BuildRuntimeStatusText(itemData, runtimeItem);

        if (string.IsNullOrWhiteSpace(runtimeText))
        {
            return baseText;
        }

        if (string.IsNullOrWhiteSpace(baseText))
        {
            return runtimeText;
        }

        return $"{baseText}\n\n{runtimeText}";
    }

    public static string BuildDescriptionText(ItemData itemData)
    {
        return ClampForTooltipDisplay(SanitizeBaseTooltip(itemData));
    }

    public static string BuildPlayerFacingTitle(ItemData itemData)
    {
        if (itemData == null)
        {
            return string.Empty;
        }

        string mappedName = GetMappedPlayerFacingItemName(itemData.itemID);
        if (!string.IsNullOrWhiteSpace(mappedName))
        {
            return mappedName;
        }

        string rawTitle = StripRichTextTags(itemData.itemName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(rawTitle))
        {
            return $"物品 {itemData.itemID}";
        }

        string normalized = NormalizeInternalName(rawTitle);
        return !string.IsNullOrWhiteSpace(normalized) ? normalized : rawTitle;
    }

    public static string BuildRuntimeStatusText(ItemData itemData, InventoryItem runtimeItem)
    {
        return BuildRuntimeSection(itemData, runtimeItem);
    }

    private static string SanitizeBaseTooltip(ItemData itemData)
    {
        string raw = itemData.GetTooltipText();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return StripRichTextTags(itemData.description ?? string.Empty);
        }

        string trimmed = StripRichTextTags(raw).Trim();
        string title = StripRichTextTags(itemData.itemName ?? string.Empty).Trim();
        if (trimmed.StartsWith(title))
        {
            trimmed = trimmed.Substring(title.Length).TrimStart('\r', '\n', ' ');
        }

        var lines = trimmed.Split('\n');
        var keptLines = new List<string>(lines.Length);
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line))
            {
                if (keptLines.Count > 0 && keptLines[^1].Length > 0)
                {
                    keptLines.Add(string.Empty);
                }
                continue;
            }

            if (line.Contains("售价:") || line.Contains("购买:"))
            {
                continue;
            }

            keptLines.Add(line);
        }

        while (keptLines.Count > 0 && string.IsNullOrEmpty(keptLines[^1]))
        {
            keptLines.RemoveAt(keptLines.Count - 1);
        }

        return string.Join("\n", keptLines);
    }

    private static string BuildRuntimeSection(ItemData itemData, InventoryItem runtimeItem)
    {
        InventoryItem displayRuntimeItem = runtimeItem;
        if ((displayRuntimeItem == null || displayRuntimeItem.IsEmpty) && itemData is ToolData fallbackToolData)
        {
            displayRuntimeItem = ToolRuntimeUtility.CreateRuntimeItem(fallbackToolData, 0, 1);
        }
        else if ((displayRuntimeItem == null || displayRuntimeItem.IsEmpty) && itemData is WeaponData fallbackWeaponData)
        {
            displayRuntimeItem = ToolRuntimeUtility.CreateRuntimeItem(fallbackWeaponData, 0, 1);
        }

        if (displayRuntimeItem == null || displayRuntimeItem.IsEmpty)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        if (itemData is ToolData toolData && ToolRuntimeUtility.UsesWater(toolData))
        {
            int currentWater = ToolRuntimeUtility.GetCurrentWater(displayRuntimeItem, toolData);
            int maxWater = ToolRuntimeUtility.GetWaterCapacity(toolData);
            sb.Append("当前水量 ");
            sb.Append(currentWater);
            sb.Append('/');
            sb.Append(maxWater);
        }
        else if (displayRuntimeItem.HasDurability)
        {
            sb.Append("当前耐久 ");
            sb.Append(displayRuntimeItem.CurrentDurability);
            sb.Append('/');
            sb.Append(displayRuntimeItem.MaxDurability);
        }

        if (itemData is SeedData seedData && SeedBagHelper.IsSeedBag(displayRuntimeItem))
        {
            AppendLineBreak(sb);
            sb.Append(SeedBagHelper.IsOpened(displayRuntimeItem)
                ? "种袋状态 已开袋"
                : "种袋状态 未开袋");

            AppendLineBreak(sb);
            sb.Append("剩余种子 ");
            sb.Append(SeedBagHelper.GetRemaining(displayRuntimeItem));
            sb.Append('/');
            sb.Append(seedData.seedsPerBag);

            int currentTotalDays = TimeManager.Instance != null ? TimeManager.Instance.GetTotalDaysPassed() : -1;
            int expireDay = displayRuntimeItem.GetPropertyInt(SeedBagHelper.KEY_EXPIRE_DAY, -1);
            if (currentTotalDays >= 0 && expireDay >= 0)
            {
                AppendLineBreak(sb);
                if (SeedBagHelper.IsExpired(displayRuntimeItem, currentTotalDays))
                {
                    sb.Append("保质期 已过期");
                }
                else
                {
                    sb.Append("剩余保质期 ");
                    sb.Append(expireDay - currentTotalDays);
                    sb.Append("天");
                }
            }
        }

        return sb.ToString();
    }

    private static string GetMappedPlayerFacingItemName(int itemId)
    {
        return itemId switch
        {
            0 => "木斧",
            1 => "石斧",
            2 => "铁斧",
            3 => "黄铜斧",
            4 => "钢斧",
            5 => "金斧",
            6 => "木镐",
            7 => "石镐",
            8 => "铁镐",
            9 => "黄铜镐",
            10 => "钢镐",
            11 => "金镐",
            12 => "木锄",
            13 => "石锄",
            14 => "铁锄",
            15 => "黄铜锄",
            16 => "钢锄",
            17 => "金锄",
            18 => "洒水壶",
            200 => "木剑",
            201 => "石剑",
            202 => "铁剑",
            203 => "黄铜剑",
            204 => "钢剑",
            205 => "金剑",
            1400 => "小木箱子",
            1401 => "大木箱子",
            1402 => "小铁箱子",
            1403 => "大铁箱子",
            _ => string.Empty
        };
    }

    private static string NormalizeInternalName(string rawTitle)
    {
        string lower = rawTitle.ToLowerInvariant();
        if (lower.Contains("wateringcan"))
        {
            return "洒水壶";
        }

        if (lower.Contains("pickaxe"))
        {
            return "镐子";
        }

        if (lower.Contains("axe"))
        {
            return "斧头";
        }

        if (lower.Contains("hoe"))
        {
            return "锄头";
        }

        if (lower.Contains("storage"))
        {
            return "箱子";
        }

        if (lower.Contains("sword"))
        {
            return "剑";
        }

        return rawTitle.Replace('_', ' ').Trim();
    }

    private static string ClampForTooltipDisplay(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        string normalized = text.Replace("\r\n", "\n").Replace('\r', '\n').Trim();
        string[] rawLines = normalized.Split('\n');
        var sb = new StringBuilder();
        int lineCount = 0;
        int charCount = 0;
        bool truncated = false;

        for (int index = 0; index < rawLines.Length; index++)
        {
            string line = rawLines[index].Trim();
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            if (lineCount >= TooltipDescriptionMaxLines || charCount >= TooltipDescriptionMaxChars)
            {
                truncated = true;
                break;
            }

            int remainingChars = TooltipDescriptionMaxChars - charCount;
            if (remainingChars <= 0)
            {
                truncated = true;
                break;
            }

            if (line.Length > remainingChars)
            {
                line = line.Substring(0, remainingChars);
                truncated = true;
            }

            if (sb.Length > 0)
            {
                sb.Append('\n');
            }

            sb.Append(line);
            charCount += line.Length;
            lineCount++;

            if (truncated)
            {
                break;
            }
        }

        if (sb.Length == 0)
        {
            return string.Empty;
        }

        if (truncated && sb[sb.Length - 1] != '…')
        {
            sb.Append('…');
        }

        return sb.ToString();
    }

    private static void AppendLineBreak(StringBuilder sb)
    {
        if (sb.Length > 0)
        {
            sb.Append('\n');
        }
    }

    private static string StripRichTextTags(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        return RichTextTagRegex.Replace(text, string.Empty);
    }
}
