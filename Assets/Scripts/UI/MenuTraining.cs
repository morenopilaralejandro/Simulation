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

public class MenuTraining : Menu
{
    #region Fields

    [Header("UI")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private TMP_Text textLevel;
    [SerializeField] private StatLayoutUI statLayoutUI;
    [SerializeField] private BarHPSP barHp;
    [SerializeField] private BarHPSP barSp;
    [SerializeField] private BarXP barXp;

    private Character character;

    #endregion

    #region Menu Overrides

    public override void SetInteractable(bool boolValue)
    {
        if (boolValue) Refresh();
        base.SetInteractable(boolValue);
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
        Populate();
    }

    #endregion

    #region Populate

    private void Populate()
    {
        if (character == null) return;

        characterCard.SetCharacter(character, character.Position);
        textLevel.text = $"{character.Level}";
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        statLayoutUI.Initialize(character);
        statLayoutUI.Populate();
    }

    #endregion

    #region Button Handlers

    public void OnButtonConfirmClicked()
    {
        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    public void OnButtonBackClicked()
    {
        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnMenuTrainingOpenRequested += HandleMenuTrainingOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnMenuTrainingOpenRequested -= HandleMenuTrainingOpenRequested;
    }

    private void HandleMenuTrainingOpenRequested(Character character)
    {
        this.character = character;
        MenuManager.Instance.OpenMenu(this);
    }

    #endregion
}
