using System;
using UnityEngine;

public static class BallEvents
{
    //possession
    public static event Action<Character> OnGained;
    public static void RaiseGained(Character character)
    {
        OnGained?.Invoke(character);
    }
    public static event Action<Character> OnReleased;
    public static void RaiseReleased(Character character)
    {
        OnReleased?.Invoke(character);
    }

    //travel
    public static event Action<Vector3> OnStartTravel;
    public static void RaiseStartTravel(Vector3 startPosition)
    {
        OnStartTravel?.Invoke(startPosition);
    }

    public static event Action<Vector3> OnEndTravel;
    public static void RaiseEndTravel(Vector3 endPosition)
    {
        OnEndTravel?.Invoke(endPosition);
    }

    public static event Action OnPauseTravel;
    public static void RaisePauseTravel()
    {
        OnPauseTravel?.Invoke();
    }

    public static event Action OnResumeTravel;
    public static void RaiseResumeTravel()
    {
        OnResumeTravel?.Invoke();
    }

    public static event Action OnCancelTravel;
    public static void RaiseCancelTravel()
    {
        OnCancelTravel?.Invoke();
    }
}
