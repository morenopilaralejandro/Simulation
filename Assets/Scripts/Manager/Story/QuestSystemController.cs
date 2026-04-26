using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class QuestSystemController
{
    private QuestSystemManager questSystemManager;
    private QuestDatabase questDatabase;
    private QuestObjectiveDatabase questObjectiveDatabase;
    private Quest auxQuest;

    private Dictionary<string, Quest> questDict = new Dictionary<string, Quest>();
    private string currentMainQuestId = "";
    private string currentActiveQuestId = "";

    public IReadOnlyDictionary<string, Quest> QuestDict => questDict;
    public string CurrentMainQuestId => currentMainQuestId;
    public string CurrentActiveQuestId => currentActiveQuestId;

    public QuestSystemController()
    {
        questSystemManager = QuestSystemManager.Instance;
        questDatabase = QuestDatabase.Instance;
        questObjectiveDatabase = QuestObjectiveDatabase.Instance;
        InitializeFromDatabase();
    }

    private void InitializeFromDatabase()
    {
        foreach (QuestData questData in questDatabase.QuestDataDict.Values)
        {
            questDict[questData.QuestId] = new Quest(questData);
        }
    }

    public void StartQuest(string questId)
    {
        if (!questDict.TryGetValue(questId, out auxQuest)) return;
        if (auxQuest.State != QuestState.NotStarted) return;

        auxQuest.SetState(QuestState.Started);
        LogManager.Trace($"[QuestSystemController] Started: {questId}");
    }

    public void CompleteQuestObjective(string questId, string objectiveId)
    {
        if (!questDict.TryGetValue(questId, out auxQuest)) return;
        if (auxQuest.State != QuestState.Started) return;

        auxQuest.MarkObjectiveAsCompleted(objectiveId);
        LogManager.Trace($"[QuestSystemController] Objective '{objectiveId}' completed in quest '{questId}'");

        if (auxQuest.AreAllObjectivesComplete())
            CompleteQuest(questId);
    }

    public void UpdateQuestObjectiveProgress(string questId, string objectiveId, int amount = 1)
    {
        if (!questDict.TryGetValue(questId, out auxQuest)) return;
        if (auxQuest.State != QuestState.Started) return;

        auxQuest.UpdateObjectiveProgress(objectiveId, amount);
    }

    public void CompleteQuest(string questId)
    {
        if (!questDict.TryGetValue(questId, out auxQuest)) return;

        auxQuest.SetState(QuestState.Completed);
        LogManager.Trace($"[QuestSystemController] Completed: {questId}");

        // Auto-start follow-up quests
        foreach (var followUpId in auxQuest.FollowUpQuestIds)
        {
            if (questSystemManager.CheckQuestPrerequisites(followUpId))
                StartQuest(followUpId);
        }
    }

    public void FailQuest(string questId)
    {
        if (!questDict.TryGetValue(questId, out auxQuest)) return;
        auxQuest.SetState(QuestState.Failed);
        LogManager.Trace($"[QuestSystemController] Failed: {questId}");
    }

    public Quest GetQuest(string questId)
    {
        return questDict.TryGetValue(questId, out auxQuest) ? auxQuest : null;
    }

    public QuestState GetQuestState(string questId)
    {
        return questDict.TryGetValue(questId, out auxQuest) ? auxQuest.State : QuestState.NotStarted;
    }

    public List<Quest> GetQuestsByState(QuestState state)
    {
        List<Quest> result = new List<Quest>();

        foreach (Quest quest in questDict.Values)
        {
            if (quest.State == state)
                result.Add(quest);
        }

        return result;
    }

    public List<Quest> GetQuestsByType(QuestType questType)
    {
        List<Quest> result = new List<Quest>();

        foreach (Quest quest in questDict.Values)
        {
            if (quest.QuestType == questType)
                result.Add(quest);
        }

        return result;
    }

}
