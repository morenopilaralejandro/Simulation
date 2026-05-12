using UnityEngine;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class MenuBattleDimensionAction : Menu
{
    [Header("UI References")]
    [SerializeField] private ButtonDimensionAction buttonPause;
    // TODO add more for a total of 4 buttons
    // input is similar to move menu

    private int openedFrame;

    protected override void OnEnable()
    {
        base.OnEnable();

        var input = InputManager.Instance;

        // Can always attempt to open even if already closed
        input.SubscribeDown(CustomAction.BattleUI_OpenDimensionMenu, HandleOpenInput);

        // Close input
        input.SubscribeDown(CustomAction.BattleUI_CloseDimensionMenu, HandleCloseInput);

        // Shortcuts
        input.SubscribeDown(CustomAction.BattleUI_DimensionShortcutPause, HandlePauseShortcut);

        BattleEvents.OnBattlePhaseChanged += HandleBattlePhaseChanged;
        BattleEvents.OnBattlePause += HandleBattlePause;
        BattleEvents.OnBattleResume += HandleBattleResumed;
        UIEvents.OnButtonDimensionActionPressed += HandleButtonDimensionActionPressed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var input = InputManager.Instance;

        input.UnsubscribeDown(CustomAction.BattleUI_OpenDimensionMenu, HandleOpenInput);
        input.UnsubscribeDown(CustomAction.BattleUI_CloseDimensionMenu, HandleCloseInput);

        input.UnsubscribeDown(CustomAction.BattleUI_DimensionShortcutPause, HandlePauseShortcut);

        BattleEvents.OnBattlePhaseChanged -= HandleBattlePhaseChanged;
        BattleEvents.OnBattlePause -= HandleBattlePause;
        BattleEvents.OnBattleResume -= HandleBattleResumed;
        UIEvents.OnButtonDimensionActionPressed -= HandleButtonDimensionActionPressed;
    }

    protected override void OnGainedInput()
    {
        base.OnGainedInput();

        InputManager.Instance.SubscribeDown(
            CustomAction.BattleUI_CloseDimensionMenu,
            HandleCloseInput
        );

        InputManager.Instance.SubscribeDown(
            CustomAction.BattleUI_ClickWestButton,
            HandlePauseShortcut
        );

        InputManager.Instance.SubscribeDown(
            CustomAction.BattleUI_CloseMoveMenu,
            OnButtonClosePressed
        );
    }

    protected override void OnLostInput()
    {
        base.OnLostInput();

        InputManager.Instance.UnsubscribeDown(
            CustomAction.BattleUI_CloseDimensionMenu,
            HandleCloseInput
        );

        InputManager.Instance.UnsubscribeDown(
            CustomAction.BattleUI_ClickWestButton,
            HandlePauseShortcut
        );

        InputManager.Instance.UnsubscribeDown(
            CustomAction.BattleUI_CloseMoveMenu,
            OnButtonClosePressed
        );
    }

    private void HandleOpenInput()
    {
        if (MenuManager.Instance.Count > 0 ||
            BattleManager.Instance.IsTimeFrozen)
            return;

        openedFrame = Time.frameCount;

        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleCloseInput()
    {
        if (Time.frameCount == openedFrame) return;

        if (!IsInteractable()) return;

        EventSystem.current.SetSelectedGameObject(null);

        RequestClose();
    }

    private void HandlePauseShortcut()
    {
        // if (!IsInteractable()) return;

        buttonPause.OnButtonPressed();
    }

    private void HandleBattlePhaseChanged(BattlePhase newPhase, BattlePhase oldPhase)
    {
        if (newPhase != BattlePhase.Battle)
        {
            CloseMenu();
        }
    }

    private void HandleBattlePause(TeamSide teamside) 
    {
        //buttonPause.SetTimeFrozen(true);
    }

    private void HandleBattleResumed() 
    {
        buttonPause.StartCooldown();
    }

    private void HandleButtonDimensionActionPressed(string dimensionActionId) 
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        RequestClose();
    }

    public void OnButtonClosePressed()
    {
        CloseMenu();
    }
}
