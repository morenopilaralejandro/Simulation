using UnityEngine;
using Aremoreno.Enums.World;

public class SelectorFastTravelPointAction : ISelectorClickAction<FastTravelPoint>
{
    public void Execute(FastTravelPoint obj, IClosableMenu menu)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        MenuManager.Instance.CloseAllMenus();
        UIEvents.RaiseMenuSideCloseRequested();
        WorldManager.Instance.TransitionToZone(obj.ZoneId, obj.SpawnPointId);
        WorldManager.Instance.PlayerWorldEntity.SetState(PlayerWorldState.FreeRoam);
    }
}
