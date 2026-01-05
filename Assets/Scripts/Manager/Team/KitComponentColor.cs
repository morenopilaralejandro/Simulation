using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Kit;

public class KitComponentColor
{
    private Kit kit;
    private Dictionary<(Variant, Role), KitColors> colors;

    public KitComponentColor(KitData kitData, Kit kit)
    {
        Initialize(kitData, kit);
    }

    public void Initialize(KitData kitData, Kit kit)
    {
        this.kit = kit;

        colors = new Dictionary<(Variant, Role), KitColors>
            {
                {
                    (Variant.Home, Role.Field),
                    new KitColors
                    {
                        Base = kitData.BaseColorHomeField,
                        Detail = kitData.DetailColorHomeField,
                        Shocks = kitData.ShockColorHomeField
                    }
                },
                {
                    (Variant.Home, Role.Keeper),
                    new KitColors
                    {
                        Base = kitData.BaseColorHomeKeeper,
                        Detail = kitData.DetailColorHomeKeeper,
                        Shocks = kitData.ShockColorHomeKeeper
                    }
                },
                {
                    (Variant.Away, Role.Field),
                    new KitColors
                    {
                        Base = kitData.BaseColorAwayField,
                        Detail = kitData.DetailColorAwayField,
                        Shocks = kitData.ShockColorAwayField
                    }
                },
                {
                    (Variant.Away, Role.Keeper),
                    new KitColors
                    {
                        Base = kitData.BaseColorAwayKeeper,
                        Detail = kitData.DetailColorAwayKeeper,
                        Shocks = kitData.ShockColorAwayKeeper
                    }
                }
            };
    }

    public KitColors GetColors(Variant variant, Role role)
    {
        return colors[(variant, role)];
    }
}
