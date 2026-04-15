using System;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Item;

public static class UIEvents
{
    // Menu Common
    public static event Action<Menu> OnMenuOpened;
    public static void RaiseMenuOpened(Menu menu)
    {
        OnMenuOpened?.Invoke(menu);
    }

    public static event Action<Menu> OnMenuClosed;
    public static void RaiseMenuClosed(Menu menu)
    {
        OnMenuClosed?.Invoke(menu);
    }

    public static event Action OnAllMenusClosed;
    public static void RaiseAllMenusClosed()
    {
        OnAllMenusClosed?.Invoke();
    }

    // Menu Team
    public static event Action OnTeamMenuClosed;
    public static void RaiseTeamMenuClosed()
    {
        OnTeamMenuClosed?.Invoke();
    }

    public static event Action OnBattleTypeChangeRequested;
    public static void RaiseBattleTypeChangeRequested()
    {
        OnBattleTypeChangeRequested?.Invoke();
    }

    public static event Action<BattleType, BattleType> OnBattleTypeChanged;
    public static void RaiseBattleTypeChanged(BattleType newType, BattleType oldType)
    {
        OnBattleTypeChanged?.Invoke(newType, oldType);
    }

    public static event Action OnTeamLoadoutRequested;
    public static void RaiseTeamLoadoutRequested()
    {
        OnTeamLoadoutRequested?.Invoke();
    }

    public static event Action<Team> OnTeamLoadoutSelected;
    public static void RaiseTeamLoadoutSelected(Team team)
    {
        OnTeamLoadoutSelected?.Invoke(team);
    }

    public static event Action OnTeamLoadoutCreateRequested;
    public static void RaiseTeamLoadoutCreateRequested()
    {
        OnTeamLoadoutCreateRequested?.Invoke();
    }

    public static event Action OnBackFromTeamRequested;
    public static void RaiseBackFromTeamRequested()
    {
        OnBackFromTeamRequested?.Invoke();
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUISelectedDefault;
    public static void RaiseFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUISelectedDefault?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIClicked;
    public static void RaiseFormationCharacterSlotUIClicked(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIClicked?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIHighlited;
    public static void RaiseFormationCharacterSlotUIHighlited(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIHighlited?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI, FormationCharacterSlotUI> OnFormationCharacterSlotUISwaped;
    public static void RaiseFormationCharacterSlotUISwaped(FormationCharacterSlotUI a, FormationCharacterSlotUI b)
    {
        OnFormationCharacterSlotUISwaped?.Invoke(a, b);
    }

    public static event Action<FormationCharacterSlotUI, Character> OnFormationCharacterSlotUIReplaced;
    public static void RaiseFormationCharacterSlotUIReplaced(FormationCharacterSlotUI slot, Character character)
    {
        OnFormationCharacterSlotUIReplaced?.Invoke(slot, character);
    }

    public static event Action<GameObject> OnTeamButtonSelected;
    public static void RaiseTeamButtonSelected(GameObject gameObject)
    {
        OnTeamButtonSelected?.Invoke(gameObject);
    }

    public static event Action<Team> OnTeamActionsOpened;
    public static void RaiseTeamActionsOpened(Team team)
    {
        OnTeamActionsOpened?.Invoke(team);
    }

    public static event Action OnBackFromTeamActionsRequested;
    public static void RaiseBackFromTeamActionsRequested()
    {
        OnBackFromTeamActionsRequested?.Invoke();
    }

    public static event Action OnTeamActionsClosed;
    public static void RaiseTeamActionsClosed()
    {
        OnTeamActionsClosed?.Invoke();
    }

    public static event Action OnCharacterActionsOpened;
    public static void RaiseCharacterActionsOpened()
    {
        OnCharacterActionsOpened?.Invoke();
    }

    public static event Action OnFormationCharacterSlotUIReplaceRequested;
    public static void RaiseFormationCharacterSlotUIReplaceRequested()
    {
        OnFormationCharacterSlotUIReplaceRequested?.Invoke();
    }

    public static event Action<string> OnTeamPanelNameOpened;
    public static void RaiseTeamPanelNameOpened(string teamName)
    {
        OnTeamPanelNameOpened?.Invoke(teamName);
    }

    public static event Action<string> OnTeamNameChanged;
    public static void RaiseTeamNameChanged(string teamName)
    {
        OnTeamNameChanged?.Invoke(teamName);
    }

    public static event Action<Sprite> OnTeamPanelEmblemOpened;
    public static void RaiseTeamPanelEmblemOpened(Sprite emblemSprite)
    {
        OnTeamPanelEmblemOpened?.Invoke(emblemSprite);
    }

    public static event Action OnEmblemSelectorOpened;
    public static void RaiseEmblemSelectorOpened()
    {
        OnEmblemSelectorOpened?.Invoke();
    }

    public static event Action<string, Sprite> OnTeamEmblemSelected;
    public static void RaiseTeamEmblemSelected(string emblemId, Sprite emblemSprite)
    {
        OnTeamEmblemSelected?.Invoke(emblemId, emblemSprite);
    }

    public static event Action<string> OnTeamEmblemChanged;
    public static void RaiseTeamEmblemChanged(string emblemId)
    {
        OnTeamEmblemChanged?.Invoke(emblemId);
    }

    public static event Action OnCharacterDetailOpened;
    public static void RaiseCharacterDetailOpened()
    {
        OnCharacterDetailOpened?.Invoke();
    }

    // Menu Character
    public static event Action<Character> OnCharacterSelected;
    public static void RaiseCharacterSelected(Character character)
    {
        OnCharacterSelected?.Invoke(character);
    }

    public static event Action OnCharacterSelectorOpened;
    public static void RaiseCharacterSelectorOpened()
    {
        OnCharacterSelectorOpened?.Invoke();
    }

    // Menu Item
    public static event Action<Item> OnItemSelected;
    public static void RaiseItemSelected(Item item)
    {
        OnItemSelected?.Invoke(item);
    }

    public static event Action<ItemCategory> OnItemSelectorSideOpened;
    public static void RaiseItemSelectorSideOpened(ItemCategory category)
    {
        OnItemSelectorSideOpened?.Invoke(category);
    }
}
