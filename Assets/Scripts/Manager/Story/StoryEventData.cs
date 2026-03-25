using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryEventData", menuName = "ScriptableObject/Story/StoryEventData")]
public class StoryEventData : ScriptableObject
{
    public string StoryEventId; //location name description

    [Header("Prerequisites")]
    public List<StoryPrerequisite> Prerequisites = new List<StoryPrerequisite>();

    [Header("Effects")]
    public List<StoryEffect> Effects = new List<StoryEffect>();

    [Header("Cutscene")]
    public string CutsceneId;

    [Header("ScriptedEvent")]
    public string ScriptedEventId;

    public AudioClip BgmClip;
}
