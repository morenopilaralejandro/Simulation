using UnityEngine;
using System;
using System.IO;

public class PersistenceManagerSave
{
    #region Fields

    private const string FLAG_NEW_GAME = "NEW_GAME";
    private long timestampCreation;
    private long timestampSessionStart;
    private long playTimeSeconds;

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

    public long PlayTimeSeconds => playTimeSeconds;
    public long TimestampSessionStart => timestampSessionStart;
    public long TimestampCreation => timestampCreation;
    public void SetTimestampCreation(long longValue) => timestampCreation = longValue;
    public void SetPlayTimeSeconds(long longValue) => playTimeSeconds = longValue;

    public void SaveGame()
    {
        if (IsNewGame()) SetNewGame(false);

        playTimeSeconds = GetCurrentPlayTimeSeconds();
        StartSession();

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
            PlayTimeSeconds = playTimeSeconds,
            CharacterSystemSaveData = CharacterManager.Instance.Export(),
            SaveDataItemSystem = ItemManager.Instance.Export(),
            QuestSystemSaveData = QuestSystemManager.Instance.Export(),
            StorySystemSaveData = StorySystemManager.Instance.Export(),
            ChestStateSaveData = ChestStateManager.Instance.Export(),
            SaveDataWorldSystem = WorldManager.Instance.Export(),
            SaveDataTeamSystem = TeamManager.Instance.Export(),
            WingSystemSaveData = WingManager.Instance.Export()
        };
    }

    public void StartNewGame() 
    {
        StartSession();
        SetTimestampCreation(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        CharacterManager.Instance.FirstTimeInitialize();
        WingManager.Instance.FirstTimeInitialize();
        ItemManager.Instance.FirstTimeInitialize();
        ItemManager.Instance.InitializeCurrencySystem();
        StorySystemManager.Instance.FirstTimeInitialize();
        WorldArgs.Hour = 12;

        SetNewGame(true);
    }

    public void StartSession()
    {
        timestampSessionStart = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    #endregion

    #region Helpers

    public long GetCurrentPlayTimeSeconds()
    {
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        long sessionSeconds = now - timestampSessionStart;

        if (sessionSeconds < 0)
            sessionSeconds = 0;

        return playTimeSeconds + sessionSeconds;
    }

    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
