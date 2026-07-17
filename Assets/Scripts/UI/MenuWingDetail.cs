using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Wing;

public class MenuWingDetail : Menu
{
    #region Fields

    [Header("Basic")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private TMP_Text textName;

    [Header("Stats")]
    [SerializeField] private StatLayoutUI statLayoutUI;

    [Header("Character")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private CanvasGroup characterCardCanvas;

    [Header("Other")]
    [SerializeField] private Button firstSelected;

    private Wing wing;

    //private MenuStateMachine<CharacterDetailState> stateMachine;
    //private Coroutine restoreFocusCoroutine;

    #endregion

    #region Lifecycle

    private void Start()
    {
        //BuildStateMachine();
    }

    /*

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

    */

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

        /*
        if (stateMachine != null && stateMachine.Is(CharacterDetailState.SwappingMove))
            stateMachine.Set(CharacterDetailState.Idle);
        */

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
        if (wing == null) return;
        //statLayoutUI.Initialize(character);
    }

    private void PopulateUI()
    {
        if (wing == null) return;

        textName.text = wing.WingName;
        imageIcon.color = ColorManager.GetWingColor(wing.WingColorType);

        statLayoutUI.Populate(wing);
        if(wing.IsEquipped()) 
        {
            wing.EquippedCharacter.SetKit(
                TeamManager.Instance.ActiveLoadout.Kit, 
                TeamManager.Instance.ActiveLoadout.Variant, 
                Role.Field);
            characterCard.SetCharacter(wing.EquippedCharacter, wing.EquippedCharacter.Position);
            SetVisible(characterCardCanvas, true);
        } else 
        {
            SetVisible(characterCardCanvas, false);
        }
    }

    private void ClearUI()
    {
        wing = null;

        textName.text = "";
        imageIcon.color = Color.white;

        statLayoutUI.Clear();
        characterCard.Clear();
        SetVisible(characterCardCanvas, false);
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

        //restoreFocusCoroutine = null;
    }

    private void SetVisible(CanvasGroup canvasGroup, bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
    }

    #endregion

    #region Button Handlers

    public void OnButtonBackClicked()
    {
        /*
        if (stateMachine.Is(CharacterDetailState.SwappingMove))
        {
            stateMachine.Set(CharacterDetailState.Idle);
            return;
        }
        */
        RequestClose();
        UIEvents.RaiseBackFromWingDetailRequested();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnWingDetailOpenRequested    += HandleWingDetailOpenRequested;
        UIEvents.OnWingDetailRefreshRequested += HandleWingDetailRefreshRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnWingDetailOpenRequested    -= HandleWingDetailOpenRequested;
        UIEvents.OnWingDetailRefreshRequested -= HandleWingDetailRefreshRequested;
    }

    private void HandleWingDetailOpenRequested(Wing wing)
    {
        if (MenuManager.Instance.IsMenuOpen(this)) return;
        this.wing = wing;
        MenuManager.Instance.OpenMenu(this);
        SetDefaultSelectable(firstSelected);
    }

    private void HandleWingDetailRefreshRequested()
    {
        if (!MenuManager.Instance.IsMenuOpen(this)) return;
        Refresh();
    }
    
    #endregion
}
