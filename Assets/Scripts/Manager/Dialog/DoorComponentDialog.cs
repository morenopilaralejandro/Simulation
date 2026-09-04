using UnityEngine;
using Aremoreno.Enums.World;

public class DoorComponentDialog : MonoBehaviour, IInteractable
{
    private DoorEntity doorEntity;

    // dialogKnot
    //private const string DIALOG_OPEN = "chest_open";
    private const string DIALOG_LOCKED = "door_locked";

    private DialogManager dialogManager;
    private InkStoryManager inkStoryManager;
    private DialogLocalizationBridge localizationBridge;
    private IDialogGameDataProvider dialogGameDataProvider;

    public void Initialize(DoorEntity doorEntity)
    {
        this.doorEntity = doorEntity;
        dialogManager = DialogManager.Instance;
        inkStoryManager = dialogManager.InkStoryManager;
        localizationBridge = dialogManager.DialogLocalizationBridge;
        dialogGameDataProvider = dialogManager.DialogGameDataProvider;
    }

    public void Interact() 
    {
        StartDialog();
    }

    public void StartDialog()
    {
        ResolveDialogKnot();
    }

    private void ResolveDialogKnot()
    {
        switch (doorEntity.State) 
        {
            case DoorState.Locked:
                StartDialogLocked();
                break;
        }
    }

    private void StartDialogLocked() 
    {
        if (!doorEntity.TryOpen())
            dialogManager.StartDialog(DIALOG_LOCKED);
    }

}
