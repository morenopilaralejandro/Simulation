using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Aremoreno.Enums.Battle;

public class MainMenu : MonoBehaviour
{

    #region Fields

    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelSettings;
    [SerializeField] private GameObject panelCredits;
    [SerializeField] private GameObject panelStory;
    [SerializeField] private GameObject panelStoryConfirm;

    [SerializeField] private GameObject firstSelectedMain;
    [SerializeField] private GameObject firstSelectedSettings;
    [SerializeField] private GameObject firstSelectedCredits;
    [SerializeField] private GameObject firstSelectedStory;
    [SerializeField] private GameObject firstSelectedStoryConfirm;

    [SerializeField] private SaveFileCard saveFileCard;
    [SerializeField] private Button buttonContinueGame;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneWorld;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;
    private PersistenceManager persistenceManager;
    private bool hasSaveData;

    #endregion

    private void Start() 
    {
        InputEvents.RaiseScreenControlsHideRequested();

        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;
        persistenceManager = PersistenceManager.Instance;
        hasSaveData = persistenceManager.HasSaveData();

        buttonContinueGame.interactable = hasSaveData;

        audioManager.PlayBgm("bgm-simulation");    
        panelMain.SetActive(true);
        panelSettings.SetActive(false);
        panelCredits.SetActive(false);
        panelStory.SetActive(false);
        panelStoryConfirm.SetActive(false);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    #region DreamMatch

    public void OnButtonDreamMatchTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        BattleArgs.SetFull(
            "faith_selection", 
            "crimson_selection");
        sceneLoader.LoadGroup(sceneBattle);
    }

    #endregion

    #region Settings

    public void OnButtonSettingsTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelSettings.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedSettings);
    }

    public void OnButtonSettingsContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_back");
        panelSettings.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    #endregion

    #region Credits

    public void OnButtonCreditsTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelCredits.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedCredits);
    }

    public void OnButtonCreditsContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_back");
        panelCredits.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    #endregion

    #region Quit

    public void OnButtonQuitTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        Application.Quit();
    }

    #endregion

    #region Story

    public void OnButtonStoryTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelMain.SetActive(false);
        panelStory.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedStory);
    }

    public void OnButtonStoryBackTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelStory.SetActive(false);
        panelMain.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedMain);
    }

    public void OnButtonStoryContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        ContinueGame();
    }

    public void OnButtonStoryNewGameTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");

        if (!hasSaveData) 
        {
            StartNewGame();
            return;
        }

        // show confirm
        panelStory.SetActive(false);
        panelStoryConfirm.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedStoryConfirm);
    }

    public void OnButtonStoryNewGameConfirmTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        StartNewGame();
    }

    public void OnButtonStoryNewGameCancelTapped()
    {
        audioManager.PlaySfx("sfx-menu_tap");
        panelStoryConfirm.SetActive(false);
        panelStory.SetActive(true);
        EventSystem.current.SetSelectedGameObject(firstSelectedStory);
    }

    private void StartNewGame() 
    {
        persistenceManager.StartNewGame();
        sceneLoader.LoadGroup(sceneWorld);
    }

    private void ContinueGame() 
    {
        persistenceManager.LoadGame();
        sceneLoader.LoadGroup(sceneWorld);
    }

    #endregion

}
