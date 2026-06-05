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
    private LocalizationComponentString localizationStringComponent;
    private WingComponentAppearance appearanceComponent;
    private WingComponentEvolution evolutionComponent;
    private WingComponentPersistence persistenceComponent;

    public Wing(WingData wingData, WingSaveData wingSaveData = null)
    {
        Initialize(wingData, wingSaveData);
    }

    public void Initialize(WingData wingData, WingSaveData wingSaveData = null)
    {
        attributesComponent = new WingComponentAttributes(wingData, this);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Wing,
            wingData.WingId,
            new [] { LocalizationField.Name, LocalizationField.Description }
        );

        //appearanceComponent
        appearanceComponent = new WingComponentAppearance(wingData, this, wingSaveData);

        //evolutionComponent
        evolutionComponent = new WingComponentEvolution(wingData, this, wingSaveData);

        //persistenceComponent
        persistenceComponent = new WingComponentPersistence(wingData, this);
    }

    #region API
    //attributesComponent
    public string WingId => attributesComponent.WingId;
    public Element Element => attributesComponent.Element;

    //localizationStringComponent
    public string WingName => localizationStringComponent.GetString(LocalizationField.Name);
    public string WingDescription => localizationStringComponent.GetString(LocalizationField.Description);

    //appearanceComponent

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

    //persistenceComponent

    #endregion
}
