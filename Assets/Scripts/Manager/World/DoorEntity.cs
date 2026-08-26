using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.World;

public class DoorEntity : MonoBehaviour
{
    #region Fields
    private Door door;
    [SerializeField] private string doorId;
    [SerializeField] private List<ItemData> requiredKeyDataList;
    [SerializeField] private DoorState state;
    [SerializeField] private bool isPersistent = true;

    #endregion

    #region Components
    
    [SerializeField] private DoorComponentAppearance appearanceComponent;
    [SerializeField] private DoorComponentDialog dialogComponent;

    #endregion

    #region Initialize

    void Start()
    {
        Initialize(doorId, requiredKeyDataList);
        // Check if already opened
        if (IsOpenedPersistent) 
            Open();
        else 
            SetState(state);
    }

    public void Initialize(string doorId, List<ItemData> requiredKeyDataList)
    {
        door = new Door(
            doorId, 
            requiredKeyDataList,
            isPersistent);

        appearanceComponent.Initialize(this);
        dialogComponent.Initialize(this);
    }

    #endregion

    #region API Door

    public Door Door => door;

    // attributesComponent
    public string DoorId => door.DoorId;

    // stateMachineComponent
    public DoorState State => door.State;
    public bool IsOpened => door.IsOpened;
    public bool IsLocked => door.IsLocked;
    public void SetState(DoorState newState) 
    { 
        door.SetState(newState);
        switch (newState) 
        {
            case DoorState.Opened:
                SetSpriteOpened();
                break;
            case DoorState.Locked:
                SetSpriteClosed();
                break;
            /*
            default: //Opened
                StartDialogEmpty();
                break;
            */
        }
    }

    //keysComponent
    public IReadOnlyList<ItemData> RequiredKeyDataList => door.RequiredKeyDataList;
    public IReadOnlyList<Item> RequiredKeyList => door.RequiredKeyList;
    public bool HasRequiredKeys() => door.HasRequiredKeys();

    //persistenceComponent
    public bool IsPersistent => door.IsPersistent;
    public bool IsOpenedPersistent => door.IsOpenedPersistent;
    public void OpenPersistent() => door.OpenPersistent();

    #endregion

    #region API Entity

    //appearanceComponent
    public void SetSpriteOpened() => appearanceComponent.SetSpriteOpened();
    public void SetSpriteClosed() => appearanceComponent.SetSpriteClosed();

    //dialogComponent
    public void StartDialog() => dialogComponent.StartDialog();

    #endregion

    #region Misc

    public bool TryOpen() 
    {
        if (HasRequiredKeys())
        {
            Open();
            return true;
        }

        return false;
    }

    public void Open() 
    {
        if (IsPersistent) OpenPersistent();
        SetState(DoorState.Opened);
    }

    #endregion
}
