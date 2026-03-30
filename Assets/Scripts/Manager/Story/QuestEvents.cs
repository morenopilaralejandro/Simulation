using System;
using System.Collections.Generic;
using Simulation.Enums.Quest;

public static class QuestEvents
{
    public static event Action<Quest> OnQuestStateChanged;
    public static void RaiseQuestStateChanged(Quest quest)
    {
        OnQuestStateChanged?.Invoke(quest);
    }
}
