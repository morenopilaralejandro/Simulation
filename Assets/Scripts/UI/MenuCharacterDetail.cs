using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;

public class MenuCharacterDetail : Menu
{
    #region Fields

    [Header("Basic")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private BarHPSP       barHp;
    [SerializeField] private BarHPSP       barSp;
    [SerializeField] private BarXP         barXp;
    [SerializeField] private TMP_Text      textLevel;

    [Header("Moves")]
    [SerializeField] private MoveLayoutUI moveLayoutUI;

    [Header("Stats")]
    [SerializeField] private StatLayoutUI statLayoutUI;

    [Header("Other")]
    [SerializeField] private Button firstSelected;

    private Character    character;
    private MoveSlotUI   pickedMoveSlot;

    private MenuStateMachine<CharacterDetailState> stateMachine;
    private Coroutine restoreFocusCoroutine;

    #endregion

    #region Lifecycle

    private void Start()
    {
        BuildStateMachine();
    }

    private void BuildStateMachine()
    {
        stateMachine = new MenuStateMachine<CharacterDetailState>(CharacterDetailState.Idle)
            .OnEnter(CharacterDetailState.SwappingMove, () =>
            {
                UIEvents.RaiseMoveSlotUIMoveStarted(pickedMoveSlot);
                if (pickedMoveSlot != null)
                    EventSystem.current.SetSelectedGameObject(pickedMoveSlot.gameObject);
            })
            .OnExit(CharacterDetailState.SwappingMove, () =>
            {
                UIEvents.RaiseMoveSlotUIMoveEnded(pickedMoveSlot);
            });
    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        InitializeUI();
        PopulateUI();
    }

    public override void Hide()
    {
        ClearUI();

        if (stateMachine != null && stateMachine.Is(CharacterDetailState.SwappingMove))
            stateMachine.Set(CharacterDetailState.Idle);

        base.Hide();
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);
    }

    public void Refresh()
    {
        InitializeUI();
        PopulateUI();
    }

    #endregion

    #region Populate

    private void InitializeUI()
    {
        if (character == null) return;
        moveLayoutUI.Initialize(character);
        statLayoutUI.Initialize(character);
    }

    private void PopulateUI()
    {
        if (character == null) return;

        characterCard.SetCharacter(character, Position.FW);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        textLevel.text = $"{character.Level}";

        moveLayoutUI.Populate();
        statLayoutUI.Populate();
    }

    private void ClearUI()
    {
        character = null;

        characterCard.Clear();
        barHp.Clear();
        barSp.Clear();
        barXp.Clear();
        textLevel.text = "";

        moveLayoutUI.Clear();
        statLayoutUI.Clear();
    }

    private IEnumerator RestoreFocusNextFrame(GameObject go)
    {
        // Wait for Refresh(), menu close, layout rebuild, OnDisable, etc.
        yield return null;

        Canvas.ForceUpdateCanvases();

        GameObject target = null;

        if (go != null && go.activeInHierarchy)
        {
            target = go;
        }
        else if (firstSelected != null)
        {
            target = firstSelected.gameObject;
        }

        if (target != null && target.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(target);
        }

        restoreFocusCoroutine = null;
    }

    #endregion

    #region Button Handlers

    public void OnButtonBackClicked()
    {
        if (stateMachine.Is(CharacterDetailState.SwappingMove))
        {
            stateMachine.Set(CharacterDetailState.Idle);
            return;
        }

        pickedMoveSlot = null;

        RequestClose();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnCharacterDetailOpenRequested    += HandleCharacterDetailOpenRequested;
        UIEvents.OnCharacterDetailRefreshRequested += HandleCharacterDetailRefreshRequested;
        UIEvents.OnMoveSlotUIClicked               += HandleMoveSlotUIClicked;
        UIEvents.OnMoveSlotUIMoveRequested         += HandleMoveSlotUIMoveRequested;
        UIEvents.OnMoveSlotUIMoveCanceled          += HandleMoveSlotUIMoveCanceled;
        UIEvents.OnMoveActionsCloseRequested       += HandleMoveActionsCloseRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnCharacterDetailOpenRequested    -= HandleCharacterDetailOpenRequested;
        UIEvents.OnCharacterDetailRefreshRequested -= HandleCharacterDetailRefreshRequested;
        UIEvents.OnMoveSlotUIClicked               -= HandleMoveSlotUIClicked;
        UIEvents.OnMoveSlotUIMoveRequested         -= HandleMoveSlotUIMoveRequested;
        UIEvents.OnMoveSlotUIMoveCanceled          -= HandleMoveSlotUIMoveCanceled;
        UIEvents.OnMoveActionsCloseRequested       -= HandleMoveActionsCloseRequested;
    }

    private void HandleCharacterDetailOpenRequested(Character character)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
        this.character = character;
        MenuManager.Instance.OpenMenu(this);
        SetDefaultSelectable(firstSelected);
    }

    private void HandleCharacterDetailRefreshRequested()
    {
        if (!MenuManager.Instance.IsMenuOpen(this)) return;
        Refresh();
    }

    private void HandleMoveSlotUIMoveRequested(MoveSlotUI slot)
    {
        if (!IsInteractable() || slot == null) return;
        if (!stateMachine.Is(CharacterDetailState.Idle)) return;

        pickedMoveSlot = slot;
        stateMachine.Set(CharacterDetailState.SwappingMove);
    }

    private void HandleMoveSlotUIMoveCanceled(MoveSlotUI _)
    {
        if (!stateMachine.Is(CharacterDetailState.SwappingMove)) return;
        stateMachine.Set(CharacterDetailState.Idle);
    }

    private void HandleMoveSlotUIClicked(MoveSlotUI slot)
    {
        if (!IsInteractable() || slot == null) return;

        if (stateMachine.Is(CharacterDetailState.SwappingMove))
        {
            if (pickedMoveSlot != null && pickedMoveSlot != slot && slot.Character != null)
                UIEvents.RaiseMoveSwapRequested(slot.Character, slot.Index, pickedMoveSlot.Index);

            stateMachine.Set(CharacterDetailState.Idle);
            return;
        }

        pickedMoveSlot = slot;
        UIEvents.RaiseMoveActionsOpenRequested(slot);
    }

    private void HandleMoveActionsCloseRequested(MoveSlotUI moveSlotUI) 
    {
        if (restoreFocusCoroutine != null)
            StopCoroutine(restoreFocusCoroutine);

        restoreFocusCoroutine = StartCoroutine(RestoreFocusNextFrame(pickedMoveSlot.gameObject));
    }

    #endregion
}
