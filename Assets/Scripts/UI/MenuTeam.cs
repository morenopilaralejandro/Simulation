using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;

/// <summary>
/// Root menu for team loadout management.
/// </summary>
public class MenuTeam : Menu
{
    public override void Show()
    {
        UIEvents.RaiseLoadoutSelectorOpenRequested(
            new SelectorLoadoutSource(),
            new SelectorLoadoutAction(),
            null
        );

        base.Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnTeamMenuClosed += HandleTeamMenuClosed;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnTeamMenuClosed -= HandleTeamMenuClosed;
    }

    private void HandleTeamMenuClosed() 
    {
        if (!MenuManager.Instance.IsMenuOpen(this)) return;
        RequestClose();
    }
}
