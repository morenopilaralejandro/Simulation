using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.UI;

public class MenuCharacter : Menu
{
    public override void Show()
    {
        UIEvents.RaiseCharacterSelectorOpenRequested(
            source: new SelectorCharacterSourceFromStorage(),
            action: new SelectorCharacterActionOpenDetail(),
            filter: null,
            closeOnSelect: false);

        base.Show();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnBackFromCharacterSelectorRequested += HandleBackFromCharacterSelectorRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnBackFromCharacterSelectorRequested -= HandleBackFromCharacterSelectorRequested;
    }

    private void HandleBackFromCharacterSelectorRequested() 
    {
        if (!MenuManager.Instance.IsMenuOpen(this)) return;
        RequestClose();
    }
}
