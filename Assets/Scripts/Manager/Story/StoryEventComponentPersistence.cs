using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

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
