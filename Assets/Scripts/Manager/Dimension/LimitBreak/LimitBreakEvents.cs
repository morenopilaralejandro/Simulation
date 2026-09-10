using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.LimitBreak;

public static class LimitBreakEvents
{
    public static event Action<Wing> OnWingLimitBreakPerformed;
    public static void RaiseWingLimitBreakPerformed(Wing wing)
    {
        OnWingLimitBreakPerformed?.Invoke(wing);
    }

    public static event Action<Move> OnMoveLimitBreakPerformed;
    public static void RaiseMoveLimitBreakPerformed(Move move)
    {
        OnMoveLimitBreakPerformed?.Invoke(move);
    }

    public static event Action<Character> OnCharacterAwakenPerformed;
    public static void RaiseCharacterAwakenPerformed(Character character)
    {
        OnCharacterAwakenPerformed?.Invoke(character);
    }
}
