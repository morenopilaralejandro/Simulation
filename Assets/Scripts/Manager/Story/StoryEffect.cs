using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[System.Serializable]
public class StoryEffect
{
    public StoryEffectType EffectType;
    public string TargetId;
    public bool BoolValue = false;
    public int IntValue = 0;
}
