using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.World;

public class TeamManagerPersistance
{
    #region Fields

    private TeamManager teamManager;

    #endregion

    #region Constructor

    public TeamManagerPersistance()
    {
        teamManager = TeamManager.Instance;
    }

    #endregion

    #region Logic

    public SaveDataTeamSystem Export()
    {
        return new SaveDataTeamSystem
        {
            SaveDataLoadoutSystem = teamManager.ExportLoadoutSystem()
        };
    }

    public void Import(SaveDataTeamSystem saveData)
    {
        teamManager.ImportLoadoutSystem(saveData.SaveDataLoadoutSystem);
    }

    #endregion

    #region Helpers



    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
