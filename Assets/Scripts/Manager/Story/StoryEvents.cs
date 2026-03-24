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

    public static event Action<int> OnChapterChanged;
    public static void RaiseChapterChanged(int intValue)
    {
        OnChapterChanged?.Invoke(intValue);
    }

    public static event Action<string> OnStoryEventTriggered;
    public static void RaiseStoryEventTriggered(string storyEventId)
    {
        OnStoryEventTriggered?.Invoke(intValue);
    }
}
