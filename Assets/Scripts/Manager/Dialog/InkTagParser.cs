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
    public static DialogLine ParseLine(string text, List<string> tags)
    {
        var line = new DialogLine
        {
            RawText = text,
            SpeakerId = "",
            Mood = "neutral",
            LocalizationKey = "",
            SFX = "",
            Animation = ""
        };

        if (tags == null) return line;

        foreach (var tag in tags)
        {
            var trimmed = tag.Trim();
            
            if (trimmed.StartsWith("speaker:"))
            {
                line.SpeakerId = trimmed.Substring("speaker:".Length);
            }
            else if (trimmed.StartsWith("loc:"))
            {
                line.LocalizationKey = trimmed.Substring("loc:".Length);
            }
            else if (trimmed.StartsWith("mood:"))
            {
                line.Mood = trimmed.Substring("mood:".Length);
            }
            else if (trimmed.StartsWith("sfx:"))
            {
                line.SFX = trimmed.Substring("sfx:".Length);
            }
            else if (trimmed.StartsWith("anim:"))
            {
                line.Animation = trimmed.Substring("anim:".Length);
            }
            else if (trimmed.StartsWith("cmd:"))
            {
                var parts = trimmed.Substring("cmd:".Length).Split(':');
                line.Commands.Add(new DialogCommand
                {
                    CommandName = parts[0],
                    Parameters = parts.Skip(1).ToArray()
                });
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
            LocalizationKey = "",
            IsYes = false,
            IsNo = false,
            IsYesNoChoice = false
        };

        if (tags == null) return choice;

        foreach (var tag in tags)
        {
            var trimmed = tag.Trim();
            
            if (trimmed.StartsWith("loc:"))
            {
                choice.LocalizationKey = trimmed.Substring("loc:".Length);
            }
            else if (trimmed == "type:yes_no_yes") //yes
            {
                choice.IsYes = true;
                choice.IsYesNoChoice = true;
            }
            else if (trimmed == "type:yes_no_no") //no
            {
                choice.IsNo = true;
                choice.IsYesNoChoice = true;
            }
        }

        return choice;
    }
}
