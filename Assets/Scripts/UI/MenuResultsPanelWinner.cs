using System;
using UnityEngine;
using UnityEngine.UI;

public class MenuResultsPanelWinner : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasWin;
    [SerializeField] private CanvasGroup canvasLose;

    #endregion

    #region Lifecycle

    #endregion

    #region Logic

    public void SetData(BattleResultData data) 
    {
        SetCanvasGroupVisibility(canvasWin, data.IsUserWin);
        SetCanvasGroupVisibility(canvasLose, !data.IsUserWin);
    }

    public void Clear() 
    {
        SetCanvasGroupVisibility(canvasWin, false);
        SetCanvasGroupVisibility(canvasLose, false);
    }

    #endregion

    #region Helper

    public static void SetCanvasGroupVisibility(CanvasGroup canvasGroup, bool visible)
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    #endregion

    /*

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnResultsArcadeOpenRequested += HandleResultsArcadeOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnResultsArcadeOpenRequested -= HandleResultsArcadeOpenRequested;
    }

    private void HandleResultsArcadeOpenRequested(string teamName)
    {
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion

    */

}
