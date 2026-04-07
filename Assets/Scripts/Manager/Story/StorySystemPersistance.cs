using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemPersistance
{
    #region Fields

    private StorySystemManager storySystemManager;
    private PersistenceManager persistenceManager;

    #endregion

    #region Constructor

    public StorySystemPersistance() 
    {
        storySystemManager = StorySystemManager.Instance;
        persistenceManager = PersistenceManager.Instance;
    }

    #endregion

    #region Import
    
    public void Import(StorySystemSaveData saveData)
    {
        storySystemManager.ImportFlagSystem(saveData);
        storySystemManager.ImportVariableSystem(saveData);
        storySystemManager.ImportEventSystem(saveData);
        storySystemManager.ImportTriggerSystem(saveData);
        storySystemManager.ImportChapterSystem(saveData);
        LogManager.Trace("[StorySystemPersistance] Save data loaded successfully.");
    }

    #endregion

    #region Export

    public StorySystemSaveData Export()
    {
        return new StorySystemSaveData
        {
            FlagsList = persistenceManager.ParseIReadOnlyDict<string, bool>(storySystemManager.FlagsDict),
            VariablesList = persistenceManager.ParseIReadOnlyDict<string, int>(storySystemManager.VariablesDict),
            CompletedEventsList = new List<string>(storySystemManager.CompletedEventsCollection),
            TriggeredList = new List<string>(storySystemManager.TriggeredCollection),
            ChapterNumber = storySystemManager.CurrentChapter.StoryChapterNumber
        };
    }

    #endregion

    #region Helpers

    #endregion

}
