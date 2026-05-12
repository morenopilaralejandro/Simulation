using UnityEngine;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Input;

public class MenuBattleActions : Menu
{
    [Header("UI References")]
    //[SerializeField] private ButtonDimensionAction buttonPause;

    private int openedFrame;

    public bool IsBattleMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    protected override void Awake()
    {
        base.Awake();
        BattleUIManager.Instance.RegisterBattleMenu(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterBattleMenu(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        var input = InputManager.Instance;

        // Can always attempt to open even if already closed
        input.SubscribeDown(CustomAction.BattleUI_OpenBattleMenu, HandleOpenInput);

        // Close input
        input.SubscribeDown(CustomAction.BattleUI_CloseBattleMenu, HandleCloseInput);

        // Shortcuts
        // input.SubscribeDown(CustomAction.BattleUI_DimensionShortcutPause, HandlePauseShortcut);

    }

    protected override void OnDisable()
    {
        base.OnDisable();

        var input = InputManager.Instance;

        input.UnsubscribeDown(CustomAction.BattleUI_OpenBattleMenu, HandleOpenInput);
        input.UnsubscribeDown(CustomAction.BattleUI_CloseBattleMenu, HandleCloseInput);

        // input.UnsubscribeDown(CustomAction.BattleUI_DimensionShortcutPause, HandlePauseShortcut);
    }

    protected override void OnGainedInput()
    {
        base.OnGainedInput();

        InputManager.Instance.SubscribeDown(
            CustomAction.BattleUI_CloseBattleMenu,
            HandleCloseInput
        );

        /* 
        input.SubscribeDown(
            CustomAction.BattleUI_ClickNorthButton,
            HandlePauseShortcut
        );
        */
    }

    protected override void OnLostInput()
    {
        base.OnLostInput();

        InputManager.Instance.UnsubscribeDown(
            CustomAction.BattleUI_CloseBattleMenu,
            HandleCloseInput
        );

        /*

        input.UnsubscribeDown(
            CustomAction.BattleUI_ClickNorthButton,
            HandlePauseShortcut
        );

        */
    }

    private void HandleOpenInput()
    {
        if (MenuManager.Instance.Count > 0 || 
            BattleManager.Instance.CurrentPhase == BattlePhase.TeamPreview) return;

        openedFrame = Time.frameCount;

        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleCloseInput()
    {
        if (Time.frameCount == openedFrame) return;

        if (!IsInteractable()) return;

        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        CloseMenu();
    }

    /*
    private void HandlePauseShortcut()
    {
        if (!IsInteractable()) return;

        buttonPause.OnButtonPressed();

        CloseMenu();
    }
    */

    private void CloseMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        RequestClose();
    }

    public void OnButtonDuelLogPressed()
    {
        BattleUIManager.Instance.OpenDuelLogMenu();
        // CloseMenu();
    }

    public void OnButtonAutoPressed()
    {
        AutoBattleManager.Instance.ToggleAutoBattle();
        CloseMenu();
    }

    public void OnButtonForfeitTapped()
    {
        BattleUIManager.Instance.OpenForfeitMenu();
        // CloseMenu();
    }

    public void OnButtonCloseTapped()
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_back");
        CloseMenu();
    }
}
