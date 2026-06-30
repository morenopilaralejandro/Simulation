using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Match;
using Aremoreno.Enums.Localization;

public class MatchChain
{
    #region Components

    private MatchChainComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;

    #endregion

    #region Initialize

    public MatchChain(MatchChainData data) 
    {
        attributesComponent = new MatchChainComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Match_Chain,
            data.MatchChainId,
            new[] { LocalizationField.Name}
        );
    }

    #endregion

    #region API

    // attributesComponent
    public string MatchChainId => attributesComponent.MatchChainId;

    // localizationComponent
    public string MatchChainName => localizationStringComponent.GetString(LocalizationField.Name);

    #endregion
}
