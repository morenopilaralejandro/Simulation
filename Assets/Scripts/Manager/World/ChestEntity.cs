using UnityEngine;
using Simulation.Enums.World;

public class ChestEntity : MonoBehaviour
{
    #region Fields
    private Chest chest;
    [SerializeField] private string chestId;
    [SerializeField] private ItemData itemData;
    [SerializeField] private ChestState state;
    [SerializeField] private bool isPersistent = true;

    #endregion

    #region Components
    
    [SerializeField] private ChestComponentInteractable interactableComponent;
    [SerializeField] private ChestComponentAppearance appearanceComponent;
    [SerializeField] private ChestComponentDialog dialogComponent;

    #endregion

    #region Initialize

    void Start()
    {
        Initialize(chestId, itemData);
        // Check if already opened
        if (IsOpenedPersistent) 
            Open();
        else 
            SetState(state);
    }

    public void Initialize(string chestId, ItemData itemData)
    {
        chest = new Chest(
            chestId, 
            itemData,
            isPersistent);

        interactableComponent.Initialize(this);
        appearanceComponent.Initialize(this);
        dialogComponent.Initialize(this);
    }

    #endregion

    #region API Chest

    public Chest Chest => chest;

    // attributesComponent
    public string ChestId => chest.ChestId;

    // stateMachineComponent
    public ChestState State => chest.State;
    public bool IsOpened => chest.IsOpened;
    public bool IsLocked => chest.IsLocked;
    public void SetState(ChestState newState) 
    { 
        chest.SetState(newState);
        switch (newState) 
        {
            case ChestState.Opened:
                SetSpriteOpened();
                break;
            case ChestState.Closed:
                SetSpriteClosed();
                break;
            /*
            default: //Opened
                StartDialogEmpty();
                break;
            */
        }
    }

    // contentComponent
    public ItemData ItemData => chest.ItemData;
    public string ItemId => chest.ItemId;

    //persistenceComponent
    public bool IsPersistent => chest.IsPersistent;
    public bool IsOpenedPersistent => chest.IsOpenedPersistent;
    public void OpenPersistent() => chest.OpenPersistent();

    #endregion

    #region API Entity

    //interactableComponent

    //appearanceComponent
    public void SetSpriteOpened() => appearanceComponent.SetSpriteOpened();
    public void SetSpriteClosed() => appearanceComponent.SetSpriteClosed();

    //dialogComponent
    public void StartDialog() => dialogComponent.StartDialog();

    #endregion

    #region Misc

    public void Open() 
    {
        if (IsPersistent) OpenPersistent();
        SetState(ChestState.Opened);
    }

    #endregion
}
