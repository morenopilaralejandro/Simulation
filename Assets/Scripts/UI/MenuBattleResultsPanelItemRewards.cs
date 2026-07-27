using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuBattleResultsPanelItemRewards : MonoBehaviour
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private ScrollViewAutoScrollPoolItemDrop scrollViewPool;

    #endregion

    #region Lifecycle

    #endregion

    #region Logic

    public void SetData(List<ItemReward> rewards)
    {
        scrollViewPool.Populate(rewards, (ui, reward) =>
        {
            Item item = ItemFactory.CreateById(reward.ItemId);
            ui.SetData(item, reward.Quantity);
        });

        scrollViewPool.ActivateScroll();
    }

    public void Clear() 
    {
        scrollViewPool.Clear();
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
