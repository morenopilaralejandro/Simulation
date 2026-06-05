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

    public static event Action<Wing, CharacterEntityBattle> OnWingUsed;
    public static void RaiseWingUsed(Wing wing, CharacterEntityBattle character)
    {
        OnWingUsed?.Invoke(wing, character);
    }
}
