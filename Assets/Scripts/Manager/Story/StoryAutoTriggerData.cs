using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

[CreateAssetMenu(fileName = "StoryAutoTriggerData", menuName = "ScriptableObject/Story/StoryAutoTriggerData")]
public class StoryAutoTriggerData : ScriptableObject
{
    public string StoryAutoTriggerId;
    public string StoryEventId;
    public List<StoryPrerequisite> StoryPrerequisites = new List<StoryPrerequisite>();
}
