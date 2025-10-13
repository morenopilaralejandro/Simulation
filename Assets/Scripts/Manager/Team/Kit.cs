using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Localization;

public class Kit
{
    [SerializeField] private string kitId;
    public string KitId => kitId;

    [SerializeField] private LocalizationComponentString localizationStringComponent;

    public void Initialize(KitData kitData)
    {
        kitId = kitData.KitId;

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Kit,
            kitData.KitId,
            new [] { LocalizationField.Name }
        );
    }

    public string KitName => localizationStringComponent.GetString(LocalizationField.Name);
}
