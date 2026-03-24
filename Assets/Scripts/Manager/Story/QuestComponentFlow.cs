using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestComponentFlow
{
    private Quest quest;

    private List<string> followUpQuestIds = new List<string>();;
    public IReadOnlyList<string> FollowUpQuestIds => followUpQuestIds;

    public QuestComponentFlow(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        this.quest = quest;

        followUpQuestIds.AddRange(questData.FollowUpQuestIds);
    }

}
