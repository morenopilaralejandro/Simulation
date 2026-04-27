using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;

public class MenuCharacterPanelFilter : Menu
{
    #region Fields

    [Header("UI References")]
    [SerializeField] private TMP_InputField inputFieldName;

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

    private bool isOpen => menuManager != null && menuManager.IsMenuOpen(this);
    private bool isTop => menuManager != null && menuManager.IsMenuOnTop(this);
    private MenuManager menuManager;


    private CharacterFilterData filterData = new CharacterFilterData();
    private Dictionary<Toggle, Position> positionToggles;
    private Dictionary<Toggle, Element> elementToggles;
    private Dictionary<Toggle, Gender> genderToggles;

    #endregion

    #region Lifecycle

    private void Awake()
    {
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
            { toggleEarth,      Element.Water },
            { toggleElectric,   Element.Electric },
            { toggleWater,      Element.Water },
        };

        genderToggles = new Dictionary<Toggle, Gender>
        {
            { toggleMale,   Gender.M },
            { toggleFemale, Gender.F },
        };
    }

    private void Start() 
    {
        base.Hide();
        base.SetInteractable(false);

        menuManager = MenuManager.Instance;
    }

    private void OnDestroy()
    {

    }

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        base.SetInteractable(true);
        ApplyFilterDataToUI();
    }

    public override void Hide()
    {
        base.SetInteractable(false);
        base.Hide();
    }

    public void Close()
    {
        if (!isOpen) return;
        menuManager.CloseMenu();
    }

    #endregion

    #region Logic

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

    #endregion

    #region Helper

    #endregion

    #region Button Handle

    public void OnButtonApplyClicked() 
    {
        BuildFilterDataFromUI();
        UIEvents.RaiseCharacterFilterUpdated(filterData);
        Close();
    }

    public void OnButtonResetClicked() 
    {
        filterData.Reset();
        ResetUI();
    }

    public void OnInputValueChanged(string currentText)
    {

    }

    public void OnInputEndEdit()
    {
        // OnButtonConfirmClicked();
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        UIEvents.OnCharacterFilterRequested += HandleCharacterFilterRequested;

    }

    private void OnDisable()
    {
        UIEvents.OnCharacterFilterRequested -= HandleCharacterFilterRequested;
    }

    private void HandleCharacterFilterRequested() 
    {
        menuManager.OpenMenu(this);
    }

    #endregion
}
