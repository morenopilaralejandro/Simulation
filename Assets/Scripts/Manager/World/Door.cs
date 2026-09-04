using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Aremoreno.Enums.World;

public class Door
{
    #region Components

    private DoorComponentAttributes attributesComponent;
    private DoorComponentStateMachine stateMachineComponent;
    private DoorComponentKeys keysComponent;
    private DoorComponentPersistence persistenceComponent;

    #endregion

    #region Constructor

    public Door(
        string doorId, 
        List<ItemData> requiredKeyDataList,
        bool isPersistent)
    {
        Initialize(doorId, requiredKeyDataList, isPersistent);
    }

    public void Initialize(
        string doorId, 
        List<ItemData> requiredKeyDataList,
        bool isPersistent)
    {
        attributesComponent = new DoorComponentAttributes(doorId);
        stateMachineComponent = new DoorComponentStateMachine();
        keysComponent = new DoorComponentKeys(requiredKeyDataList);
        persistenceComponent = new DoorComponentPersistence(this, isPersistent);
    }

    #endregion

    #region API

    // attributesComponent
    public string DoorId => attributesComponent.DoorId;

    // stateMachineComponent
    public DoorState State => stateMachineComponent.State;
    public bool IsOpened => stateMachineComponent.IsOpened;
    public bool IsLocked => stateMachineComponent.IsLocked;
    public void SetState(DoorState state) => stateMachineComponent.SetState(state);

    // keysComponent
    public IReadOnlyList<ItemData> RequiredKeyDataList => keysComponent.RequiredKeyDataList;
    public IReadOnlyList<Item> RequiredKeyList => keysComponent.RequiredKeyList;
    public bool HasRequiredKeys() => keysComponent.HasRequiredKeys();

    //persistenceComponent
    public bool IsPersistent => persistenceComponent.IsPersistent;
    public bool IsOpenedPersistent => persistenceComponent.IsOpenedPersistent;
    public void OpenPersistent() => persistenceComponent.OpenPersistent();

    #endregion
}
