using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Simulation.Enums.Localization;

public class Kit
{
    [SerializeField] private string kitId;
    public string KitId => kitId;

    [SerializeField] private LocalizedString localizedName;
    public LocalizedString LocalizedName => localizedName;

    public void Initialize(KitData kitData)
    {
        kitId = kitData.KitId;

        SetName();
    }

    private void SetName()
    {
        localizedName = new LocalizedString(
            LocalizationManager.Instance.GetTableReference(LocalizationEntity.Kit, LocalizationField.Name),
            kitId
        );
    }
}
