using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestComponentStateMachine
{
    private Quest quest;

    public QuestState State { get; set; }
    public long TimestampStart { get; set; }
    public long TimestampEnd { get; set; }

    public QuestComponentStateMachine(QuestData questData, Quest quest, QuestSaveData questSaveData = null)
    {
        this.quest = quest;

        if (questSaveData != null)
        {
            State = questSaveData.State;
            TimestampStart = questSaveData.TimestampStart;
            TimestampEnd = questSaveData.TimestampEnd;
        }
    }

    public void SetState(QuestState newState) 
    {
        State = newState;
        QuestEvents.RaiseQuestStateChanged(quest);

        switch(newState) 
        {
            case QuestState.NotStarted:
                break;
            case QuestState.Started:
                TimestampStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                break;
            case QuestState.Active:
                break;
            case QuestState.Completed:
                TimestampEnd = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                break;
            case QuestState.Failed:
                break;
            case QuestState.Locked:
                break;
            default:
                LogManager.Error("[QuestComponentStateMachine] default");
                break;
        }
    }
}
