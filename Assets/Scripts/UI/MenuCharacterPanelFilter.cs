using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using UnityEngine.EventSystems;

public class MenuCharacterPanelFilter : Menu
{
    [Header("UI References")]
    [SerializeField] private TMPInputFieldNoAutoActivate inputFieldName;
    [SerializeField] private Button buttonApply;

    [Header("Position Toggles")]
    [SerializeField] private Toggle toggleFW;
    [SerializeField] private Toggle toggleMF;
    [SerializeField] private Toggle toggleDF;
    [SerializeField] private Toggle toggleGK;

    [Header("Element Toggles")]
    [SerializeField] private Toggle toggleFire;
    [SerializeField] private Toggle toggleIce;
    [SerializeField] private Toggle toggleHoly;
    [SerializeField] private Toggle toggleEvil;
    [SerializeField] private Toggle toggleAir;
    [SerializeField] private Toggle toggleForest;
    [SerializeField] private Toggle toggleEarth;
    [SerializeField] private Toggle toggleElectric;
    [SerializeField] private Toggle toggleWater;

    [Header("Gender Toggles")]
    [SerializeField] private Toggle toggleMale;
    [SerializeField] private Toggle toggleFemale;

    private CharacterFilterData filterData = new CharacterFilterData();
    private Dictionary<Toggle, Position> positionToggles;
    private Dictionary<Toggle, Element> elementToggles;
    private Dictionary<Toggle, Gender> genderToggles;


    protected override void Awake()
    {
        base.Awake();

        positionToggles = new Dictionary<Toggle, Position>
        {
            { toggleFW, Position.FW },
            { toggleMF, Position.MF },
            { toggleDF, Position.DF },
            { toggleGK, Position.GK },
        };

        elementToggles = new Dictionary<Toggle, Element>
        {
            { toggleFire,       Element.Fire },
            { toggleIce,        Element.Ice },
            { toggleHoly,       Element.Holy },
            { toggleEvil,       Element.Evil },
            { toggleAir,        Element.Air },
            { toggleForest,     Element.Forest },
            { toggleEarth,      Element.Earth },
            { toggleElectric,   Element.Electric },
            { toggleWater,      Element.Water },
        };

        genderToggles = new Dictionary<Toggle, Gender>
        {
            { toggleMale,   Gender.M },
            { toggleFemale, Gender.F },
        };
    }

    protected override void OnGainedInput()
        => InputManager.Instance.SubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    protected override void OnLostInput()
        => InputManager.Instance.UnsubscribeDown(CustomAction.Navigation_Back, OnButtonBackClicked);

    public void OnButtonApplyClicked() 
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_tap");
        BuildFilterDataFromUI();
        UIEvents.RaiseCharacterFilterUpdated(filterData);
        RequestClose();
    }

    public void OnButtonResetClicked() 
    {
        AudioManager.Instance.PlaySfxUI("sfx-menu_cancel");
        filterData.ResetUI();
        ResetUI();
    }

    public void OnInputValueChanged(string currentText)
    {

    }

    public void OnInputEndEdit()
    {
        if (EventSystem.current.alreadySelecting) return;
        AudioManager.Instance.PlaySfxUI("sfx-menu_confirm");
        EventSystem.current.SetSelectedGameObject(buttonApply.gameObject);
    }

    public void OnButtonBackClicked() 
    {
        RequestClose();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UIEvents.OnCharacterFilterRequested += HandleCharacterFilterRequested;
        UIEvents.OnCharacterFilterResetRequested += HandleCharacterFilterResetRequested;
        UIEvents.OnCharacterFilterUpdated += HandleCharacterFilterUpdated;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UIEvents.OnCharacterFilterRequested -= HandleCharacterFilterRequested;
        UIEvents.OnCharacterFilterResetRequested -= HandleCharacterFilterResetRequested;
        UIEvents.OnCharacterFilterUpdated -= HandleCharacterFilterUpdated;
    }

    private void HandleCharacterFilterRequested() 
    {
        MenuManager.Instance.OpenMenu(this);
    }

    private void HandleCharacterFilterResetRequested() 
    {
        filterData.Reset();
        ResetUI();
    }

    private void HandleCharacterFilterUpdated(CharacterFilterData characterFilterData) 
    {
        filterData = characterFilterData;
    }

    /// <summary>
    /// Reads all UI controls and writes into filterData.
    /// </summary>
    private void BuildFilterDataFromUI()
    {
        // Name
        filterData.Name = string.IsNullOrWhiteSpace(inputFieldName.text)
            ? null
            : inputFieldName.text.Trim();

        // Positions
        filterData.Positions.Clear();
        foreach (var kvp in positionToggles)
        {
            if (kvp.Key.isOn)
                filterData.Positions.Add(kvp.Value);
        }

        // Elements
        filterData.Elements.Clear();
        foreach (var kvp in elementToggles)
        {
            if (kvp.Key.isOn)
                filterData.Elements.Add(kvp.Value);
        }

        // Genders
        filterData.Genders.Clear();
        foreach (var kvp in genderToggles)
        {
            if (kvp.Key.isOn)
                filterData.Genders.Add(kvp.Value);
        }
    }

    /// <summary>
    /// Pushes current filterData state back into UI (used when reopening the panel).
    /// </summary>
    private void ApplyFilterDataToUI()
    {
        inputFieldName.SetTextWithoutNotify(filterData.Name ?? string.Empty);

        foreach (var kvp in positionToggles)
            kvp.Key.SetIsOnWithoutNotify(filterData.Positions.Contains(kvp.Value));

        foreach (var kvp in elementToggles)
            kvp.Key.SetIsOnWithoutNotify(filterData.Elements.Contains(kvp.Value));

        foreach (var kvp in genderToggles)
            kvp.Key.SetIsOnWithoutNotify(filterData.Genders.Contains(kvp.Value));
    }

    private void ResetUI()
    {
        inputFieldName.SetTextWithoutNotify(string.Empty);

        foreach (var kvp in positionToggles)
            kvp.Key.SetIsOnWithoutNotify(false);

        foreach (var kvp in elementToggles)
            kvp.Key.SetIsOnWithoutNotify(false);

        foreach (var kvp in genderToggles)
            kvp.Key.SetIsOnWithoutNotify(false);
    }

}
