using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Scout;

public class ScoutTier
{
    #region Components

    private ScoutTierComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private ScoutTierComponentCharacters charactersComponent;
    private ScoutTierComponentEntries entriesComponent;

    #endregion

    #region Initialize

    public ScoutTier(ScoutTierData data) 
    {
        attributesComponent = new ScoutTierComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Scout_Tier,
            data.ScoutTierId,
            new[] { LocalizationField.Name }
        );
        charactersComponent = new ScoutTierComponentCharacters(data);
        entriesComponent = new ScoutTierComponentEntries(data, this);
    }

    #endregion

    #region API

    // attributesComponent
    public string ScoutTierId => attributesComponent.ScoutTierId;
    public string UnlockFlag => attributesComponent.UnlockFlag;

    // localizationComponent
    public string ScoutTierName => localizationStringComponent.GetString(LocalizationField.Name);

    // charactersComponent
    public int CharacterCost => charactersComponent.CharacterCost;
    public int CharacterLevel => charactersComponent.CharacterLevel;
    public List<string> CharacterIds => charactersComponent.CharacterIds;

    // entriesComponent
    public List<ScoutEntry> GetEntries() => entriesComponent.GetEntries();

    #endregion
}
