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
            SetAutoOrManualButtonActive();
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

    public void OnButtonDuelLogTapped()
    {
        Debug.Log("Duel Log button tapped.");
        // TODO: Show duel log UI

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
        Debug.Log("Forfeit button tapped.");
        ToggleBattleMenu();
        //TODO open forfeit menu
    }

    public void OnButtonPauseTapped()
    {
        Debug.Log("Pause button tapped.");
        // TODO: Pause the game
        // buttonPause.GetComponent<SpecialOption>().StartCooldown();

        ToggleBattleMenu();
    }

    public void OnButtonResumeTapped()
    {
        Debug.Log("Resume button tapped.");
        // TODO: Resume the game

        ToggleBattleMenu();
    }

    public void OnButtonSelected() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_change");
    }

}
