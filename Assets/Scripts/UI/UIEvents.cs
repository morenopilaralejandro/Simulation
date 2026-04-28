using System;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.UI;

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

    public static event Action<Team> OnMenuTeamBattleRequested;
    public static void RaiseMenuTeamBattleRequested(Team team)
    {
        OnMenuTeamBattleRequested?.Invoke(team);
    }

    public static event Action OnTeamLoadoutCreateRequested;
    public static void RaiseTeamLoadoutCreateRequested()
    {
        OnTeamLoadoutCreateRequested?.Invoke();
    }

    public static event Action<Team> OnTeamLoadoutDeleteRequested;
    public static void RaiseTeamLoadoutDeleteRequested(Team team)
    {
        OnTeamLoadoutDeleteRequested?.Invoke(team);
    }

    public static event Action<Team> OnTeamPanelDeleteOpened;
    public static void RaiseTeamPanelDeleteOpened(Team team)
    {
        OnTeamPanelDeleteOpened?.Invoke(team);
    }

    public static event Action<Team, bool> OnBackFromTeamRequested;
    public static void RaiseBackFromTeamRequested(Team team, bool hasSwapped)
    {
        OnBackFromTeamRequested?.Invoke(team, hasSwapped);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUISelectedDefault;
    public static void RaiseFormationCharacterSlotUISelectedDefault(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUISelectedDefault?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUISelected;
    public static void RaiseFormationCharacterSlotUISelected(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUISelected?.Invoke(slot);
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

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIMoveRequested;
    public static void RaiseFormationCharacterSlotUIMoveRequested(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIMoveRequested?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIMoveStarted;
    public static void RaiseFormationCharacterSlotUIMoveStarted(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIMoveStarted?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIMoveEnded;
    public static void RaiseFormationCharacterSlotUIMoveEnded(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIMoveEnded?.Invoke(slot);
    }

    public static event Action<FormationCharacterSlotUI> OnFormationCharacterSlotUIMoveCanceled;
    public static void RaiseFormationCharacterSlotUIMoveCanceled(FormationCharacterSlotUI slot)
    {
        OnFormationCharacterSlotUIMoveCanceled?.Invoke(slot);
    }

    public static event Action<GameObject> OnTeamButtonSelected;
    public static void RaiseTeamButtonSelected(GameObject gameObject)
    {
        OnTeamButtonSelected?.Invoke(gameObject);
    }

    public static event Action<Team, BattleType> OnTeamActionsOpened;
    public static void RaiseTeamActionsOpened(Team team, BattleType battleType)
    {
        OnTeamActionsOpened?.Invoke(team, battleType);
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

    public static event Action<Character> OnTeamCharacterActionsOpenRequested;
    public static void RaiseTeamCharacterActionsOpenRequested(Character character)
    {
        OnTeamCharacterActionsOpenRequested?.Invoke(character);
    }

    public static event Action OnTeamCharacterActionsClosed;
    public static void RaiseTeamCharacterActionsClosed()
    {
        OnTeamCharacterActionsClosed?.Invoke();
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

    public static event Action<Character> OnCharacterDetailOpenRequested;
    public static void RaiseCharacterDetailOpenRequested(Character character)
    {
        OnCharacterDetailOpenRequested?.Invoke(character);
    }

    public static event Action<int, int> OnSubstitutionChangesUpdated;
    public static void RaiseSubstitutionChangesUpdated(int currentValue, int maxValue)
    {
        OnSubstitutionChangesUpdated?.Invoke(currentValue, maxValue);
    }

    public static event Action OnTeamPreviewButtonContinueClicked;
    public static void RaiseTeamPreviewButtonContinueClicked()
    {
        OnTeamPreviewButtonContinueClicked?.Invoke();
    }

    // Menu Character
    public static event Action<Character> OnCharacterSelected;
    public static void RaiseCharacterSelected(Character character)
    {
        OnCharacterSelected?.Invoke(character);
    }

    public static event Action<CharacterSelectorMode, Team, BattleType, bool> OnCharacterSelectorOpenRequested;
    public static void RaiseCharacterSelectorOpenRequested(
        CharacterSelectorMode mode,
        Team team, 
        BattleType battleType, 
        bool isCloseOnSelect)
    {
        OnCharacterSelectorOpenRequested?.Invoke(mode, team, battleType, isCloseOnSelect);
    }

    public static event Action OnBackFromCharacterSelectorRequested;
    public static void RaiseBackFromCharacterSelectorRequested()
    {
        OnBackFromCharacterSelectorRequested?.Invoke();
    }

    public static event Action<Character> OnCharacterDetailSideUpdateRequested;
    public static void RaiseCharacterDetailSideUpdateRequested(Character character)
    {
        OnCharacterDetailSideUpdateRequested?.Invoke(character);
    }

    public static event Action OnCharacterDetailSideNextPageRequested;
    public static void RaiseCharacterDetailSideNextPageRequested()
    {
        OnCharacterDetailSideNextPageRequested?.Invoke();
    }

    public static event Action OnCharacterFilterRequested;
    public static void RaiseCharacterFilterRequested()
    {
        OnCharacterFilterRequested?.Invoke();
    }

    public static event Action<CharacterFilterData> OnCharacterFilterUpdated;
    public static void RaiseCharacterFilterUpdated(CharacterFilterData characterFilterData)
    {
        OnCharacterFilterUpdated?.Invoke(characterFilterData);
    }

    public static event Action OnCharacterFilterResetRequested;
    public static void RaiseCharacterFilterResetRequested()
    {
        OnCharacterFilterResetRequested?.Invoke();
    }

    // Menu Move

    public static event Action<Move, Character> OnMoveActionsOpenRequested;
    public static void RaiseMoveActionsOpenRequested(Move move, Character character)
    {
        OnMoveActionsOpenRequested?.Invoke(move, character);
    }

    public static event Action<Move, Character> OnMoveReplaceRequested;
    public static void RaiseMoveReplaceRequested(Move move, Character character)
    {
        OnMoveReplaceRequested?.Invoke(move, character);
    }

    public static event Action<Move, Move> OnMoveReplaced;
    public static void RaiseMoveReplaced(Move moveOld, Move moveNew)
    {
        OnMoveReplaced?.Invoke(moveOld, moveNew);
    }

    public static event Action<Move, Character> OnMoveSelectorOpenRequested;
    public static void RaiseMoveSelectorOpenRequested(Move move, Character character)
    {
        OnMoveSelectorOpenRequested?.Invoke(move, character);
    }

    public static event Action<Move> OnMoveSelected;
    public static void RaiseMoveSelected(Move move)
    {
        OnMoveSelected?.Invoke(move);
    }

    // Menu Item
    public static event Action<Item> OnItemSelected;
    public static void RaiseItemSelected(Item item)
    {
        OnItemSelected?.Invoke(item);
    }

    public static event Action<ItemCategory, BattleType> OnItemSelectorSideOpened;
    public static void RaiseItemSelectorSideOpened(ItemCategory category, BattleType battleType)
    {
        OnItemSelectorSideOpened?.Invoke(category, battleType);
    }

    public static event Action<ItemCategory> OnBackFromSelectorItemSideRequested;
    public static void RaiseBackFromSelectorItemSideRequested(ItemCategory category)
    {
        OnBackFromSelectorItemSideRequested?.Invoke(category);
    }

    public static event Action<SelectorItemSideListItem> OnSelectorItemSideListItemHighlighted;
    public static void RaiseSelectorItemSideListItemHighlighted(SelectorItemSideListItem listItem)
    {
        OnSelectorItemSideListItemHighlighted?.Invoke(listItem);
    }

    public static event Action<SelectorItemSideListItem> OnSelectorItemSideListItemSelected;
    public static void RaiseSelectorItemSideListItemSelected(SelectorItemSideListItem listItem)
    {
        OnSelectorItemSideListItemSelected?.Invoke(listItem);
    }
}
