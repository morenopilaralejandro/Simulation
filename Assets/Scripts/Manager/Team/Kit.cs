using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class Kit
{
    [SerializeField] private string kitId;
    public string KitId => kitId;

    [SerializeField] private LocalizedString localizedName;
    public LocalizedString LocalizedName => localizedName;
  
    [SerializeField] private bool isRomazed = false;
    [SerializeField] private string stringTableNameLocalized = "KitNamesLocalized";
    [SerializeField] private string stringTableNameRomanized = "KitNamesRomanized";

    public void Initialize(KitData kitData)
    {
        kitId = kitData.KitId;

        SetName();
    }

    private void SetName()
    {
        localizedName = new LocalizedString(
            isRomazed ? stringTableNameRomanized : stringTableNameLocalized,
            kitId
        );
    }
}
