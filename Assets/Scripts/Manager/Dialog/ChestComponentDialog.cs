using UnityEngine;
using UnityEngine.InputSystem;
using Simulation.Enums.World;

public class ChestComponentDialog : MonoBehaviour
{
    private ChestEntity chestEntity;

    // dialogKnot
    private const string DIALOG_OPEN = "chest_open";
    private const string DIALOG_EMPTY = "chest_empty";
    private const string DIALOG_LOCKED = "chest_locked";

    private DialogManager dialogManager;
    private InkStoryManager inkStoryManager;
    private DialogLocalizationBridge localizationBridge;
    private IDialogGameDataProvider dialogGameDataProvider;

    public void Initialize(ChestEntity chestEntity)
    {
        this.ChestEntity = chestEntity;
        dialogManager = DialogManager.Instance;
        inkStoryManager = dialogManager.InkStoryManager;
        localizationBridge = dialogManager.DialogLocalizationBridge;
        dialogGameDataProvider = dialogManager.DialogGameDataProvider;
    }

    public void StartDialog()
    {
        ResolveDialogKnot();
    }

    private void ResolveDialogKnot()
    {
        switch (chestEntity.State) 
        {
            case Locked:
                StartDialogLocked();
                break;
            case Closed:
                StartDialogOpen();
                break:
            default: //Opened
                StartDialogEmpty();
                break;
        }
    }

    private void StartDialogLocked() 
    {
        dialogManager.StartDialog(DIALOG_LOCKED);
    }

    private void StartDialogOpen() 
    {
        inkStoryManager.SetVariable(
            "chest_item_name", 
            localizationBridge.ResolveItemName(chestEntity.Item.ItemId));

        inkStoryManager.SetVariable(
            "chest_item_count", 
            1);

        dialogManager.StartDialog(DIALOG_OPEN);
    }

    private void StartDialogEmpty() 
    {
        dialogManager.StartDialog(DIALOG_EMPTY);
    }

}
