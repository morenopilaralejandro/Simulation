using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

/// <summary>
/// Central manager for all story progress tracking.
/// Singleton pattern for global access.
/// </summary>
public class StoryProgressManager : MonoBehaviour
{
    public static StoryProgressManager Instance { get; private set; }

    [Header("Story Configuration")]
    [SerializeField] private StoryDatabase storyDatabase;

    // Story state


    private Dictionary<string, QuestState> questStates = new Dictionary<string, QuestState>();
    private string currentMainQuestId = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (storyDatabase != null)
            InitializeFromDatabase();
    }

    private void InitializeFromDatabase()
    {
        foreach (var quest in storyDatabase.allQuests)
        {
            if (!questStates.ContainsKey(quest.questId))
            {
                questStates[quest.questId] = new QuestState(quest.questId);
            }
        }
    }

    // ==================== STORY FLAGS ====================

    public void SetFlag(string flagName, bool value)
    {
        storyFlags[flagName] = value;
        OnStoryFlagChanged?.Invoke(flagName);
        Debug.Log($"[Story] Flag '{flagName}' set to {value}");
        EvaluateTriggers();
    }

    public bool GetFlag(string flagName)
    {
        return storyFlags.TryGetValue(flagName, out bool value) && value;
    }

    public bool HasFlag(string flagName)
    {
        return storyFlags.ContainsKey(flagName);
    }

    // ==================== STORY VARIABLES ====================

    public void SetVariable(string varName, int value)
    {
        storyVariables[varName] = value;
        OnStoryVariableChanged?.Invoke(varName, value);
        Debug.Log($"[Story] Variable '{varName}' set to {value}");
        EvaluateTriggers();
    }

    public void IncrementVariable(string varName, int amount = 1)
    {
        int current = GetVariable(varName);
        SetVariable(varName, current + amount);
    }

    public int GetVariable(string varName)
    {
        return storyVariables.TryGetValue(varName, out int value) ? value : 0;
    }

    // ==================== CHAPTERS ====================

    public int CurrentChapter => currentChapter;

    public void AdvanceChapter()
    {
        currentChapter++;
        OnChapterChanged?.Invoke(currentChapter);
        Debug.Log($"[Story] Advanced to Chapter {currentChapter}");
    }

    public void SetChapter(int chapter)
    {
        currentChapter = chapter;
        OnChapterChanged?.Invoke(currentChapter);
    }

    // ==================== STORY EVENTS ====================

    public void TriggerStoryEvent(string eventId)
    {
        if (completedEvents.Contains(eventId))
        {
            Debug.LogWarning($"[Story] Event '{eventId}' already triggered.");
            return;
        }

        completedEvents.Add(eventId);
        OnStoryEventTriggered?.Invoke(eventId);
        Debug.Log($"[Story] Event '{eventId}' triggered.");

        // Process event effects from database
        if (storyDatabase != null)
        {
            var storyEvent = storyDatabase.GetStoryEvent(eventId);
            if (storyEvent != null)
                ProcessStoryEvent(storyEvent);
        }
    }

    public bool IsEventCompleted(string eventId)
    {
        return completedEvents.Contains(eventId);
    }

    private void ProcessStoryEvent(StoryEvent storyEvent)
    {
        foreach (var effect in storyEvent.effects)
        {
            switch (effect.effectType)
            {
                case StoryEffectType.SetFlag:
                    SetFlag(effect.targetName, effect.boolValue);
                    break;
                case StoryEffectType.SetVariable:
                    SetVariable(effect.targetName, effect.intValue);
                    break;
                case StoryEffectType.IncrementVariable:
                    IncrementVariable(effect.targetName, effect.intValue);
                    break;
                case StoryEffectType.StartQuest:
                    StartQuest(effect.targetName);
                    break;
                case StoryEffectType.CompleteQuest:
                    CompleteQuest(effect.targetName);
                    break;
                case StoryEffectType.AdvanceChapter:
                    AdvanceChapter();
                    break;
            }
        }
    }

    // ==================== QUEST SYSTEM ====================

    public void StartQuest(string questId)
    {
        if (!questStates.ContainsKey(questId))
            questStates[questId] = new QuestState(questId);

        var quest = questStates[questId];
        if (quest.Status != QuestStatus.NotStarted) return;

        quest.Status = QuestStatus.Active;
        quest.StartTime = DateTime.Now;
        OnQuestStatusChanged?.Invoke(questId, QuestStatus.Active);
        Debug.Log($"[Quest] Started: {questId}");
    }

    public void CompleteQuestObjective(string questId, string objectiveId)
    {
        if (!questStates.ContainsKey(questId)) return;

        var quest = questStates[questId];
        if (quest.Status != QuestStatus.Active) return;

        quest.CompleteObjective(objectiveId);
        Debug.Log($"[Quest] Objective '{objectiveId}' completed in quest '{questId}'");

        // Check if all objectives are done
        var questData = storyDatabase?.GetQuest(questId);
        if (questData != null && quest.AreAllObjectivesComplete(questData.objectives))
        {
            CompleteQuest(questId);
        }
    }

    public void UpdateQuestObjectiveProgress(string questId, string objectiveId, int amount = 1)
    {
        if (!questStates.ContainsKey(questId)) return;

        var quest = questStates[questId];
        if (quest.Status != QuestStatus.Active) return;

        quest.UpdateObjectiveProgress(objectiveId, amount);

        var questData = storyDatabase?.GetQuest(questId);
        if (questData != null)
        {
            var objective = questData.objectives.Find(o => o.objectiveId == objectiveId);
            if (objective != null && quest.GetObjectiveProgress(objectiveId) >= objective.requiredAmount)
            {
                CompleteQuestObjective(questId, objectiveId);
            }
        }
    }

    public void CompleteQuest(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;

        var quest = questStates[questId];
        quest.Status = QuestStatus.Completed;
        quest.CompletionTime = DateTime.Now;
        OnQuestStatusChanged?.Invoke(questId, QuestStatus.Completed);
        Debug.Log($"[Quest] Completed: {questId}");

        // Auto-start follow-up quests
        var questData = storyDatabase?.GetQuest(questId);
        if (questData != null)
        {
            foreach (var followUpId in questData.followUpQuestIds)
            {
                if (CheckQuestPrerequisites(followUpId))
                    StartQuest(followUpId);
            }
        }
    }

    public void FailQuest(string questId)
    {
        if (!questStates.ContainsKey(questId)) return;
        questStates[questId].Status = QuestStatus.Failed;
        OnQuestStatusChanged?.Invoke(questId, QuestStatus.Failed);
        Debug.Log($"[Quest] Failed: {questId}");
    }

    public QuestStatus GetQuestStatus(string questId)
    {
        return questStates.TryGetValue(questId, out var state) ? state.Status : QuestStatus.NotStarted;
    }

    public QuestState GetQuestState(string questId)
    {
        return questStates.TryGetValue(questId, out var state) ? state : null;
    }

    public List<QuestState> GetActiveQuests()
    {
        return questStates.Values.Where(q => q.Status == QuestStatus.Active).ToList();
    }

    public List<QuestState> GetCompletedQuests()
    {
        return questStates.Values.Where(q => q.Status == QuestStatus.Completed).ToList();
    }

    // ==================== PREREQUISITES ====================

    public bool CheckQuestPrerequisites(string questId)
    {
        var questData = storyDatabase?.GetQuest(questId);
        if (questData == null) return true;

        return CheckPrerequisites(questData.prerequisites);
    }

    public bool CheckPrerequisites(List<StoryPrerequisite> prerequisites)
    {
        if (prerequisites == null || prerequisites.Count == 0) return true;

        foreach (var prereq in prerequisites)
        {
            if (!EvaluatePrerequisite(prereq))
                return false;
        }
        return true;
    }

    private bool EvaluatePrerequisite(StoryPrerequisite prereq)
    {
        switch (prereq.type)
        {
            case PrerequisiteType.FlagIsTrue:
                return GetFlag(prereq.targetName);

            case PrerequisiteType.FlagIsFalse:
                return !GetFlag(prereq.targetName);

            case PrerequisiteType.VariableEquals:
                return GetVariable(prereq.targetName) == prereq.intValue;

            case PrerequisiteType.VariableGreaterThan:
                return GetVariable(prereq.targetName) > prereq.intValue;

            case PrerequisiteType.VariableLessThan:
                return GetVariable(prereq.targetName) < prereq.intValue;

            case PrerequisiteType.QuestCompleted:
                return GetQuestStatus(prereq.targetName) == QuestStatus.Completed;

            case PrerequisiteType.QuestActive:
                return GetQuestStatus(prereq.targetName) == QuestStatus.Active;

            case PrerequisiteType.QuestNotStarted:
                return GetQuestStatus(prereq.targetName) == QuestStatus.NotStarted;

            case PrerequisiteType.ChapterReached:
                return currentChapter >= prereq.intValue;

            case PrerequisiteType.EventCompleted:
                return IsEventCompleted(prereq.targetName);

            default:
                return true;
        }
    }

    // ==================== TRIGGER EVALUATION ====================

    private void EvaluateTriggers()
    {
        if (storyDatabase == null) return;

        foreach (var trigger in storyDatabase.autoTriggers)
        {
            if (trigger.hasTriggered) continue;
            if (!CheckPrerequisites(trigger.prerequisites)) continue;

            trigger.hasTriggered = true;
            TriggerStoryEvent(trigger.eventId);
        }
    }

    // ==================== SAVE / LOAD ====================

    public StoryProgressSaveData GetSaveData()
    {
        return new StoryProgressSaveData
        {
            storyFlags = new Dictionary<string, bool>(storyFlags),
            storyVariables = new Dictionary<string, int>(storyVariables),
            questStates = questStates.Values.Select(q => q.ToSaveData()).ToList(),
            completedEvents = new List<string>(completedEvents),
            currentChapter = currentChapter,
            currentMainQuestId = currentMainQuestId
        };
    }

    public void LoadSaveData(StoryProgressSaveData data)
    {
        storyFlags = new Dictionary<string, bool>(data.storyFlags);
        storyVariables = new Dictionary<string, int>(data.storyVariables);
        completedEvents = new List<string>(data.completedEvents);
        currentChapter = data.currentChapter;
        currentMainQuestId = data.currentMainQuestId;

        questStates.Clear();
        foreach (var questSave in data.questStates)
        {
            var state = QuestState.FromSaveData(questSave);
            questStates[state.QuestId] = state;
        }

        Debug.Log("[Story] Save data loaded successfully.");
    }
}
