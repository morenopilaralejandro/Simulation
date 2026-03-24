// StoryEvent.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryEventData", menuName = "ScriptableObject/Story/StoryEventData")]
public class StoryEventData : ScriptableObject
{
    public string eventId;
    //public string eventName;
    //public string description; location

    [Header("Prerequisites")]
    public List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();

    [Header("Effects")]
    public List<StoryEffect> effects = new List<StoryEffect>();

    [Header("Cutscene")]
    public bool hasCutscene;
    public string cutsceneId;
    public AudioClip eventMusic;
}
