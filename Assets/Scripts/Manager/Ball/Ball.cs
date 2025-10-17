using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Components
    [SerializeField] private BallComponentAttributes attributesComponent;
    [SerializeField] private BallComponentAppearance appearanceComponent;
    #endregion

    #region Initialize
    public void Initialize(BallData ballData) 
    {
        attributesComponent.Initialize(ballData, this);
        appearanceComponent.Initialize(ballData, this);
    }
    #endregion

    #region API
    //attributesComponent
    public string BallId => attributesComponent.BallId;
    #endregion

}
