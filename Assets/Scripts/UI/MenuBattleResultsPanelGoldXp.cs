using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro; 

public class MenuBattleResultsPanelGoldXp : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private TMP_Text textGold;
    [SerializeField] private TMP_Text textXp;

    #endregion

    #region Lifecycle

    #endregion

    #region Logic

    public void SetData(int gold, int xp)
    {
        if (textGold != null)
            textGold.text = gold.ToString();

        if (textXp != null)
            textXp.text = xp.ToString();
    }

    public void Clear()
    {
        if (textGold != null)
            textGold.text = "0";

        if (textXp != null)
            textXp.text = "0";
    }

    #endregion

    #region Helper

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
