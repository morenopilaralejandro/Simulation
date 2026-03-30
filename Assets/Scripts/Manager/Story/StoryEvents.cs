using System;
using System.Collections.Generic;
using Simulation.Enums.Story;

public static class StoryEvents
{
    public static event Action<string> OnStoryFlagChanged;
    public static void RaiseStoryFlagChanged(string flagId)
    {
        OnStoryFlagChanged?.Invoke(flagId);
    }

    public static event Action<string, int> OnStoryVariableChanged;
    public static void RaiseStoryVariableChanged(string varibaleId, int intValue)
    {
        OnStoryVariableChanged?.Invoke(varibaleId, intValue);
    }

    public static event Action<StoryChapter> OnChapterChanged;
    public static void RaiseChapterChanged(StoryChapter storyChapter)
    {
        OnChapterChanged?.Invoke(storyChapter);
    }

    public static event Action<string> OnStoryEventTriggered;
    public static void RaiseStoryEventTriggered(string storyEventId)
    {
        OnStoryEventTriggered?.Invoke(storyEventId);
    }

}
