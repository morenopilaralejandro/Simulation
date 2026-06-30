using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.World;

public class WorldManagerRealm
{
    #region Fields

    public Realm CurrentRealm { get; private set; }

    #endregion

    #region Constructor

    public WorldManagerRealm() 
    {

    }

    #endregion

    #region Logic

    public void SetRealm(Realm realm) 
    {
        CurrentRealm = realm;
        WorldEvents.RaiseRealmChanged(realm);
    }

    public OverworldDefinition GetOverworldDefinition() 
    {
        return DatabaseManager.Instance.GetOverworldDefinitionByRealm(CurrentRealm);
    }

    #endregion

    #region Events

    public void Subscribe()
    {

    }

    public void Unsubscribe()
    {

    }

    #endregion

}
