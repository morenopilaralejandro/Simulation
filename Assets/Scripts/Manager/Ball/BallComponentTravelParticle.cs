using UnityEngine;
using Simulation.Enums.Duel;
using Simulation.Enums.Character;

public class BallComponentTravelParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleTravel;

    private Ball ball;
    private Vector3 travelDirection;

    private bool isPlaying;
    private Element currentElement;

    #region Unity Lifecycle

    private void Awake()
    {
        StopParticle();
    }

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

    #endregion

    public void Initialize(BallData ballData, Ball ball)
    {
        this.ball = ball;
    }

    #region Ball Events

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

    #endregion

    // =========================================================
    // MAIN ENTRY POINT
    // =========================================================
    public void UpdateTravelEffect(Move move, Element characterElement)
    {
        Element targetElement = move != null ? move.Element : characterElement;

        if (!isPlaying)
        {
            if (move == null) return;
            StartParticle(targetElement);
            return;
        }

        //if (playing)
        // Only update color if element changed
        if (currentElement != targetElement)
        {
            SetElementColor(targetElement);
            currentElement = targetElement;
        }
    }

    #region Particle Control

    private void StartParticle(Element element)
    {
        if (isPlaying)
            return;

        currentElement = element;
        SetElementColor(element);

        if (travelDirection != Vector3.zero)
            particleTravel.transform.forward = -travelDirection;

        particleTravel.Play(true);
        AudioManager.Instance.PlaySfxLoop("sfx-ball_energy");

        isPlaying = true;
    }

    private void StopParticle()
    {
        if (!isPlaying)
            return;

        particleTravel.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        AudioManager.Instance.StopSfxLoop("sfx-ball_energy");

        isPlaying = false;
    }

    private void SetElementColor(Element element)
    {
        var main = particleTravel.main;
        main.startColor = ColorManager.GetElementColor(element);
    }

    #endregion
}
