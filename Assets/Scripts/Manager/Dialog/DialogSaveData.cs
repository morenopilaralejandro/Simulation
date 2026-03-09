using System;
using System.Collections.Generic;

/// <summary>
/// Serializable save data for the dialog system.
/// Ink tracks its own state (which knots were visited, variable values, etc.)
/// We save the raw JSON from ink's state plus any extra tracking we do.
/// </summary>
[Serializable]
public class DialogSaveData
{
    public List<StorySaveEntry> storyStates = new List<StorySaveEntry>();
    public List<string> viewedDialogIds = new List<string>();
}
