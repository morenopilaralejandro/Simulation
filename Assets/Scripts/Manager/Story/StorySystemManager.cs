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
public class StorySystemManager : MonoBehaviour
{
    #region Fields

    public static StorySystemManager Instance { get; private set; }

    private StorySystemFlags flagSystem;
    private StorySystemVariables variableSystem;
    private StorySystemEvents eventSystem;
    private StorySystemTriggers triggerSystem;
    private StorySystemChapters chapterSystem;
    private StorySystemPersistance persistanceSystem;

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
        flagSystem = new StorySystemFlags();
        variableSystem = new StorySystemVariables();
        eventSystem = new StorySystemEvents();
        triggerSystem = new StorySystemTriggers();
        chapterSystem = new StorySystemChapters();
        persistanceSystem = new StorySystemPersistance();
    }

    #endregion

    #region API

    // flagSystem
    public IReadOnlyDictionary<string, bool> FlagsDict => flagSystem.FlagsDict;
    public void SetFlag(string flagId, bool boolValue) => flagSystem.SetFlag(flagId, boolValue);
    public bool GetFlag(string flagId) => flagSystem.GetFlag(flagId);
    public bool HasFlag(string flagId) => flagSystem.HasFlag(flagId);
    public void ImportFlagSystem(StorySystemSaveData saveData) => flagSystem.Import(saveData);


    // variableSystem
    public IReadOnlyDictionary<string, int> VariablesDict => variableSystem.VariablesDict;
    public void SetVariable(string variableId, int intValue) => variableSystem.SetVariable(variableId, intValue);
    public void IncrementVariable(string variableId, int amount = 1) => variableSystem.IncrementVariable(variableId, amount);
    public int GetVariable(string variableId) => variableSystem.GetVariable(variableId);
    public void ImportVariableSystem(StorySystemSaveData saveData) => variableSystem.Import(saveData);

    // eventSystem
    public IReadOnlyCollection<string> CompletedEventsCollection => eventSystem.CompletedEventsCollection;
    public void TriggerStoryEvent(string storyEventId) => eventSystem.TriggerStoryEvent(storyEventId);
    public bool IsEventCompleted(string storyEventId) => eventSystem.IsEventCompleted(storyEventId);
    public void ImportEventSystem(StorySystemSaveData saveData) => eventSystem.Import(saveData);

    // triggerSystem
    public IReadOnlyCollection<string> TriggeredCollection => triggerSystem.TriggeredCollection;
    public void TriggerAutoTrigger(string storyAutoTriggerId) => triggerSystem.TriggerAutoTrigger(storyAutoTriggerId);
    public bool HasAutoTriggerTriggered(string storyAutoTriggerId) => triggerSystem.HasAutoTriggerTriggered(storyAutoTriggerId);
    public void EvaluateTriggers() => triggerSystem.EvaluateTriggers();
    public void ImportTriggerSystem(StorySystemSaveData saveData) => triggerSystem.Import(saveData);

    // chapterSystem
    public StoryChapter CurrentChapter => chapterSystem.CurrentChapter;
    public void AdvanceChapter() => chapterSystem.AdvanceChapter();
    public void SetChapter(int intValue) => chapterSystem.SetChapter(intValue);
    public void ImportChapterSystem(StorySystemSaveData saveData) => chapterSystem.Import(saveData);

    // persistanceSystem
    public void Import(StorySystemSaveData data) => persistanceSystem.Import(data);
    public StorySystemSaveData Export() => persistanceSystem.Export();

    #endregion
   
}
