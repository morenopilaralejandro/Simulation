using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public static class CSVImporterParser
{
    
    #region Generic

    public static int ParseInt(string strValue, int defaultValue = 0)
    {
        if (string.IsNullOrWhiteSpace(strValue))
            return defaultValue;

        if (int.TryParse(strValue.Trim(), out int result))
            return result;

        return defaultValue;
    }

    public static bool ParseBool(string strValue)
    {
        return strValue == "TRUE" || strValue == "true" || strValue == "1" || strValue == "yes" || strValue == "YES";
    }

    /// <summary>
    /// Converts a delimited string (like "Move1|Move2") to a list of plain strings.
    /// </summary>
    public static List<string> ParseListString(string strValue, char delimiter = '|')
    {
        var list = new List<string>();
        if (string.IsNullOrWhiteSpace(strValue))
            return list;

        string[] parts = strValue.Split(delimiter);
        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (!string.IsNullOrEmpty(trimmed))
                list.Add(trimmed);
        }

        return list;
    }

    #endregion

    #region Item

    /// <summary>
    /// Parses a formatted string into a List of ItemReward.
    /// Example input: "itemId#5|itemId#3|itemId#10"
    /// </summary>
    public static List<ItemReward> ParseListItemReward(
        string strValue,
        char delimiter1 = '|',   // separates each reward
        char delimiter2 = '#')   // separates itemId from quantity
    {
        var list = new List<ItemReward>();

        // ── Guard clause ──
        if (string.IsNullOrWhiteSpace(strValue))
            return list;

        // ── Split into individual reward tokens ──
        string[] parts = strValue.Split(delimiter1);

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            // ── Split each token into itemId and quantity ──
            string[] subParts = trimmed.Split(delimiter2);

            var reward = new ItemReward();

            // First element → itemId
            reward.ItemId = subParts[0].Trim();

            // Second element (optional) → quantity
            if (subParts.Length >= 2 && int.TryParse(subParts[1].Trim(), out int qty))
            {
                reward.Quantity = qty;
            }
            else
            {
                reward.Quantity = 1; // default fallback
            }

            list.Add(reward);
        }

        return list;
    }

    #endregion


    #region Story

    /// <summary>
    /// Parses a formatted string into a List of StoryPrerequisite.
    /// Example input: "VariableEquals#variableId#10|FlagIsTrue#flagId#1"
    /// </summary>
    public static List<StoryPrerequisite> ParseListStoryPrerequisite(
        string strValue,
        char delimiter1 = '|',
        char delimiter2 = '#')
    {
        var list = new List<StoryPrerequisite>();

        if (string.IsNullOrWhiteSpace(strValue))
            return list;

        string[] parts = strValue.Split(delimiter1);

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed))
                continue;

            string[] subParts = trimmed.Split(delimiter2);

            var storyPrerequisite = new StoryPrerequisite();

            storyPrerequisite.PrerequisiteType = EnumManager.StringToEnum<PrerequisiteType>(subParts[0].Trim());
            storyPrerequisite.TargetId = subParts[1].Trim();
            storyPrerequisite.IntValue = ParseInt(subParts[2].Trim());
 
            list.Add(storyPrerequisite);
        }

        return list;
    }

    /// <summary>
    /// Parses a formatted string into a List of StoryEffect.
    /// Example input: "SetFlag#flagId#true#0|SetVariable#variableId#false#10"
    /// </summary>
    public static List<StoryEffect> ParseListStoryEffect(
        string strValue,
        char delimiter1 = '|',
        char delimiter2 = '#')
    {
        var list = new List<StoryEffect>();

        if (string.IsNullOrWhiteSpace(strValue))
            return list;

        string[] parts = strValue.Split(delimiter1);

        foreach (string part in parts)
        {
            string trimmed = part.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            string[] subParts = trimmed.Split(delimiter2);

            var storyEffect = new StoryEffect();

            storyEffect.EffectType = EnumManager.StringToEnum<StoryEffectType>(subParts[0].Trim());
            storyEffect.TargetId = subParts[1].Trim();
            storyEffect.BoolValue = ParseBool(subParts[2].Trim());
            storyEffect.IntValue = ParseInt(subParts[3].Trim());

            list.Add(storyEffect);
        }

        return list;
    }

    #endregion

}
