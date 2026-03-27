using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestSystemManager : MonoBehaviour
{
    #region Fields

    public static QuestSystemManager Instance { get; private set; }

    private QuestSystemController questSystemController;
    private QuestSystemPrerequisites questSystemPrerequisites;
    private QuestSystemPersistence questSystemPersistence;

    #endregion

    #region Lifecycle

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
        questSystemController = new QuestSystemController();
        questSystemPrerequisites = new QuestSystemPrerequisites();
        questSystemPersistence = new QuestSystemPersistence();
    }

    #endregion

    #region API

    // questSystemController
    public IReadOnlyDictionary<string, Quest> QuestDict => questSystemController.QuestDict;
    public string CurrentMainQuestId => questSystemController.CurrentMainQuestId;
    public string CurrentActiveQuestId => questSystemController.CurrentActiveQuestId;
    public void StartQuest(string questId) => questSystemController.StartQuest(questId);
    public void CompleteQuestObjective(string questId, string objectiveId)
        => questSystemController.CompleteQuestObjective(questId, objectiveId);
    public void UpdateQuestObjectiveProgress(string questId, string objectiveId, int amount = 1) 
        => questSystemController.UpdateQuestObjectiveProgress(questId, objectiveId, amount);
    public void CompleteQuest(string questId) => questSystemController.CompleteQuest(questId);
    public void FailQuest(string questId) => questSystemController.FailQuest(questId);
    public Quest GetQuest(string questId) => questSystemController.GetQuest(questId);
    public QuestState GetQuestState(string questId) => questSystemController.GetQuestState(questId);
    public List<Quest> GetQuestsByState(QuestState state) => questSystemController.GetQuestsByState(state);
    public List<Quest> GetQuestsByType(QuestType questType) => questSystemController.GetQuestsByType(questType);

    // questSystemPrerequisites
    public bool CheckQuestPrerequisites(string questId) => questSystemPrerequisites.CheckQuestPrerequisites(questId);
    public bool CheckPrerequisites(IReadOnlyList<StoryPrerequisite> prerequisites)
            => questSystemPrerequisites.CheckPrerequisites(prerequisites);
    public bool EvaluatePrerequisite(StoryPrerequisite prereq) => questSystemPrerequisites.EvaluatePrerequisite(prereq);

    // questSystemPersistence
    public QuestSystemSaveData Export() => questSystemPersistence.Export();
    public void Import(QuestSystemSaveData data) => questSystemPersistence.Import(data);

    #endregion
}
