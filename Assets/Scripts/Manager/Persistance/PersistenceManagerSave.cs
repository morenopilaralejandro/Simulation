using UnityEngine;
using System;
using System.IO;

public class PersistenceManagerSave
{
    #region Fields

    private const string FLAG_NEW_GAME = "NEW_GAME";
    private long timestampCreation;

    private PersistenceManager persistenceManager;
    private StorySystemManager storyManager;

    #endregion

    #region Constructor

    public PersistenceManagerSave() 
    {
        persistenceManager = PersistenceManager.Instance;
        storyManager = StorySystemManager.Instance;
    }

    #endregion

    #region Logic

    public bool IsNewGame() => storyManager.GetFlag(FLAG_NEW_GAME);
    public void SetNewGame(bool boolValue) => storyManager.SetFlag(FLAG_NEW_GAME, boolValue);

    public long TimestampCreation => timestampCreation;
    public void SetTimestampCreation(long longValue) => timestampCreation = longValue;

    public void SaveGame()
    {
        if (IsNewGame())
            SetNewGame(false);
        SaveData data = CreateSaveData();
        persistenceManager.Save(data);
    }

    private SaveData CreateSaveData()
    {
        return new SaveData
        {
            Header = new SaveDataHeader 
            {
                FileSignature = persistenceManager.FileSignature,
                GameIdentifier = persistenceManager.GameIdentifier,
                SaveFormatVersion = persistenceManager.SaveFormatVersion,
                GameVersion = persistenceManager.GameVersion
            },

            TimestampSave = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TimestampCreation = timestampCreation,
            CharacterSystemSaveData = CharacterManager.Instance.Export(),
            SaveDataItemSystem = ItemManager.Instance.Export(),
            QuestSystemSaveData = QuestSystemManager.Instance.Export(),
            StorySystemSaveData = StorySystemManager.Instance.Export(),
            ChestStateSaveData = ChestStateManager.Instance.Export(),
            SaveDataWorldSystem = WorldManager.Instance.Export(),
            SaveDataTeamSystem = TeamManager.Instance.Export()
        };
    }

    public void StartNewGame() 
    {
        SetNewGame(true);
        SetTimestampCreation(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        CharacterManager.Instance.FirstTimeInitialize();
        ItemManager.Instance.FirstTimeInitialize();
        StorySystemManager.Instance.SetChapter(0);
    }

    #endregion

    #region Helpers

    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
