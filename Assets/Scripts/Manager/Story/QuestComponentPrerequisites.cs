using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestComponentPrerequisites
{
    public List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisites;

    public QuestComponentPrerequisites(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        prerequisites = questData.Prerequisites;
    }

}
