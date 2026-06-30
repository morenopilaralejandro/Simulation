using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Match;

public class MatchChainNodeMatch : MatchChainNode
{
    #region Components

    private MatchChainNodeComponentMatch matchComponent;

    #endregion

    #region Initialize

    public MatchChainNodeMatch(MatchChainData data) : base(data)
    {
        matchComponent = new MatchChainNodeComponentMatch(data, this);
        SetIconAddress();
    }

    #endregion

    #region API

    // matchComponent
    // public EquipmentType EquipmentType => matchComponent.EquipmentType;

    #endregion
}
