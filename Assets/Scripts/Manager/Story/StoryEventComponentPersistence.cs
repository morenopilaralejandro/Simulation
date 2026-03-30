using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentPersistence
{
    private StoryEvent storyEvent;
    private StorySystemManager storySystemManager;

    public StoryEventComponentPersistence(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        this.storyEvent = storyEvent;
        storySystemManager = StorySystemManager.Instance;
    }

    public bool IsCompleted => storySystemManager.IsEventCompleted(storyEvent.StoryEventId);
}
