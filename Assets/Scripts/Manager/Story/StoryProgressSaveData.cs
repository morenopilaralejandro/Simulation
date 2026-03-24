using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[Serializable]
public class StoryProgressSaveData
{
    public Dictionary<string, bool> storyFlags;
    public Dictionary<string, int> storyVariables;
    public List<QuestStateSaveData> questStates;
    public List<string> completedEvents;
    public int currentChapter;
    public string currentMainQuestId;
}

