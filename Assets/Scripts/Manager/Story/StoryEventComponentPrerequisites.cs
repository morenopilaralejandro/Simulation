using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentPrerequisites
{
    private List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisites;

    public StoryEventComponentPrerequisites(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        prerequisites = storyEventData.Prerequisites;
    }
}
