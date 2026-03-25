using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[CreateAssetMenu(fileName = "StoryAutoTriggerData", menuName = "ScriptableObject/Story/StoryAutoTriggerData")]
public class StoryAutoTriggerData : ScriptableObject
{
    public string StoryAutoTriggerId;
    public string StoryEventId;
    public List<StoryPrerequisite> Prerequisites = new List<StoryPrerequisite>();
}
