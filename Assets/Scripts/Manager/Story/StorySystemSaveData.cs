using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class StorySystemSaveData
{
    public List<SerializableKeyValue<string, bool>> FlagsList;

    public List<SerializableKeyValue<string, int>> VariablesList;

    public List<string> CompletedEventsList;

    public List<string> TriggeredList;

    public int ChapterNumber;
}

