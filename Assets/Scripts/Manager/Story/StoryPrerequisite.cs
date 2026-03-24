using Simulation.Enums.Quest;
using Simulation.Enums.Story;
using Simulation.Enums.Item;

[System.Serializable]
public class StoryPrerequisite
{
    public PrerequisiteType type;
    public string targetName;
    public int intValue;
}
