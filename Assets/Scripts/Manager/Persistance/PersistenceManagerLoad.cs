using UnityEngine;
using System;
using System.IO;
using Simulation.Enums.World;

public class PersistenceManagerLoad
{
    #region Fields

    private PersistenceManager persistenceManager;

    #endregion

    #region Constructor

    public PersistenceManagerLoad() 
    {
        persistenceManager = PersistenceManager.Instance;
    }

    #endregion

    #region Logic

    public void LoadGame()
    {
        SaveData saveData = persistenceManager.GetLastSaveData();

        persistenceManager.SetTimestampCreation(saveData.TimestampCreation);
        CharacterManager.Instance.Import(saveData.CharacterSystemSaveData);
        ItemManager.Instance.Import(saveData.SaveDataItemSystem);
        QuestSystemManager.Instance.Import(saveData.QuestSystemSaveData);
        StorySystemManager.Instance.Import(saveData.StorySystemSaveData);
        ChestStateManager.Instance.Import(saveData.ChestStateSaveData);
        //WorldManager.Instance.Import(saveData.SaveDataWorldSystem);
        TeamManager.Instance.Import(saveData.SaveDataTeamSystem);

        WorldArgs.Set(
            zoneId : saveData.SaveDataWorldSystem.ZoneId,
            realm : saveData.SaveDataWorldSystem.Realm,
            playerPosition : saveData.SaveDataWorldSystem.PlayerPosition,
            facingDirection : saveData.SaveDataWorldSystem.FacingDirection,
            worldState : WorldState.Loading
        );

        PersistenceEvents.RaiseGameLoaded(saveData);
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
