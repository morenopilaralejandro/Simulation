using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentScriptedEvent
{
    public string ScriptedEventId { get; private set; }
    public bool HasScriptedEvent { get; private set; }
    public AudioClip ScriptedEventBgmClip { get; private set; }

    public StoryEventComponentScriptedEvent(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        ScriptedEventId = storyEventData.ScriptedEventId;
        HasScriptedEvent = !string.IsNullOrEmpty(ScriptedEventId);
        ScriptedEventBgmClip = storyEventData.BgmClip;
    }
}
