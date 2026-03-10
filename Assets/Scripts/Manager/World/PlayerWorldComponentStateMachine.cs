// PlayerMovementController.cs
using UnityEngine;
using Simulation.Enums.World;

public class PlayerWorldComponentStateMachine : MonoBehaviour
{
    private PlayerWorldEntity playerWorldEntity;
    private PlayerWorldConfig config;

    private PlayerWorldState state = PlayerWorldState.FreeRoam;
    public PlayerWorldState PlayerWorldState => state;

    public void Initialize(PlayerWorldEntity playerWorldEntity, PlayerWorldConfig cfg)
    {
        this.playerWorldEntity = playerWorldEntity;
        config = cfg;
    }

    public void SetState(PlayerWorldState newState)
    {
        if (newState == state) return;

        PlayerWorldState previous = state;
        state = newState;

        switch (newState)
        {
            case PlayerWorldState.FreeRoam:
                playerWorldEntity.SetControlEnabled(true);
                break;

            case PlayerWorldState.InDialogue:
            case PlayerWorldState.InMenu:
            case PlayerWorldState.InCutscene:
            case PlayerWorldState.InBattle:
                playerWorldEntity.SetControlEnabled(false);
                playerWorldEntity.StopMovement();
                break;
            case PlayerWorldState.Transitioning:
                playerWorldEntity.SetControlEnabled(false);
                playerWorldEntity.StopMovement();
                break;

            case PlayerWorldState.Paused:
                playerWorldEntity.SetControlEnabled(false);
                break;
        }

        WorldEvents.RaisePlayerStateChanged(previous, newState);
        LogManager.Trace($"[PlayerWorldManager] State: {previous} → {newState}");
    }

    #region Events

    private void OnEnable()
    {
        DialogEvents.OnDialogStarted += HandleDialogStarted;
        DialogEvents.OnDialogEnded += HandleDialogEnded;

        WorldEvents.OnMenuOpened += HandleMenuOpened;
        WorldEvents.OnMenuClosed -= HandleMenuClosed;
    }

    private void OnDisable()
    {
        DialogEvents.OnDialogStarted += HandleDialogStarted;
        DialogEvents.OnDialogEnded += HandleDialogEnded;
    }

    private void HandleDialogStarted() => SetState(PlayerWorldState.InDialogue);
    private void HandleDialogEnded() => SetState(PlayerWorldState.FreeRoam);

    private void HandleMenuOpened() => SetState(PlayerWorldState.InMenu);
    private void HandleMenuClosed() => SetState(PlayerWorldState.FreeRoam);

    #endregion

}
