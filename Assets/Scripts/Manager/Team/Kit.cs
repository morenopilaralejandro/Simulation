using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Localization;

public class Kit
{
    [SerializeField] private string kitId;
    public string KitId => kitId;

    [SerializeField] private ComponentLocalizationString stringLocalizationComponent;

    public void Initialize(KitData kitData)
    {
        kitId = kitData.KitId;

        stringLocalizationComponent = new ComponentLocalizationString(
            LocalizationEntity.Kit,
            kitData.KitId,
            new [] { LocalizationField.Name }
        );
    }

    public string KitName => stringLocalizationComponent.GetString(LocalizationField.Name);
}
