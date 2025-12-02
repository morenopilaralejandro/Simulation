using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class BattleMenu : Menu
{
    [Header("Buttons - Regular")]
    [SerializeField] private Button buttonDuelLog;
    [SerializeField] private Button buttonAuto;
    [SerializeField] private Button buttonManual;
    [SerializeField] private Button buttonForfeit;

    [Header("Buttons - Special")]
    [SerializeField] private Button buttonPause;
    [SerializeField] private Button buttonResume;

    private List<SpecialOption> specialOptions = new List<SpecialOption>();

    public bool IsBattleMenuOpen => MenuManager.Instance.IsMenuOpen(this);

    private void Awake()
    {
        BattleUIManager.Instance.RegisterBattleMenu(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterBattleMenu(this);
    }

    void Start()
    {
        specialOptions.Add(buttonPause.GetComponent<SpecialOption>());
        base.Hide();
        base.SetInteractable(false);
    }

    void Update()
    {
        //if (!IsInteractable()) return;

        HandleInput();
        RefreshSpecialButtons();
    }

    private void HandleInput()
    {
        if (IsBattleMenuOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_CloseBattleMenu))
                Close();
        } else 
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_OpenBattleMenu))
                Open();
        }

        // Number shortcuts for specials (1, 2, 3…)
        if (InputManager.Instance.GetDown(CustomAction.BattleUI_BattleMenuShortcutPause))
            OnButtonPauseTapped();
    }

    public override void Show()
    {
        base.Show();
        SetAutoOrManualButtonActive();
        SetPauseOrResumeButtonActive();
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        base.Hide();
    }

    public void Open()
    {
        if (MenuManager.Instance.CurrentMenu != null && 
            !MenuManager.Instance.IsMenuOnTop(this)) 
            return;

        if (IsBattleMenuOpen) return;
        MenuManager.Instance.OpenMenu(this);
    }

    public void Close()
    {
        if (!IsBattleMenuOpen) return;
        MenuManager.Instance.CloseMenu();
    }

    private void RefreshSpecialButtons()
    {
        foreach (var option in specialOptions)
            option.UpdateCooldown();
    }

    private void SetAutoOrManualButtonActive() 
    {
        if (SettingsManager.Instance.IsAutoBattleEnabled) 
        {
            buttonAuto.gameObject.SetActive(false);
            buttonManual.gameObject.SetActive(true);
        }
        else 
        {
            buttonAuto.gameObject.SetActive(true);
            buttonManual.gameObject.SetActive(false);
        }
    }

    private void SetPauseOrResumeButtonActive() 
    {
        if (PauseManager.Instance.IsPaused) 
        {
            buttonPause.gameObject.SetActive(false);
            buttonResume.gameObject.SetActive(true);
        }
        else 
        {
            buttonPause.gameObject.SetActive(true);
            buttonResume.gameObject.SetActive(false);
        }
    }

    public void OnButtonDuelLogTapped()
    {
        BattleUIManager.Instance.OpenDuelLogMenu();
        Close();
    }

    public void OnButtonAutoTapped()
    {
        AutoBattleManager.Instance.ToggleAutoBattle();
        Close();
    }

    public void OnButtonManualTapped()
    {
        AutoBattleManager.Instance.ToggleAutoBattle();
        Close();
    }

    public void OnButtonForfeitTapped()
    {
        BattleUIManager.Instance.OpenForfeitMenu();
    }

    public void OnButtonPauseTapped()
    {
        var specialOptionPause = buttonPause.GetComponent<SpecialOption>();

        if (specialOptionPause.IsOnCooldown() || 
            !PauseManager.Instance.CanPause()) return;

        if (IsBattleMenuOpen) Close();

        PauseManager.Instance.StartPause(BattleManager.Instance.GetUserSide());
        specialOptionPause.StartCooldown();
    }

    public void OnButtonResumeTapped()
    {
        if (!PauseManager.Instance.IsPaused) return; 

        Close();
        PauseManager.Instance.SetTeamReady(BattleManager.Instance.GetUserSide());
    }

    public void OnButtonSelected() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_change");
    }

}
