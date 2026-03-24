using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[System.Serializable]
public class StoryAutoTrigger
{
    public string triggerId;
    public string eventId;
    public List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();
    [System.NonSerialized] public bool hasTriggered;
}
