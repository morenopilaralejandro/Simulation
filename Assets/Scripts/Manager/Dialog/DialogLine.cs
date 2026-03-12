using System.Collections.Generic;

/// <summary>
/// Represents a single parsed line of dialog with all metadata from ink tags.
/// </summary>
public class DialogLine
{
    public string RawText { get; set; }
    public string LocalizationKey { get; set; }
    public string SpeakerId { get; set; }
    public string Mood { get; set; }
    public string SFX { get; set; }
    public string Animation { get; set; }
    public List<DialogCommand> Commands { get; set; } = new List<DialogCommand>();
        
    /// <summary>
    /// The final resolved text after localization and variable substitution.
    /// </summary>
    public string ResolvedText { get; set; }

    public DialogKit DialogKit { get; set; } = new DialogKit();
        
    public bool IsSystemMessage => SpeakerId == "system";
    public bool HasLocalization => !string.IsNullOrEmpty(LocalizationKey);
}
