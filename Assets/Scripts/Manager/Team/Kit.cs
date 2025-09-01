using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Localization;

public class Kit
{
    [SerializeField] private string kitId;
    public string KitId => kitId;

    [SerializeField] private ComponentLocalization localizationComponent;

    public void Initialize(KitData kitData)
    {
        kitId = kitData.KitId;

        localizationComponent = new ComponentLocalization(
            LocalizationEntity.Kit,
            kitData.KitId,
            new [] { LocalizationField.Name }
        );
    }

    public string GetKitName() => localizationComponent.GetString(LocalizationField.Name);
}
