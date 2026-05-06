using UnityEngine;

public class SelectorTeamEmblemAction : ISelectorClickAction<SelectorTeamEmblemData>
{
    public void Execute(SelectorTeamEmblemData obj, IClosableMenu menu)
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        UIEvents.RaiseSelectorTeamEmblemActionClicked(obj);
    }
}
