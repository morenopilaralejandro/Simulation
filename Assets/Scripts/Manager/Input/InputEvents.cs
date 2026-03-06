using System;
using UnityEngine;
using Simulation.Enums.Input;

public static class InputEvents
{
    public static event Action<DirectionalInputMode> OnDirectionalInputModeChanged;
    public static void RaiseDirectionalInputModeChanged(DirectionalInputMode directionalInputMode)
    {
        OnDirectionalInputModeChanged?.Invoke(directionalInputMode);
    }
}
