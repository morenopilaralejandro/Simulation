using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.World;

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
        ChestState state,
        bool isPersistent)
    {
        Initialize(chestId, itemData);
    }

    public void Initialize(
        string chestId, 
        ItemData itemData,
        ChestState state,
        bool isPersistent)
    {
        attributesComponent = new ChestComponentAttributes(chestId);
        stateMachineComponent = new ChestComponentStateMachine(state);
        contentComponent = new ChestComponentContent(itemData);
        persistenceComponent = ChestComponentPersistence(this, isPersistent);
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

    //persistenceComponent
    public bool IsPersistent => persistenceComponent.IsPersistent;
    public bool IsOpenedPersistent => persistenceComponent.IsOpenedPersistent;
    public void OpenPersistent() => persistenceComponent.OpenPersistent;

    #endregion
}
