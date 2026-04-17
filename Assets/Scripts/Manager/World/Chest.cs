using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Aremoreno.Enums.World;

public class Chest
{
    #region Components

    private ChestComponentAttributes attributesComponent;
    private ChestComponentStateMachine stateMachineComponent;
    private ChestComponentContent contentComponent;
    private ChestComponentPersistence persistenceComponent;

    #endregion

    #region Constructor

    public Chest(
        string chestId, 
        ItemData itemData,
        bool isPersistent)
    {
        Initialize(chestId, itemData, isPersistent);
    }

    public void Initialize(
        string chestId, 
        ItemData itemData,
        bool isPersistent)
    {
        attributesComponent = new ChestComponentAttributes(chestId);
        stateMachineComponent = new ChestComponentStateMachine();
        contentComponent = new ChestComponentContent(itemData);
        persistenceComponent = new ChestComponentPersistence(this, isPersistent);
    }

    #endregion

    #region API

    // attributesComponent
    public string ChestId => attributesComponent.ChestId;

    // stateMachineComponent
    public ChestState State => stateMachineComponent.State;
    public bool IsOpened => stateMachineComponent.IsOpened;
    public bool IsLocked => stateMachineComponent.IsLocked;
    public void SetState(ChestState state) => stateMachineComponent.SetState(state);

    // contentComponent
    public ItemData ItemData => contentComponent.ItemData;
    public string ItemId => contentComponent.ItemId;

    //persistenceComponent
    public bool IsPersistent => persistenceComponent.IsPersistent;
    public bool IsOpenedPersistent => persistenceComponent.IsOpenedPersistent;
    public void OpenPersistent() => persistenceComponent.OpenPersistent();

    #endregion
}
