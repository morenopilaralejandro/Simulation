using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestComponentAttributes
{
    public string QuestId { get; private set; }
    public QuestType QuestType { get; private set; }
    public int RecommendedLevel { get; private set; }

    public QuestComponentAttributes(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        QuestId = questData.QuestId;
        QuestType = questData.QuestType;
        RecommendedLevel = questData.RecommendedLevel;
    }
}
