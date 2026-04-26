using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.World;

public class WorldManagerPersistance
{
    #region Fields

    private WorldManager worldManager;
    private PlayerWorldEntity playerWorldEntity;

    #endregion

    #region Constructor

    public WorldManagerPersistance()
    {
        worldManager = WorldManager.Instance;
        playerWorldEntity = worldManager.PlayerWorldEntity;
    }

    #endregion

    #region Logic

    public SaveDataWorldSystem Export()
    {
        return new SaveDataWorldSystem
        {
            Realm = worldManager.CurrentRealm,
            PlayerPosition = playerWorldEntity.CurrentTilePosition3d(),
            FacingDirection = playerWorldEntity.FacingToVector(playerWorldEntity.FacingDirection),
            ZoneId = worldManager.CurrentZone.zoneId
        };
    }

    public void Import(SaveDataWorldSystem saveData)
    {
        //itemManager.ImportStorageSystem(saveData.SaveDataItemStorage);
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
