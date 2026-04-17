using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryEventComponentScriptedEvent
{
    public string ScriptedEventId { get; private set; }
    public bool HasScriptedEvent { get; private set; }
    public string ScriptedEventBgmId { get; private set; }

    public StoryEventComponentScriptedEvent(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        ScriptedEventId = storyEventData.ScriptedEventId;
        HasScriptedEvent = !string.IsNullOrEmpty(ScriptedEventId);
        ScriptedEventBgmId = storyEventData.BgmId;
    }
}
