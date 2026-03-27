using System;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Simulation.Enums.World;

public class WorldManagerRealm
{
    #region Fields

    public Realm CurrentRealm { get; private set; }
    private OverworldDefinitionDatabase overworldDefinitionDatabase;

    #endregion

    #region Constructor

    public WorldManagerRealm() 
    {
        overworldDefinitionDatabase = OverworldDefinitionDatabase.Instance;
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
        return overworldDefinitionDatabase.GetOverworldDefinitionByRealm(CurrentRealm);
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
