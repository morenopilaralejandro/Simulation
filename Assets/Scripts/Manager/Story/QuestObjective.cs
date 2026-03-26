using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Localization;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestObjective
{
    #region Components

    private QuestObjectiveComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private QuestObjectiveComponentProgress progressComponent;
    private QuestObjectiveComponentPersistence persistenceComponent;

    #endregion

    #region Initialize

    public QuestObjective(QuestObjectiveData questObjectiveData, QuestObjectiveSaveData questObjectiveSaveData = null) 
    {
        Initialize(questObjectiveData);
    }

    public void Initialize(QuestObjectiveData questObjectiveData, QuestObjectiveSaveData questObjectiveSaveData = null)
    {
        attributesComponent = new QuestObjectiveComponentAttributes(questObjectiveData, this, questObjectiveSaveData);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Quest_Objective,
            questObjectiveData.QuestObjectiveId,
            new[] { LocalizationField.Description }
        );
        progressComponent = new QuestObjectiveComponentProgress(questObjectiveData, this, questObjectiveSaveData);
        persistenceComponent = new QuestObjectiveComponentPersistence(questObjectiveData, this, questObjectiveSaveData);
    }

    #endregion

    #region API
    // attributesComponent
    public string QuestObjectiveId => attributesComponent.QuestObjectiveId;
    public ObjectiveType ObjectiveType => attributesComponent.ObjectiveType;
    public string TargetId => attributesComponent.TargetId;
    public bool IsOptional => attributesComponent.IsOptional;
    public bool IsHidden => attributesComponent.IsHidden;

    // localizationComponent
    public LocalizationComponentString LocalizationComponent => localizationStringComponent;
    public string ObjectiveDescription => localizationStringComponent.GetString(LocalizationField.Description);

    // progressComponent
    public int RequiredAmount => progressComponent.RequiredAmount;
    public int CurrentAmount => progressComponent.RequiredAmount;
    public bool IsCompleted => progressComponent.IsCompleted;
    public void MarkAsCompleted() => progressComponent.MarkAsCompleted();
    public void UpdateProgress(int amount) => progressComponent.UpdateProgress(amount);

    // persistenceComponent
    public void Import(QuestObjectiveSaveData questObjectiveSaveData) => persistenceComponent.Import(questObjectiveSaveData);
    public QuestObjectiveSaveData Export() => persistenceComponent.Export();

    #endregion
}
