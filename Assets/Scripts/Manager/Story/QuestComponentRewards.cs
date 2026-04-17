using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestComponentRewards
{
    private Quest quest;
    public QuestRewards Rewards { get; set; }

    public QuestComponentRewards(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        this.quest = quest;
        Rewards = new QuestRewards();

        Rewards.Exp = questData.RewardExp;
        Rewards.Gold = questData.RewardGold;
        Rewards.ItemRewards.AddRange(questData.ItemRewards);
        Rewards.StoryEffects.AddRange(questData.StoryEffects);
    }

}
