using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryAutoTriggerComponentPrerequisites
{
    private List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisites;

    public StoryAutoTriggerComponentAttributes(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        prerequisites = storyAutoTriggerData.Prerequisites;
    }
}
