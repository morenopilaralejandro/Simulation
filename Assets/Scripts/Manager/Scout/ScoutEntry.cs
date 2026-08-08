using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Scout;

public class ScoutEntry
{
    #region Components

    private ScoutEntryComponentCharacter characterComponent;

    #endregion

    #region Initialize

    public ScoutEntry(ScoutEntryData data) 
    {
        characterComponent = new ScoutEntryComponentCharacter(data);
    }

    #endregion

    #region API

    // characterComponent
    public string CharacterId => characterComponent.CharacterId;
    public int Cost => characterComponent.Cost;
    public int Level => characterComponent.Level;
    public Character Character => characterComponent.Character;
    public bool IsOwned => characterComponent.IsOwned;
    public bool IsAffordable => characterComponent.IsAffordable;

    #endregion
}
