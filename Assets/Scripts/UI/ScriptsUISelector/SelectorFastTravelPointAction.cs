using UnityEngine;

public class SelectorFastTravelPointAction : ISelectorClickAction<FastTravelPoint>
{
    public void Execute(FastTravelPoint obj, IClosableMenu menu)
    {
        MenuManager.Instance.CloseAllMenus();
        WorldManager.Instance.TransitionToZone(obj.ZoneId, obj.SpawnPointId);
    }
}
