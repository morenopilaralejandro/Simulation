using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Emblem
{
    #region Components
    private EmblemComponentAttributes attributesComponent;
    #endregion

    #region Initialize
    public Emblem(EmblemData data) 
    {
        Initialize(data);
    }

    public void Initialize(EmblemData data)
    {
        attributesComponent = new EmblemComponentAttributes(data, this);
    }

    public void Deinitialize()
    {

    }
    #endregion

    #region API
    //attributeComponent
    public string EmblemId => attributesComponent.EmblemId;
    public string EmblemAddress => attributesComponent.EmblemAddress;
    #endregion
}
