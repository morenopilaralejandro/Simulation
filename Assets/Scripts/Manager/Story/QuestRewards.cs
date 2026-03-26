using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;
using Simulation.Enums.Item;

[System.Serializable]
public class QuestRewards
{
    public int Exp;
    public int Gold;
    public List<ItemReward> ItemRewards = new List<ItemReward>();
    public List<StoryEffect> StoryEffects = new List<StoryEffect>();
}
