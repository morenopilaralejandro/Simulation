using UnityEngine;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Input;

public class SelectorCharacter : Selector<Character, SelectorCharacterListItem>
{
    [Header("Apply Kit On Bind")]
    private Kit     kit;
    private Variant variant;
    private Role    role;

    public void Configure(Kit k, Variant v, Role r) { kit = k; variant = v; role = r; }

    protected override void Bind(SelectorCharacterListItem view, Character c)
    {
        c.ApplyKit(kit, variant, role);
        view.Bind(c);
    }

    protected override void OnGainedInput()
    {
        var im = InputManager.Instance;
        im.SubscribeDown(CustomAction.Navigation_Back, RequestClose);
        im.SubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter, OnFilterShortcut);
    }

    protected override void OnLostInput()
    {
        var im = InputManager.Instance;
        im.UnsubscribeDown(CustomAction.Navigation_Back, RequestClose);
        im.UnsubscribeDown(CustomAction.Navigation_ShortcutCharacterFilter, OnFilterShortcut);
    }

    private void OnFilterShortcut() => UIEvents.RaiseCharacterFilterRequested();

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnCharacterSelectorOpenRequested += HandleOpenRequested;
        UIEvents.OnCharacterFilterUpdated         += HandleFilterUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnCharacterSelectorOpenRequested -= HandleOpenRequested;
        UIEvents.OnCharacterFilterUpdated         -= HandleFilterUpdated;
    }

    private void HandleOpenRequested(
        ISelectorSource<Character>      source,
        ISelectorClickAction<Character> action,
        ISelectorFilter<Character>      filter,
        bool                            closeAfterPick)
    {
        closeOnSelect = closeAfterPick;
        Open(source, action, filter);
    }

    private void HandleFilterUpdated(CharacterFilterData data)
        => ApplyFilter(new CharacterFilterAdapter(data));
}
