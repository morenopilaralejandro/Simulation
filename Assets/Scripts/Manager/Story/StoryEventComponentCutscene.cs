using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryEventComponentCutscene
{
    public string CutsceneId { get; private set; }
    public bool HasCutscene { get; private set; }
    public string CutsceneBgmId { get; private set; }

    public StoryEventComponentCutscene(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        CutsceneId = storyEventData.CutsceneId;
        HasCutscene = !string.IsNullOrEmpty(CutsceneId);
        CutsceneBgmId = storyEventData.BgmId;
    }
}
