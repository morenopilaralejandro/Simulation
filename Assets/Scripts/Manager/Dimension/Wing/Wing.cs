using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;
using Aremoreno.Enums.Localization;

public class Wing
{
    private WingComponentAttributes attributesComponent;
    private ElementComponent elementComponent;
    private LocalizationComponentString localizationStringComponent;
    private WingComponentAppearance appearanceComponent;
    private WingComponentEvolution evolutionComponent;
    private WingComponentStats statComponent;
    private WingComponentRefinement refinementComponent;
    private WingComponentEquip equipComponent;

    private WingComponentPersistence persistenceComponent;

    public Wing(WingData wingData, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wingSaveData);
    }

    public void Initialize(WingData wingData, WingSaveData wingSaveData = null)
    {
        attributesComponent = new WingComponentAttributes(wingData, this, wingSaveData);
        elementComponent = new ElementComponent(wingData.Elements);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Wing,
            wingData.WingId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        appearanceComponent = new WingComponentAppearance(wingData, this, wingSaveData);
        evolutionComponent = new WingComponentEvolution(wingData, this, wingSaveData);
        statComponent = new WingComponentStats(wingData, this, wingSaveData);
        refinementComponent = new WingComponentRefinement(wingData, this, wingSaveData);
        equipComponent = new WingComponentEquip(wingData, this, wingSaveData);

        persistenceComponent = new WingComponentPersistence(wingData, this);
    }

    #region API
    //attributesComponent
    public string WingId => attributesComponent.WingId;
    public string WingGuid => attributesComponent.WingGuid;
    
    //elementComponent
    public Element[] Elements => elementComponent.Elements;
    public bool ContainsElement(Element element) => elementComponent.ContainsElement(element);
    public bool ContainsElement(Element[] elements) => elementComponent.ContainsElement(elements); 

    //localizationStringComponent
    public string WingName => localizationStringComponent.GetString(LocalizationField.Name);
    public string WingDescription => localizationStringComponent.GetString(LocalizationField.Description);

    //appearanceComponent
    public WingType WingType => appearanceComponent.WingType;
    public WingColorType WingColorTypeDefault => appearanceComponent.WingColorTypeDefault;
    public WingColorType WingColorTypeDye => appearanceComponent.WingColorTypeDye;
    public WingColorType WingColorType => appearanceComponent.WingColorType;
    public bool HasDye => appearanceComponent.HasDye;
    public void SetHasDye(bool boolValue) => appearanceComponent.SetHasDye(boolValue);
    public void SetWingColorTypeDye(WingColorType wingColorType) => appearanceComponent.SetWingColorTypeDye(wingColorType);

    //evolutionComponent
    public WingEvolution CurrentEvolution => evolutionComponent.CurrentEvolution;
    public WingGrowthType WingGrowthType => evolutionComponent.WingGrowthType;
    public WingGrowthRate WingGrowthRate => evolutionComponent.WingGrowthRate;
    public string WingEvolutionAddress => evolutionComponent.WingEvolutionAddress;
    public int TimesUsedTotal => evolutionComponent.TimesUsedTotal;
    public int TimesUsedCurrentEvolution => evolutionComponent.TimesUsedCurrentEvolution;
    public bool IsBefore => evolutionComponent.IsBefore;
    public int CurrentEvolutionIndex => evolutionComponent.CurrentEvolutionIndex;
    public WingEvolutionGrowthProfile WingEvolutionGrowthProfile => evolutionComponent.WingEvolutionGrowthProfile;
    public WingEvolutionPath WingEvolutionPath => evolutionComponent.WingEvolutionPath;
    public bool IsAtFinalEvolution => evolutionComponent.IsAtFinalEvolution;
    public void ProgressEvolution() => evolutionComponent.ProgressEvolution();
    public bool TryEvolve() => evolutionComponent.TryEvolve();
    public bool LimitBreak() => evolutionComponent.LimitBreak();
    public void ForceMaxEvolution() => evolutionComponent.ForceMaxEvolution();
    public int GetUsageThreshold() => evolutionComponent.GetUsageThreshold();
    public void ResetEvolution() => evolutionComponent.ResetEvolution();

    //statComponent
    public int GetIndividualStat(Stat stat) => statComponent.GetIndividualStat(stat);
    public int GetTrueStat(Stat stat) => statComponent.GetTrueStat(stat);
    public void SetIndividualStat(Stat stat, int amount) => statComponent.SetIndividualStat(stat, amount);
    public void UpdateStats() => statComponent.UpdateStats();

    //refinementComponent
    public WingRefinementRank CurrentRank => refinementComponent.CurrentRank;
    public int CurrentRankIndex => refinementComponent.CurrentRankIndex;
    public int CurrentRankProgress => refinementComponent.CurrentRankProgress;
    public int GetRefinementThreshold() => refinementComponent.GetRefinementThreshold();
    public bool AddDuplicate(Wing duplicate) => refinementComponent.AddDuplicate(duplicate);
    public float GetElementMatchBonus() => refinementComponent.GetElementMatchBonus();

    //equipComponent
    public Character EquippedCharacter => equipComponent.EquippedCharacter;
    public void SetEquippedCharacter(Character character) => equipComponent.SetEquippedCharacter(character);
    public bool IsEquipped() => equipComponent.IsEquipped();

    //persistenceComponent
    public WingSaveData Export() => persistenceComponent.Export();
    public void Import(WingSaveData saveData) => persistenceComponent.Import(saveData);

    #endregion
}
