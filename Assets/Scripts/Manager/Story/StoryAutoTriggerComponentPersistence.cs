using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryAutoTriggerComponentPersistence
{
    private StoryAutoTrigger storyAutoTrigger;
    private StorySystemManager storySystemManager;

    public StoryAutoTriggerComponentPersistence(StoryAutoTriggerData storyAutoTriggerData, StoryAutoTrigger storyAutoTrigger)
    {
        this.storyAutoTrigger = storyAutoTrigger;
        storySystemManager = StorySystemManager.Instance;
    }

    public bool HasTriggered => storySystemManager.HasAutoTriggerTriggered(storyEvent.StoryEventId);
}
