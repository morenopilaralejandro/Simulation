using Simulation.Enums.Quest;
using Simulation.Enums.Story;
using Simulation.Enums.Item;

[System.Serializable]
public class StoryPrerequisite
{
    public PrerequisiteType PrerequisiteType;
    public string TargetId;
    public int IntValue = 0;
}
