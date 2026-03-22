using System.Collections.Generic;
using System.Text;
using FarmGame.Data;
using FarmGame.Data.Core;
using FarmGame.Farm;

/// <summary>
/// Builds tooltip body text from static item data plus optional runtime state.
/// </summary>
public static class ItemTooltipTextBuilder
{
    public static string Build(ItemData itemData, InventoryItem runtimeItem)
    {
        if (itemData == null)
        {
            return string.Empty;
        }

        string baseText = SanitizeBaseTooltip(itemData);
        string runtimeText = BuildRuntimeSection(itemData, runtimeItem);

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

    private static string SanitizeBaseTooltip(ItemData itemData)
    {
        string raw = itemData.GetTooltipText();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return itemData.description ?? string.Empty;
        }

        string trimmed = raw.Trim();
        string title = $"<b>{itemData.itemName}</b>";
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
        if (runtimeItem == null || runtimeItem.IsEmpty)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();

        if (runtimeItem.HasDurability)
        {
            sb.Append("<color=orange>当前耐久: ");
            sb.Append(runtimeItem.CurrentDurability);
            sb.Append('/');
            sb.Append(runtimeItem.MaxDurability);
            sb.Append("</color>");
        }

        if (itemData is SeedData seedData && SeedBagHelper.IsSeedBag(runtimeItem))
        {
            AppendLineBreak(sb);
            sb.Append(SeedBagHelper.IsOpened(runtimeItem)
                ? "<color=cyan>种袋状态: 已开袋</color>"
                : "<color=cyan>种袋状态: 未开袋</color>");

            AppendLineBreak(sb);
            sb.Append("<color=green>剩余种子: ");
            sb.Append(SeedBagHelper.GetRemaining(runtimeItem));
            sb.Append('/');
            sb.Append(seedData.seedsPerBag);
            sb.Append("</color>");

            int currentTotalDays = TimeManager.Instance != null ? TimeManager.Instance.GetTotalDaysPassed() : -1;
            int expireDay = runtimeItem.GetPropertyInt(SeedBagHelper.KEY_EXPIRE_DAY, -1);
            if (currentTotalDays >= 0 && expireDay >= 0)
            {
                AppendLineBreak(sb);
                if (SeedBagHelper.IsExpired(runtimeItem, currentTotalDays))
                {
                    sb.Append("<color=red>保质期: 已过期</color>");
                }
                else
                {
                    sb.Append("<color=yellow>剩余保质期: ");
                    sb.Append(expireDay - currentTotalDays);
                    sb.Append(" 天</color>");
                }
            }
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
}
