using UnityEngine;
using Simulation.Enums.Duel;
using Simulation.Enums.Character;

public class BallComponentTravelParticle : MonoBehaviour
{
    private Ball ball;

    [SerializeField] private ParticleSystem particleTravel;

    private Vector3 travelDirection;
    private Element currentElement;
    private bool isPlaying;

    private void OnEnable()
    {
        BallEvents.OnTravelStart += HandleTravelStart;
        BallEvents.OnTravelCancel += HandleTravelCancel;
        BallEvents.OnTravelEnd += HandleTravelEnd;
    }

    private void OnDisable()
    {
        BallEvents.OnTravelStart -= HandleTravelStart;
        BallEvents.OnTravelCancel -= HandleTravelCancel;
        BallEvents.OnTravelEnd -= HandleTravelEnd;
    }

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
        particleTravel.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void HandleTravelStart(Vector3 startPosition)
    {
        travelDirection = (startPosition - transform.position).normalized;
    }

    private void HandleTravelCancel()
    {
        StopParticle();
    }

    private void HandleTravelEnd(Vector3 endPosition)
    {
        StopParticle();
    }

    public void TryPlayParticle(Move move)
    {
        if (move == null) return;

        currentElement = move.Element;
        StartParticle();
    }

    private void StartParticle()
    {
        if (isPlaying) return;

        SetElementColor(currentElement);
        particleTravel.transform.forward = -travelDirection;
        particleTravel.Play(true);

        AudioManager.Instance.PlaySfxLoop("sfx-ball_energy");
        isPlaying = true;
    }

    private void StopParticle()
    {
        if (!isPlaying) return;

        particleTravel.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        AudioManager.Instance.StopSfxLoop("sfx-ball_energy");
        isPlaying = false;
    }

    public void SetElementColor(Element element)
    {
        if (!isPlaying) return;
        Color newColor = ColorManager.GetElementColor(element);
        var main = particleTravel.main;
        main.startColor = newColor;
    }

}
