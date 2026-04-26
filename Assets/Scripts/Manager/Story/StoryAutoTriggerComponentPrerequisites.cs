using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryAutoTriggerComponentPrerequisites
{
    private List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisites;

    public StoryAutoTriggerComponentPrerequisites(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        prerequisites = storyAutoTriggerData.StoryPrerequisites;
    }
}
