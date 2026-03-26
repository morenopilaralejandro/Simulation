using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public static class StoryEventFactory
{
    public static StoryEvent Create(StoryEventData data)
    {
        return new StoryEvent(data);
    }

    public static StoryEvent CreateById(string storyEventId) 
    {
        return Create(StoryEventDatabase.Instance.GetStoryEventData(storyEventId));
    }
}
