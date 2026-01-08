using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Simulation.Enums.Localization;
using Simulation.Enums.Kit;

public class Kit
{
    #region Components
    private KitComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;
    private KitComponentColor colorComponent;
    #endregion

    #region Initialize
    public Kit(KitData kitData) 
    {
        Initialize(kitData);
    }

    public void Initialize(KitData kitData)
    {
        attributesComponent = new KitComponentAttributes(kitData, this);
        colorComponent = new KitComponentColor(kitData, this);

        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Kit,
            kitData.KitId,
            new [] { LocalizationField.Name }
        );
    }

    public void Deinitialize()
    {

    }
    #endregion

    #region API
    //attributeComponent
    public string KitId => attributesComponent.KitId;
    //localizationComponent
    public string KitName => localizationStringComponent.GetString(LocalizationField.Name);
    //colorComponent
    public KitColors GetColors(Variant variant, Role role) => colorComponent.GetColors(variant, role);
    #endregion
}
