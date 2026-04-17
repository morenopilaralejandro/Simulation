using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;
using Aremoreno.Enums.Item;

[System.Serializable]
public class StoryPrerequisite
{
    public PrerequisiteType PrerequisiteType;
    public string TargetId;
    public int IntValue = 0;
}
