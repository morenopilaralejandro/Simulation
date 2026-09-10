using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.Item;

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

    [Header("UI - Kick")]
    [SerializeField] private TMP_Text textValueKick;
    [SerializeField] private TMP_Text textValueTrainedKick;

    [Header("UI - Body")]
    [SerializeField] private TMP_Text textValueBody;
    [SerializeField] private TMP_Text textValueTrainedBody;

    [Header("UI - Control")]
    [SerializeField] private TMP_Text textValueControl;
    [SerializeField] private TMP_Text textValueTrainedControl;

    [Header("UI - Guard")]
    [SerializeField] private TMP_Text textValueGuard;
    [SerializeField] private TMP_Text textValueTrainedGuard;

    [Header("UI - Speed")]
    [SerializeField] private TMP_Text textValueSpeed;
    [SerializeField] private TMP_Text textValueTrainedSpeed;

    [Header("UI - Stamina")]
    [SerializeField] private TMP_Text textValueStamina;
    [SerializeField] private TMP_Text textValueTrainedStamina;

    [Header("UI - Courage")]
    [SerializeField] private TMP_Text textValueCourage;
    [SerializeField] private TMP_Text textValueTrainedCourage;

    [Header("UI - Training")]
    [SerializeField] private TMP_Text textFreedom;
    [SerializeField] private TMP_Text textCost;

    [Header("Buttons - Kick")]
    [SerializeField] private Button buttonAddKick;
    [SerializeField] private Button buttonSubtractKick;

    [Header("Buttons - Body")]
    [SerializeField] private Button buttonAddBody;
    [SerializeField] private Button buttonSubtractBody;

    [Header("Buttons - Control")]
    [SerializeField] private Button buttonAddControl;
    [SerializeField] private Button buttonSubtractControl;

    [Header("Buttons - Guard")]
    [SerializeField] private Button buttonAddGuard;
    [SerializeField] private Button buttonSubtractGuard;

    [Header("Buttons - Speed")]
    [SerializeField] private Button buttonAddSpeed;
    [SerializeField] private Button buttonSubtractSpeed;

    [Header("Buttons - Stamina")]
    [SerializeField] private Button buttonAddStamina;
    [SerializeField] private Button buttonSubtractStamina;

    [Header("Buttons - Courage")]
    [SerializeField] private Button buttonAddCourage;
    [SerializeField] private Button buttonSubtractCourage;

    [Header("Buttons - Confirm")]
    [SerializeField] private Button buttonConfirm;

    private Character character;

    // Values when the menu was opened.
    private readonly Dictionary<Stat, int> cachedStats = new();
    private readonly Dictionary<Stat, int> cachedTraining = new();

    // Temporary values while editing.
    private readonly Dictionary<Stat, int> pendingTraining = new();

    private int cachedFreedom;
    private int pendingFreedom;

    private int trainingPointCost;
    private int maxTrainingPerStat;

    #endregion

    #region Menu Overrides

    public override void SetInteractable(bool boolValue)
    {
        if (boolValue)
            Refresh();

        base.SetInteractable(boolValue);
    }

    protected override void OnGainedInput()
    {
        InputManager.Instance.SubscribeDown(
            CustomAction.Navigation_Back,
            OnButtonBackClicked);
    }

    protected override void OnLostInput()
    {
        InputManager.Instance.UnsubscribeDown(
            CustomAction.Navigation_Back,
            OnButtonBackClicked);
    }

    #endregion

    #region Refresh

    public void Refresh()
    {
        Populate();
        RefreshTrainingUI();
    }

    private void Populate()
    {
        if (character == null)
            return;

        characterCard.SetCharacter(character, character.Position);

        textLevel.text = character.Level.ToString();

        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);

        statLayoutUI.Initialize(character);
        statLayoutUI.Populate();
    }

    #endregion

    #region Cache

    private void CacheTrainingValues()
    {
        cachedStats.Clear();
        cachedTraining.Clear();
        pendingTraining.Clear();

        trainingPointCost = character.TrainingPointCost;
        maxTrainingPerStat = character.MaxTrainingPerStat;

        Stat[] trainingStats =
        {
            Stat.Kick,
            Stat.Body,
            Stat.Control,
            Stat.Guard,
            Stat.Speed,
            Stat.Stamina,
            Stat.Courage
        };

        foreach (Stat stat in trainingStats)
        {
            // Cache the actual stat value.
            cachedStats[stat] = character.GetTrueStat(stat);

            // Cache the current training value.
            int training = character.GetTrainedStat(stat);

            cachedTraining[stat] = training;

            // Start pending value at the current value.
            pendingTraining[stat] = training;
        }

        cachedFreedom = character.TrueFreedom;
        pendingFreedom = cachedFreedom;
    }

    #endregion

    #region Training

    private void ModifyPendingTraining(Stat stat, int amount)
    {
        int currentTraining = pendingTraining[stat];

        if (amount > 0)
        {
            // No Freedom available.
            if (pendingFreedom <= 0) return;
            // Stat reached maximum training.
            if (currentTraining >= maxTrainingPerStat) return;

            pendingTraining[stat]++;
            pendingFreedom--;
        }
        else if (amount < 0)
        {
            // Cannot go below zero training.
            if (currentTraining <= 0) return;

            pendingTraining[stat]--;
            pendingFreedom++;
        }

        RefreshTrainingUI();
    }

    #endregion

    #region Cost

    private int CalculateTrainingCost()
    {
        int totalCost = 0;
        foreach (KeyValuePair<Stat, int> pair in pendingTraining)
        {
            Stat stat = pair.Key;

            int originalTraining = cachedTraining[stat];
            int currentTraining = pair.Value;

            // Only training added above the original amount costs Gold.
            int addedTraining = Mathf.Max(currentTraining - originalTraining, 0);
            totalCost += addedTraining * trainingPointCost;
        }
        return totalCost;
    }

    private bool CanAffordTraining()
    {
        int cost = CalculateTrainingCost();
        return ItemManager.Instance.CanAfford(CurrencyType.Gold, cost);
    }

    #endregion

    #region UI

    private void RefreshTrainingUI()
    {
        if (character == null) return;

        UpdateStatUI(Stat.Kick, textValueKick, textValueTrainedKick);
        UpdateStatUI(Stat.Body, textValueBody, textValueTrainedBody);
        UpdateStatUI(Stat.Control, textValueControl, textValueTrainedControl);
        UpdateStatUI(Stat.Guard, textValueGuard, textValueTrainedGuard);
        UpdateStatUI(Stat.Speed, textValueSpeed, textValueTrainedSpeed);
        UpdateStatUI(Stat.Stamina, textValueStamina, textValueTrainedStamina);
        UpdateStatUI(Stat.Courage, textValueCourage, textValueTrainedCourage);

        textFreedom.text = pendingFreedom.ToString();

        int cost = CalculateTrainingCost();
        textCost.text = cost.ToString();
        textCost.color = ItemManager.Instance.CanAfford(CurrencyType.Gold, cost) ? Color.white : Color.red;
        //RefreshButtonsAll();
        buttonConfirm.interactable = CanAffordTraining();
    }

    private void UpdateStatUI(
        Stat stat,
        TMP_Text textValue,
        TMP_Text textValueTrained)
    {
        int originalStat = cachedStats[stat];
        int originalTraining = cachedTraining[stat];
        int currentTraining = pendingTraining[stat];

        // Training difference from when the menu was opened.
        int trainingDifference =
            currentTraining - originalTraining;

        // Training directly adds to the stat.
        int previewStat =
            originalStat + trainingDifference;

        textValue.text = previewStat.ToString();
        textValueTrained.text = $"({currentTraining})";
    }

    #endregion

    #region Buttons

    private void RefreshButtonsAll()
    {
        RefreshButtons(Stat.Kick, buttonAddKick, buttonSubtractKick);
        RefreshButtons(Stat.Body, buttonAddBody, buttonSubtractBody);
        RefreshButtons(Stat.Control, buttonAddControl, buttonSubtractControl);
        RefreshButtons(Stat.Guard, buttonAddGuard, buttonSubtractGuard);
        RefreshButtons(Stat.Speed, buttonAddSpeed, buttonSubtractSpeed);
        RefreshButtons(Stat.Stamina, buttonAddStamina, buttonSubtractStamina);
        RefreshButtons(Stat.Courage, buttonAddCourage, buttonSubtractCourage);
        buttonConfirm.interactable = CanAffordTraining();
    }

    private void RefreshButtons(Stat stat, Button addButton, Button subtractButton)
    {
        int training = pendingTraining[stat];
        addButton.interactable = pendingFreedom > 0 && training < maxTrainingPerStat;
        subtractButton.interactable = training > 0;
    }

    #endregion

    #region Button Handlers

    public void OnButtonAddKickClicked()
    {
        ModifyPendingTraining(Stat.Kick, 1);
    }

    public void OnButtonSubtractKickClicked()
    {
        ModifyPendingTraining(Stat.Kick, -1);
    }

    public void OnButtonAddBodyClicked()
    {
        ModifyPendingTraining(Stat.Body, 1);
    }

    public void OnButtonSubtractBodyClicked()
    {
        ModifyPendingTraining(Stat.Body, -1);
    }

    public void OnButtonAddControlClicked()
    {
        ModifyPendingTraining(Stat.Control, 1);
    }

    public void OnButtonSubtractControlClicked()
    {
        ModifyPendingTraining(Stat.Control, -1);
    }

    public void OnButtonAddGuardClicked()
    {
        ModifyPendingTraining(Stat.Guard, 1);
    }

    public void OnButtonSubtractGuardClicked()
    {
        ModifyPendingTraining(Stat.Guard, -1);
    }

    public void OnButtonAddSpeedClicked()
    {
        ModifyPendingTraining(Stat.Speed, 1);
    }

    public void OnButtonSubtractSpeedClicked()
    {
        ModifyPendingTraining(Stat.Speed, -1);
    }

    public void OnButtonAddStaminaClicked()
    {
        ModifyPendingTraining(Stat.Stamina, 1);
    }

    public void OnButtonSubtractStaminaClicked()
    {
        ModifyPendingTraining(Stat.Stamina, -1);
    }

    public void OnButtonAddCourageClicked()
    {
        ModifyPendingTraining(Stat.Courage, 1);
    }

    public void OnButtonSubtractCourageClicked()
    {
        ModifyPendingTraining(Stat.Courage, -1);
    }

    #endregion

    #region Confirm / Back

    public void OnButtonConfirmClicked()
    {
        int cost = CalculateTrainingCost();

        if (cost > 0)
        {
            if (!ItemManager.Instance.CanAfford(CurrencyType.Gold, cost))
            {
                RefreshTrainingUI();
                return;
            }

            if (!ItemManager.Instance.Spend(CurrencyType.Gold, cost))
            {
                RefreshTrainingUI();
                return;
            }
        }

        ApplyTraining();

        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    public void OnButtonBackClicked()
    {
        // Nothing was changed on the character.
        // Simply discard the pending values.

        RequestClose();
        UIEvents.RaiseCharacterDetailRefreshRequested();
    }

    private void ApplyTraining()
    {
        foreach (KeyValuePair<Stat, int> pair in pendingTraining)
        {
            Stat stat = pair.Key;

            int originalTraining = cachedTraining[stat];
            int currentTraining = pair.Value;

            int delta = currentTraining - originalTraining;

            if (delta == 0)
                continue;

            character.ApplyTrainingDelta(stat, delta);
        }

        // Recalculate the character's actual stats only after
        // all training changes have been applied.
        character.UpdateStats();
    }

    #endregion

    #region Events

    protected override void OnEnable()
    {
        base.OnEnable();

        UIEvents.OnMenuTrainingOpenRequested +=
            HandleMenuTrainingOpenRequested;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        UIEvents.OnMenuTrainingOpenRequested -=
            HandleMenuTrainingOpenRequested;
    }

    private void HandleMenuTrainingOpenRequested(Character character)
    {
        this.character = character;

        // Cache everything before opening the menu.
        CacheTrainingValues();

        MenuManager.Instance.OpenMenu(this);
    }

    #endregion
}
