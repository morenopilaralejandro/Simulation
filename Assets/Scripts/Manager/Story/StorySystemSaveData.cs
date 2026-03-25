using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class StorySystemSaveData
{
    public Dictionary<string, bool> FlagsDict;

    public Dictionary<string, int> VariablesDict;

    public List<string> CompletedEventsList;

    public List<string> TriggeredList;

    public int ChapterNumber;
}

