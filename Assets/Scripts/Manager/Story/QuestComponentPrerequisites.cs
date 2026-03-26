using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestComponentPrerequisites
{
    public List<StoryPrerequisite> storyPrerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> StoryPrerequisites => storyPrerequisites;

    public QuestComponentPrerequisites(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        storyPrerequisites = questData.StoryPrerequisites;
    }

}
