using UnityEngine;
using Simulation.Enums.Duel;

public class BallComponentTravel : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private bool isTraveling;
    [SerializeField] private bool isTravelPaused;

    private float defaultBallY = 1f;
    private float travelSpeed = 3.5f;
    private float endThreshold = 0.01f;
    private float maxVelocity = 10f;
    private Vector3 travelVelocity;
    private Vector3 currentTarget;

    public bool IsTraveling => isTraveling;
    public bool IsTravelPaused => isTravelPaused;

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    private void Update()
    {
        if (!isTraveling || isTravelPaused) return;

        HandleTravel();
    }

    private void HandleTravel() 
    {
        Vector3 prevPos = transform.position;
        float step = travelSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, currentTarget, step);
        travelVelocity = (transform.position - prevPos) / Time.deltaTime;
        //LogManager.Trace($"[BallComponentTravel] Moving towards {currentTarget}. Current: {transform.position}, Step: {step}, Velocity: {travelVelocity.magnitude}", this);
        
        if (Vector3.Distance(transform.position, currentTarget) < endThreshold)
        {
            LogManager.Trace("[BallComponentTravel] [Update] Arrived at target. Ending travel...", this);
            EndTravel();
        }
    }

    public void StartTravel(Vector3 target, DuelCommand command)
    {
        if (isTraveling)
        {
            LogManager.Trace("[BallComponentTravel] [StartTravel] Already traveling, cannot start new travel.", this);
            return;
        }

        LogManager.Trace($"[BallComponentTravel] [StartTravel] Travel started to {target}.", this);
        currentTarget = target;
        ball.SetKinematic();

        var ballPostion = ball.transform.position;
        if(command == DuelCommand.Move)
            ballPostion.y = defaultBallY;
        ball.transform.position = ballPostion;

        isTraveling = true;
        isTravelPaused = false;
        BallEvents.RaiseTravelStart(target);
    }

    public void PauseTravel()
    {
        if (!isTraveling || isTravelPaused) {
            LogManager.Trace("[BallComponentTravel] [PauseTravel] Cannot pause: either not traveling or already paused.", this);   
            return;
        }

        isTravelPaused = true;
        LogManager.Trace("[BallComponentTravel] [PauseTravel] Travel paused.", this);
        BallEvents.RaiseTravelPause();
    }

    public void ResumeTravel()
    {
        if (!isTraveling || !isTravelPaused)
        {
            LogManager.Trace("[BallComponentTravel] [ResumeTravel] Cannot resume: either not traveling or not paused.", this);
            return;
        }
 
        LogManager.Trace("[BallComponentTravel] [ResumeTravel] Travel resumed.", this);
        isTravelPaused = false;
        BallEvents.RaiseTravelResume();
    }

    public void CancelTravel()
    {
        if (!isTraveling)
        {
            LogManager.Trace("[BallComponentTravel] [CancelTravel] Can not cancel because it is not traveling.", this);
            return;
        }

        LogManager.Trace($"[BallComponentTravel] [CancelTravel] Travel cancelled at position {transform.position}.", this);
        isTraveling = false;
        isTravelPaused = false;

        if (travelVelocity.magnitude > maxVelocity)
            travelVelocity = travelVelocity.normalized * maxVelocity;
        ball.SetDynamic(travelVelocity);

        ShootTriangleManager.Instance.Hide();

        DuelManager.Instance.CancelDuel();
        BallEvents.RaiseTravelCancel();
    }

    public void EndTravel()
    {
        isTraveling = false;

        //clamp travel velocity        
        if (travelVelocity.magnitude > maxVelocity)
            travelVelocity = travelVelocity.normalized * maxVelocity;
        ball.SetDynamic(travelVelocity);

        ShootTriangleManager.Instance.Hide();
    
        if(!DuelManager.Instance.IsResolved)
            DuelManager.Instance.CancelDuel();

        LogManager.Trace($"[BallComponentTravel] [EndTravel] Travel ended at {ball.transform.position} with velocity {travelVelocity}.", this);
        BallEvents.RaiseTravelEnd(currentTarget);
    }
}
