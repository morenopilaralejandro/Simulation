using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryAutoTriggerComponentPersistence
{
    private StoryAutoTrigger storyAutoTrigger;
    private StorySystemManager storySystemManager;

    public StoryAutoTriggerComponentPersistence(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        this.storyAutoTrigger = storyAutoTrigger;
        storySystemManager = StorySystemManager.Instance;
    }

    public bool HasTriggered => storySystemManager.HasAutoTriggerTriggered(storyAutoTrigger.StoryAutoTriggerId);
}
