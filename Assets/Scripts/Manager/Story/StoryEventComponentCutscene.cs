using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryEventComponentCutscene
{
    public string CutsceneId { get; private set; }
    public bool HasCutscene { get; private set; }
    public AudioClip CutsceneBgmClip { get; private set; }

    public StoryEventComponentAttributes(StoryEventData storyEventData, StoryEvent storyEvent)
    {
        CutsceneId = storyEventData.CutsceneId;
        HasCutscene = !string.IsNullOrEmpty(CutsceneId);
        CutsceneBgmClip = storyEventData.BgmClip;
    }
}
