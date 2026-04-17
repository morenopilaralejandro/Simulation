using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;
using Aremoreno.Enums.Item;

[System.Serializable]
public class QuestRewards
{
    public int Exp;
    public int Gold;
    public List<ItemReward> ItemRewards = new List<ItemReward>();
    public List<StoryEffect> StoryEffects = new List<StoryEffect>();
}
