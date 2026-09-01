using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Localization;
using Aremoreno.Enums.World;

public class FastTravelPoint
{
    #region Components

    private FastTravelPointComponentAttributes attributesComponent;
    private LocalizationComponentString localizationStringComponent;

    #endregion

    #region Initialize

    public FastTravelPoint(FastTravelPointData data) 
    {
        attributesComponent = new FastTravelPointComponentAttributes(data);
        localizationStringComponent = new LocalizationComponentString(
            LocalizationEntity.Zone,
            data.ZoneId,
            new [] { LocalizationField.Name }
        );
    }

    #endregion

    #region API

    // attributesComponent
    public string FastTravelPointId => attributesComponent.FastTravelPointId;
    public string FlagId => attributesComponent.FlagId;
    public string ZoneId => attributesComponent.ZoneId;
    public string SpawnPointId => attributesComponent.SpawnPointId;

    // localizationComponent
    public string ZoneName => localizationStringComponent.GetString(LocalizationField.Name);

    #endregion
}
