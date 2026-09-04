using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuSideButtonOpen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private CanvasGroup canvasGroup;

    private void SetVisible(bool isInteractable)
    {
        canvasGroup.alpha = isInteractable ? 1f : 0f;
        canvasGroup.interactable = isInteractable;
        canvasGroup.blocksRaycasts = isInteractable;
    }

    public void OnButtonClicked()
    {
        UIEvents.RaiseMenuSideOpenRequested();
    }

    private void OnEnable()
    {
        DialogEvents.OnDialogStarted += HandleDialogStarted;
        DialogEvents.OnDialogEnded += HandleDialogEnded;

        UIEvents.OnMenuSideOpened += HandleMenuSideOpened;
        UIEvents.OnMenuSideClosed += HandleMenuSideClosed;
    }

    private void OnDisable()
    {
        DialogEvents.OnDialogStarted -= HandleDialogStarted;
        DialogEvents.OnDialogEnded -= HandleDialogEnded;

        UIEvents.OnMenuSideOpened -= HandleMenuSideOpened;
        UIEvents.OnMenuSideClosed -= HandleMenuSideClosed;
    }

    private void HandleDialogStarted()
    {
        SetVisible(false);
    }

    private void HandleDialogEnded()
    {
        SetVisible(true);
    }

    private void HandleMenuSideOpened()
    {
        SetVisible(false);
    }

    private void HandleMenuSideClosed()
    {
        SetVisible(true);
    }
}
