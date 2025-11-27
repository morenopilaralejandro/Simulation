using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class BattleMenu : MonoBehaviour
{
    [Header("Buttons - Regular")]
    [SerializeField] private Button buttonDuelLog;
    [SerializeField] private Button buttonAuto;
    [SerializeField] private Button buttonManual;
    [SerializeField] private Button buttonForfeit;

    [Header("Buttons - Special")]
    [SerializeField] private Button buttonPause;
    [SerializeField] private Button buttonResume;

    [SerializeField] private GameObject firstSelected;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private bool isBattleMenuOpen;
    private List<SpecialOption> specialOptions;

    public bool IsBattleMenuOpen => isBattleMenuOpen;

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
        SetBattleMenuActive(false);
        specialOptions = new List<SpecialOption>();
        specialOptions.Add(buttonPause.GetComponent<SpecialOption>());
    }

    void Update()
    {
        HandleInput();
        RefreshSpecialButtons();
    }

    private void HandleInput()
    {
        if (isBattleMenuOpen) 
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_CloseBattleMenu))
                ToggleBattleMenu();
        } else 
        {
            if (InputManager.Instance.GetDown(CustomAction.BattleUI_OpenBattleMenu))
                ToggleBattleMenu();
        }

        // Number shortcuts for specials (1, 2, 3…)
        if (InputManager.Instance.GetDown(CustomAction.BattleUI_BattleMenuShortcutPause))
            OnButtonPauseTapped();
    }

    public void ToggleBattleMenu()
    {
        isBattleMenuOpen = !isBattleMenuOpen;
        SetBattleMenuActive(isBattleMenuOpen);

        if (isBattleMenuOpen) 
        {
            if (BattleUIManager.Instance.IsDuelLogMenuOpen)
                BattleUIManager.Instance.ToggleDuelLogMenu();
            SetAutoOrManualButtonActive();
            SetPauseOrResumeButtonActive();
            EventSystem.current.SetSelectedGameObject(firstSelected);
        }

        AudioManager.Instance.PlaySfx("sfx-menu_tap");
    }

    public void SetBattleMenuActive(bool active)
    {
        menuPanel.SetActive(active);
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
        BattleUIManager.Instance.ToggleDuelLogMenu();

        ToggleBattleMenu();
    }

    public void OnButtonAutoTapped()
    {
        AutoBattleManager.Instance.ToggleAutoBattle();

        ToggleBattleMenu();
    }

    public void OnButtonManualTapped()
    {
        AutoBattleManager.Instance.ToggleAutoBattle();

        ToggleBattleMenu();
    }

    public void OnButtonForfeitTapped()
    {
        ToggleBattleMenu();
        if (!BattleUIManager.Instance.IsForfeitMenuOpen)
            BattleUIManager.Instance.ToggleForfeitMenu();
    }

    public void OnButtonPauseTapped()
    {
        var specialOptionPause = buttonPause.GetComponent<SpecialOption>();

        if (specialOptionPause.IsOnCooldown() || 
            !PauseManager.Instance.CanPause()) return;

        if (isBattleMenuOpen)
            ToggleBattleMenu();
        PauseManager.Instance.StartPause(BattleManager.Instance.GetUserSide());
        specialOptionPause.StartCooldown();
    }

    public void OnButtonResumeTapped()
    {
        if (!PauseManager.Instance.IsPaused) return; 

        ToggleBattleMenu();
        PauseManager.Instance.SetTeamReady(BattleManager.Instance.GetUserSide());
    }

    public void OnButtonSelected() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_change");
    }

}
