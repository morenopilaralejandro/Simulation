using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;

public static class QuestEvents
{
    public static event Action<string, QuestStatus> OnQuestStatusChanged;
    public static void RaiseQuestStatusChanged(string questId, QuestStatus questStatus)
    {
        OnQuestStatusChanged?.Invoke(questId, questStatus);
    }
}
