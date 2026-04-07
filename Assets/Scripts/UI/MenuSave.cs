using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;
using System.Collections;
using Simulation.Enums.Battle;

/// <summary>
/// Menu for saving the game.
/// </summary>
public class MenuSave : Menu
{
    [Header("References")]
    [SerializeField] private SaveFileCard saveFileCard;
    [SerializeField] private CanvasGroup canvasButtons;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private Button buttonCancel;
    [SerializeField] private TMP_Text textMessage;
    [SerializeField] private LocalizedString stringSaveDialog;
    [SerializeField] private LocalizedString stringSaveProgress;
    [SerializeField] private LocalizedString stringSaveFinished;

    private MenuManager menuManager;
    private AudioManager audioManager;
    private PersistenceManager persistenceManager;
    private float coroutineCloseDuration = 0.5f;

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    public bool IsTeamMenuOpen => isOpen;

    #region Lifecycle

    private void Awake()
    {
        menuManager = MenuManager.Instance;
        audioManager = AudioManager.Instance;
        persistenceManager = PersistenceManager.Instance;
    }

    private void Start()
    {
        base.Hide();
        base.SetInteractable(false);
    }

    /*
 
    private void Update()
    {
        HandleInput();
    }

    */

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);

        saveFileCard.SetFromRuntime();
        textMessage.text = stringSaveDialog.GetLocalizedString();
        SetButtonVisible(true);
    }

    public override void Hide()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        base.SetInteractable(false);
        base.Hide();
    }

    #endregion

    #region Navigation

    public void OnButtonConfirmTapped()
    {
        textMessage.text = stringSaveProgress.GetLocalizedString();
        SetButtonVisible(false);
        persistenceManager.SaveGame();
        // Closes when event is fired
    }

    public void OnButtonCancelTapped()
    {
        Close();
    }

    public void Close()
    {
        Debug.LogError("Close");
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    private IEnumerator CoroutineCloseMenu()
    {
        Debug.LogError("CoroutineCloseMenu");
        yield return new WaitForSeconds(coroutineCloseDuration);
        Close();
    }

    #endregion

    #region Helpers

    public void SetButtonVisible(bool boolValue)
    {
        canvasButtons.alpha = boolValue ? 1f : 0f;
        canvasButtons.interactable = boolValue;
        canvasButtons.blocksRaycasts = boolValue;

        buttonConfirm.interactable = boolValue;
        buttonCancel.interactable = boolValue;
    }

    #endregion

    #region Input

    /*

    private void HandleInput()
    {
        // Placeholder for input handling

        if (isOpen)
        {
            if (InputManager.Instance.GetDown(CustomAction.World_CloseSideMenu))
                Close();
        }
        else
        {
            if (!WorldManager.Instance.PlayerCanOpenMenu) return;
            if (InputManager.Instance.GetDown(CustomAction.World_OpenSideMenu))
                Open();
        }

    }

    */

    #endregion

    #region Events

    private void OnEnable()
    {
        PersistenceEvents.OnGameSaved += HandleGameSaved;
    }

    private void OnDisable()
    {
        PersistenceEvents.OnGameSaved -= HandleGameSaved;
    }

    private void HandleGameSaved(SaveData saveData) 
    {
        Debug.LogError("event");
        saveFileCard.SetFromSaveData(saveData);
        textMessage.text = stringSaveFinished.GetLocalizedString();
        StartCoroutine(CoroutineCloseMenu());
    }

    #endregion

}
