using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentPrerequisites
{
    private List<StoryPrerequisite> storyPrerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> StoryPrerequisites => storyPrerequisites;

    public StoryEventComponentPrerequisites(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        storyPrerequisites = storyEventData.StoryPrerequisites;
    }
}
