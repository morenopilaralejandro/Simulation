using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Parses ink tags (#speaker:name #loc:key etc.) into structured data.
/// 
/// TAG FORMAT REFERENCE:
/// #speaker:character_id   - Who is speaking
/// #loc:localization_key   - Key for Unity Localization table
/// #mood:expression_name   - Character portrait variant
/// #sfx:sound_name         - Sound effect to play
/// #anim:animation_name    - Animation to trigger
/// #cmd:command:param1:p2  - Game commands (give item, etc.)
/// #type:yes_no_yes        - Marks a choice as the "yes" option
/// #type:yes_no_no         - Marks a choice as the "no" option
/// #speed:slow             - Text speed modifier
/// #shake                  - Screen shake
/// #wait:1.5               - Pause for seconds
/// </summary>
public static class InkTagParser
{
    // Cached empty defaults to avoid repeated string allocations.
    private const string EmptyString = "";
    private const string DefaultMood = "neutral";
    private const string DefaultKitId = "none";
    private const string DefaultVariantId = "home";
    private const string DefaultRoleId = "field";

    // Prefix lengths cached to avoid recalculating.
    private const int PrefixSpeaker = 8;  // "speaker:"
    private const int PrefixLoc = 4;      // "loc:"
    private const int PrefixMood = 5;     // "mood:"
    private const int PrefixSfx = 4;      // "sfx:"
    private const int PrefixAnim = 5;     // "anim:"
    private const int PrefixKit = 4;      // "kit:"
    private const int PrefixCmd = 4;      // "cmd:"

    // Reusable split buffer for cmd parsing to avoid repeated array allocations.
    private static readonly char[] s_ColonSeparator = { ':' };

    public static DialogLine ParseLine(string text, List<string> tags)
    {
        var line = new DialogLine
        {
            RawText = text,
            SpeakerId = EmptyString,
            Mood = DefaultMood,
            LocalizationKey = EmptyString,
            SFX = EmptyString,
            Animation = EmptyString,
            DialogKit = new DialogKit()
        };

        if (tags == null) return line;

        for (int i = 0, count = tags.Count; i < count; i++)
        {
            var tag = tags[i];
            if (tag == null || tag.Length == 0) continue;

            // Inline trim — find bounds without allocating a new string.
            int start = 0;
            int end = tag.Length - 1;
            while (start <= end && tag[start] == ' ') start++;
            while (end >= start && tag[end] == ' ') end--;
            int len = end - start + 1;
            if (len <= 0) continue;

            // Use the trimmed view only if whitespace was actually present;
            // otherwise reuse the original reference (zero alloc).
            string trimmed = (start == 0 && end == tag.Length - 1) ? tag : tag.Substring(start, len);

            char c = trimmed[0];

            if (c == 's')
            {
                if (trimmed.StartsWith("speaker:"))
                    line.SpeakerId = trimmed.Substring(PrefixSpeaker);
                else if (trimmed.StartsWith("sfx:"))
                    line.SFX = trimmed.Substring(PrefixSfx);
            }
            else if (c == 'l')
            {
                if (trimmed.StartsWith("loc:"))
                    line.LocalizationKey = trimmed.Substring(PrefixLoc);
            }
            else if (c == 'm')
            {
                if (trimmed.StartsWith("mood:"))
                    line.Mood = trimmed.Substring(PrefixMood);
            }
            else if (c == 'a')
            {
                if (trimmed.StartsWith("anim:"))
                    line.Animation = trimmed.Substring(PrefixAnim);
            }
            else if (c == 'k')
            {
                if (trimmed.StartsWith("kit:"))
                    line.DialogKit = ParseKit(trimmed.Substring(PrefixKit));
            }
            else if (c == 'c')
            {
                if (trimmed.StartsWith("cmd:"))
                {
                    var parts = trimmed.Substring(PrefixCmd).Split(s_ColonSeparator);
                    line.Commands.Add(new DialogCommand
                    {
                        CommandName = parts[0],
                        Parameters = parts.Skip(1).ToArray()
                    });
                }
            }
        }

        return line;
    }

    public static DialogChoice ParseChoice(string text, int index, List<string> tags)
    {
        var choice = new DialogChoice
        {
            Index = index,
            RawText = text,
            LocalizationKey = EmptyString,
            IsYes = false,
            IsNo = false,
            IsYesNoChoice = false
        };

        if (tags == null) return choice;

        for (int i = 0, count = tags.Count; i < count; i++)
        {
            var tag = tags[i];
            if (tag == null || tag.Length == 0) continue;

            int start = 0;
            int end = tag.Length - 1;
            while (start <= end && tag[start] == ' ') start++;
            while (end >= start && tag[end] == ' ') end--;
            int len = end - start + 1;
            if (len <= 0) continue;

            string trimmed = (start == 0 && end == tag.Length - 1) ? tag : tag.Substring(start, len);

            char c = trimmed[0];

            if (c == 'l')
            {
                if (trimmed.StartsWith("loc:"))
                    choice.LocalizationKey = trimmed.Substring(PrefixLoc);
            }
            else if (c == 't')
            {
                if (trimmed == "type:yes_no_yes")
                {
                    choice.IsYes = true;
                    choice.IsYesNoChoice = true;
                }
                else if (trimmed == "type:yes_no_no")
                {
                    choice.IsNo = true;
                    choice.IsYesNoChoice = true;
                }
            }
        }

        return choice;
    }

    /// <summary>
    /// Parse kit tag value.
    /// "plate_armor:gold:captain" → KitId=plate_armor, VariantId=gold, RoleId=captain
    /// "mage_robe:blue"           → KitId=mage_robe, VariantId=blue, RoleId=""
    /// "casual"                   → KitId=casual, VariantId=default, RoleId=""
    /// </summary>
    private static DialogKit ParseKit(string kitValue)
    {
        var parts = kitValue.Split(s_ColonSeparator);

        return new DialogKit
        {
            KitId = parts.Length > 0 ? parts[0] : DefaultKitId,
            VariantId = parts.Length > 1 ? parts[1] : DefaultVariantId,
            RoleId = parts.Length > 2 ? parts[2] : DefaultRoleId
        };
    }
}
