using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class Quest
{
    #region Components

    private QuestComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private QuestComponentStateMachine stateMachineComponent;
    private QuestComponentObjectives objectivesComponent;
    private QuestComponentPrerequisites prerequisitesComponent;
    private QuestComponentRewards rewardsComponent;
    private QuestComponentFlow flowComponent;
    private QuestComponentPersistence persistenceComponent;

    #endregion

    #region Initialize

    public Quest(QuestData questData, QuestSaveData questSaveData = null) 
    {
        Initialize(characterData, characterSaveData);
    }

    public void Initialize(QuestData questData, QuestSaveData questSaveData = null)
    {
        attributesComponent = new QuestComponentAttributes(questData, this, questSaveData);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Quest,
            questData.QuestId,
            new[] { LocalizationField.Name, LocalizationField.Description }
        );
        stateMachineComponent = new QuestComponentStateMachine(questData, this, questSaveData);
        objectivesComponent = new QuestComponentObjectives(questData, this, questSaveData);
        prerequisitesComponent = new QuestComponentPrerequisites(questData, this, questSaveData);
        rewardsComponent = new QuestComponentRewards(questData, this, questSaveData);
        flowComponent = new QuestComponentFlow(questData, this, questSaveData);
        persistenceComponent = new QuestComponentPersistence(questData, this, questSaveData);
    }

    #endregion

    #region API

    // attributesComponent
    public string QuestId => attributesComponent.QuestId;
    public QuestType QuestType => attributesComponent.QuestType;
    public int RecommendedLevel => attributesComponent.RecommendedLevel;

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string QuestName => localizationStringComponent.GetString(LocalizationField.Name);
    public string QuestDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // stateMachineComponent
    public QuestState State => stateMachineComponent.State;
    public long TimestampStart => stateMachineComponent.TimestampStart;
    public long TimestampEnd => stateMachineComponent.TimestampEnd;
    public void SetState(QuestState newState) => stateMachineComponent.SetState(newState);

    // objectivesComponent
    public IReadOnlyDictionary<string, QuestObjective> QuestObjectiveDict => objectivesComponent.QuestObjectiveDict;
    public void MarkObjectiveAsCompleted(string objectiveId) => objectivesComponent.MarkObjectiveAsCompleted(objectiveId);
    public bool IsObjectiveComplete(string objectiveId) => objectivesComponent.IsObjectiveComplete(objectiveId);
    public void UpdateObjectiveProgress(string objectiveId, int amount) => objectivesComponent.UpdateObjectiveProgress(objectiveId, amount);
    public int GetObjectiveProgress(string objectiveId) => objectivesComponent.GetObjectiveProgress(objectiveId);
    public bool AreAllObjectivesComplete() => objectivesComponent.AreAllObjectivesComplete();

    // prerequisitesComponent
    public IReadOnlyList<StoryPrerequisite> Prerequisites => prerequisitesComponent.Prerequisites;

    // rewardsComponent
    public QuestRewards Rewards => rewardsComponent.Rewards;

    // flowComponent
    public IReadOnlyList<string> FollowUpQuestIds => flowComponent.FollowUpQuestIds;

    // persistenceComponent
    public void Import(QuestSaveData questSaveData) => persistenceComponent.Import(questSaveData);
    public QuestSaveData Export() => persistenceComponent.Export();

    #endregion
}
