using UnityEngine;

public class SelectorTeamEmblemAction : ISelectorClickAction<Emblem>
{
    public void Execute(Emblem obj, IClosableMenu menu)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaiseSelectorTeamEmblemActionClicked(obj);
    }
}
