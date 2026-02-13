using System;
using UnityEngine;

public static class BallEvents
{
    //spawn
    public static event Action<Ball> OnBallSpawned;
    public static void RaiseBallSpawned(Ball ball)
    {
        OnBallSpawned?.Invoke(ball);
    }

    //possession
    public static event Action<CharacterEntityBattle> OnGained;
    public static void RaiseGained(CharacterEntityBattle characterEntityBattle)
    {
        OnGained?.Invoke(characterEntityBattle);
    }
    public static event Action<CharacterEntityBattle> OnReleased;
    public static void RaiseReleased(CharacterEntityBattle characterEntityBattle)
    {
        OnReleased?.Invoke(characterEntityBattle);
    }

    //travel
    public static event Action<Vector3> OnTravelStart;
    public static void RaiseTravelStart(Vector3 startPosition)
    {
        OnTravelStart?.Invoke(startPosition);
    }

    public static event Action<Vector3> OnTravelEnd;
    public static void RaiseTravelEnd(Vector3 endPosition)
    {
        OnTravelEnd?.Invoke(endPosition);
    }

    public static event Action OnTravelPause;
    public static void RaiseTravelPause()
    {
        OnTravelPause?.Invoke();
    }

    public static event Action OnTravelResume;
    public static void RaiseTravelResume()
    {
        OnTravelResume?.Invoke();
    }

    public static event Action OnTravelCancel;
    public static void RaiseTravelCancel()
    {
        OnTravelCancel?.Invoke();
    }
}
