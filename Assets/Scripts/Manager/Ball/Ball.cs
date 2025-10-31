using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Components
    [SerializeField] private BallComponentAttributes attributesComponent;
    [SerializeField] private BallComponentAppearance appearanceComponent;
    [SerializeField] private BallComponentKinematic kinematicComponent;
    [SerializeField] private BallComponentCollider colliderComponent;
    [SerializeField] private BallComponentKeep keepComponent;
    [SerializeField] private BallComponentKick kickComponent;



    [SerializeField] private BallComponentTravel travelComponent;
    #endregion

    #region Initialize
    public void Initialize(BallData ballData) 
    {
        attributesComponent.Initialize(ballData, this);
        appearanceComponent.Initialize(ballData, this);
        kinematicComponent.Initialize(ballData, this);
        colliderComponent.Initialize(ballData, this);
        keepComponent.Initialize(ballData, this);
        kickComponent.Initialize(ballData, this);


        travelComponent.Initialize(ballData, this);
    }
    #endregion

    #region API
    //attributesComponent
    public string BallId => attributesComponent.BallId;
    //appearanceComponent
    //kinematicComponent
    public bool IsKinematic => kinematicComponent.IsKinematic;
    public void SetKinematic() => kinematicComponent.SetKinematic();
    public void SetDynamic() => kinematicComponent.SetDynamic();
    public void SetDynamic(Vector3 velocity) => kinematicComponent.SetDynamic(velocity);
    public void ToggleKinematic() => kinematicComponent.ToggleKinematic();
    public Vector3 GetVelocity() => kinematicComponent.GetVelocity();
    //kickComponent
    public void KickBallTo(Vector3 targetPos) => kickComponent.KickBallTo(targetPos);


    //travelComponent
    public bool IsTraveling => travelComponent.IsTraveling;
    public bool IsTravelPaused => travelComponent.IsTravelPaused;
    public void StartTravel(Vector3 target) => travelComponent.StartTravel(target);
    public void PauseTravel() => travelComponent.PauseTravel();
    public void ResumeTravel() => travelComponent.ResumeTravel();
    public void CancelTravel() => travelComponent.CancelTravel();
    public void EndTravel() => travelComponent.EndTravel();

    //misc
    public bool IsFree() => PossessionManager.Instance.CurrentCharacter == null;
    #endregion

}
