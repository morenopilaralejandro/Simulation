using Simulation.Enums.Quest;
using Simulation.Enums.Story;
using Simulation.Enums.Item;

[System.Serializable]
public class QuestRewards
{
    public int experiencePoints;
    public int gold;
    public List<ItemReward> items = new List<ItemReward>();
    public List<StoryEffect> storyEffects = new List<StoryEffect>();
}
