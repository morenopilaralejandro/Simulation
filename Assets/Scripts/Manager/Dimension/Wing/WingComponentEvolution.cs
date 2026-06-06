using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;
using Aremoreno.Enums.Localization;

public class WingComponentEvolution
{
    private Wing wing;
    private WingEvolutionGrowthProfile growthProfile;
    private WingEvolutionPath path;
    
    public WingEvolution CurrentEvolution { get; private set; }
    public WingGrowthType WingGrowthType { get; private set; }
    public WingGrowthRate WingGrowthRate { get; private set; }
    public string WingEvolutionAddress { get; private set; }

    public int TimesUsedTotal { get; private set; }
    public int TimesUsedCurrentEvolution { get; private set; }
    public bool IsBefore { get; private set; }
    public int CurrentEvolutionIndex => path.GetEvolutionIndex(CurrentEvolution);
    public WingEvolutionGrowthProfile WingEvolutionGrowthProfile => growthProfile;
    public WingEvolutionPath WingEvolutionPath => path;

    private static readonly string EmptyAddress = string.Empty;

    public WingComponentEvolution(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wing, wingSaveData);
    }

    public void Initialize(WingData wingData, Wing wing, WingSaveData wingSaveData = null)
    {
        this.wing = wing;
        this.WingGrowthType = wingData.WingGrowthType;
        this.WingGrowthRate = wingData.WingGrowthRate;
        this.growthProfile = WingDatabase.Instance.GetWingEvolutionGrowthProfile(wingData);
        this.path = WingDatabase.Instance.GetWingEvolutionPath(wingData);

        if (wingSaveData != null) 
        {
            this.CurrentEvolution = wingSaveData.CurrentEvolution;
            this.TimesUsedTotal = wingSaveData.TimesUsedTotal;
            this.TimesUsedCurrentEvolution = wingSaveData.TimesUsedCurrentEvolution;
        } else 
        {
            this.CurrentEvolution = WingEvolution.None;
            this.TimesUsedTotal = 0;
            this.TimesUsedCurrentEvolution = 0;
        }

    }

    public bool IsAtFinalEvolution => !path.TryGetNextEvolution(this.CurrentEvolution, out _);

    public void ProgressEvolution()
    {
        this.TimesUsedTotal++;
        this.TimesUsedCurrentEvolution++;

        // Stop at last evolution stage
        if (IsAtFinalEvolution) return;

        int threshold = growthProfile.GetUsageThreshold(CurrentEvolution);

        if (this.TimesUsedCurrentEvolution >= threshold)
        {
            if (TryEvolve()) this.TimesUsedCurrentEvolution = 0;
        }
    }

    public bool TryEvolve()
    {
        if (path.TryGetNextEvolution(this.CurrentEvolution, out WingEvolution next))
        {
            this.CurrentEvolution = next;
            UpdateLocalization();
            wing.UpdateStats();
            LogManager.Trace($"[WingComponentEvolution] [{wing.WingId}] evolved to {this.CurrentEvolution}");
            return true;
        }

        return false;
    }

    public bool LimitBreak()
    {
        if (path.TryGetNextEvolution(this.CurrentEvolution, out WingEvolution next))
        {
            if (path.TryGetNextEvolution(next, out WingEvolution _)) return false; // Not at penultimate stage yet

            this.CurrentEvolution = next;
            this.TimesUsedCurrentEvolution = 0;
            UpdateLocalization();
            wing.UpdateStats();
            LogManager.Trace($"[WingComponentEvolution] [{wing.WingId}] performed LIMIT BREAK -> {this.CurrentEvolution}");
            return true;
        }

        return false;
    }

    public void ForceMaxEvolution()
    {
        this.CurrentEvolution = path.GetLastEvolution();
        this.TimesUsedCurrentEvolution = 0;
        UpdateLocalization();
            wing.UpdateStats();
        LogManager.Trace($"WingComponentEvolution] [{wing.WingId}] was force-evolved to MAX: {this.CurrentEvolution}");
    }

    //public int GetExtraPower() => growthProfile.GetBonus(this.CurrentEvolution);

    public int GetUsageThreshold() => growthProfile.GetUsageThreshold(this.CurrentEvolution);

    public void ResetEvolution()
    {
        this.CurrentEvolution = WingEvolution.None;
        this.TimesUsedTotal = TimesUsedCurrentEvolution = 0;
        UpdateLocalization();
        wing.UpdateStats();
    }

    private void UpdateLocalization()
    {
        IsBefore = false;
        if (CurrentEvolution == WingEvolution.None)
        {
            WingEvolutionAddress = EmptyAddress;
            return;
        }

        WingEvolution evolution = CurrentEvolution;
        var locale = LocalizationSettings.SelectedLocale;
        var settings = SettingsManager.Instance.CurrentSettings;
        string type = WingGrowthType.ToString().ToLowerInvariant();
        string evolutionId = evolution.ToString().ToLowerInvariant();
        string localizationCode = locale.Identifier.Code.ToLowerInvariant();
        string localizationStyle = settings.CurrentLocalizationStyle.ToString().ToLowerInvariant();
        int numCode = path.GetEvolutionIndex(evolution);

        string id = string.Concat(
            type, "-",
            numCode.ToString(), "-",
            evolutionId
        );

        WingEvolutionAddress = AddressableLoader.GetWingEvolutionAddress(
            id,
            localizationCode,
            localizationStyle
        );

        /*
        if (localizationCode == "ja" || localizationStyle == "romanized")
        {
            if (CurrentEvolution == MoveEvolution.Yari || CurrentEvolution == MoveEvolution.Mori || CurrentEvolution == MoveEvolution.Shi)
                IsBefore = true;
        }
        */
        IsBefore = false;
    }

}
