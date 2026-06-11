using System;
using Aremoreno.Enums.Character;

public static class WingEvents
{
    public static event Action<Wing> OnWingCutsceneStart;
    public static void RaiseWingCutsceneStart(Wing wing)
    {
        OnWingCutsceneStart?.Invoke(wing);
    }

    public static event Action OnWingCutsceneEnd;
    public static void RaiseWingCutsceneEnd()
    {
        OnWingCutsceneEnd?.Invoke();
    }

    public static event Action<CharacterEntityBattle, Wing> OnWingActivated;
    public static void RaiseWingActivated(CharacterEntityBattle characterEntityBattle, Wing wing)
    {
        OnWingActivated?.Invoke(characterEntityBattle, wing);
    }

    public static event Action<Wing> OnWingAdded;
    public static void RaiseWingAdded(Wing wing)
    {
        OnWingAdded?.Invoke(wing);
    }

    public static event Action<Wing> OnWingRemoved;
    public static void RaiseWingRemoved(Wing wing)
    {
        OnWingRemoved?.Invoke(wing);
    }
}
