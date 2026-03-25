using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemPersistance
{
    #region Fields

    private StorySystemManager storySystemManager;

    #endregion

    #region Constructor

    public StorySystemPersistance() 
    {
        storySystemManager = StorySystemManager.Instance;
    }

    #endregion

    #region Import
    
    public void Import(StorySystemSaveData data)
    {
        /*
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
        */
    }

    #endregion

    #region Export

    public StorySystemSaveData Export()
    {
        return new StorySystemSaveData
        {
            FlagsDict = storySystemManager.FlagsDict,
            VariablesDict = storySystemManager.VariablesDict,
            CompletedEventsList = new List<string>(storySystemManager.CompletedEventsCollection),
            TriggeredList = new List<string>(storySystemManager.TriggeredCollection),
            ChapterNumber = storySystemManager.CurrentChapter.StoryChapterNumber
        };
    }

    #endregion

    #region Helpers

    #endregion


