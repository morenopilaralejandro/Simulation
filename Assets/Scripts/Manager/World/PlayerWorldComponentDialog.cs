using UnityEngine;
using Simulation.Enums.Input;
using Simulation.Enums.World;

public class PlayerWorldComponentDialog : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;
    private DialogManager dialogManager;
    private InputManager inputManager;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
        dialogManager = DialogManager.Instance;
        inputManager = InputManager.Instance;
    }

    private void Update()
    {
        if (!dialogManager.CanAcceptInput) return;
        if (inputManager.GetDown(CustomAction.Dialog_Submit))
            dialogManager.InputPressed();
    }

}
