using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;
using System.Collections;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Input;

/// <summary>
/// Menu for saving the game.
/// </summary>
public class MenuSave : Menu
{
    [Header("UI References")]
    [SerializeField] private SaveFileCard saveFileCard;
    [SerializeField] private CanvasGroup canvasButtons;
    [SerializeField] private Button buttonConfirm;
    [SerializeField] private Button buttonCancel;
    [SerializeField] private TMP_Text textMessage;
    [SerializeField] private LocalizedString stringSaveDialog;
    [SerializeField] private LocalizedString stringSaveProgress;
    [SerializeField] private LocalizedString stringSaveFinished;

    private float coroutineCloseDuration = 0.5f;

    public override void Show() 
    {
        saveFileCard.SetFromRuntime();
        textMessage.text = stringSaveDialog.GetLocalizedString();
        SetButtonVisible(true);

        base.Show();
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonCancelClicked);

    public void OnButtonConfirmClicked()
    {
        textMessage.text = stringSaveProgress.GetLocalizedString();
        SetButtonVisible(false);
        PersistenceManager.Instance.SaveGame();
        // Closes when event is fired
    }

    public void OnButtonCancelClicked()
    {
        if(!buttonCancel.interactable) return;
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        PersistenceEvents.OnGameSaved += HandleGameSaved;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        PersistenceEvents.OnGameSaved -= HandleGameSaved;
    }

    private void HandleGameSaved(SaveData saveData) 
    {
        saveFileCard.SetFromSaveData(saveData);
        textMessage.text = stringSaveFinished.GetLocalizedString();
        StartCoroutine(CoroutineCloseMenu());
    }

    #region Helpers

    private IEnumerator CoroutineCloseMenu()
    {
        yield return new WaitForSeconds(coroutineCloseDuration);
        RequestClose();
    }

    public void SetButtonVisible(bool boolValue)
    {
        canvasButtons.alpha = boolValue ? 1f : 0f;
        canvasButtons.interactable = boolValue;
        canvasButtons.blocksRaycasts = boolValue;

        buttonConfirm.interactable = boolValue;
        buttonCancel.interactable = boolValue;
    }

    #endregion
}
