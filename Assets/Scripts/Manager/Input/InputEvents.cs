using System;
using UnityEngine;
using Aremoreno.Enums.Input;

public static class InputEvents
{
    public static event Action<DirectionalInputMode> OnDirectionalInputModeChanged;
    public static void RaiseDirectionalInputModeChanged(DirectionalInputMode directionalInputMode)
    {
        OnDirectionalInputModeChanged?.Invoke(directionalInputMode);
    }

    public static event Action OnScreenControlsShowRequested;
    public static void RaiseScreenControlsShowRequested()
    {
        OnScreenControlsShowRequested?.Invoke();
    }

    public static event Action OnScreenControlsHideRequested;
    public static void RaiseScreenControlsHideRequested()
    {
        OnScreenControlsHideRequested?.Invoke();
    }

    public static event Action<InputDeviceType> OnDeviceTypeChanged;
    public static void RaiseDeviceTypeChanged(InputDeviceType inputDeviceType)
    {
        OnDeviceTypeChanged?.Invoke(inputDeviceType);
    }



}
