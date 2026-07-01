using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.Match;

public class MatchChainNodeComponentText
{
    private MatchChainNodeText matchChainNodeText;
    private LocalizationComponentString localizationStringComponent;

    public MatchChainNodeComponentText(MatchChainNodeDataText data, MatchChainNodeText obj, MatchChainNodeSaveData saveData = null)
    {
        this.matchChainNodeText = obj;
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Dialog,
            data.TextLocalizationKey,
            new[] { LocalizationField.Text}
        );
    }

    // localizationComponent
    public string MatchChainText => localizationStringComponent.GetString(LocalizationField.Text);
}
