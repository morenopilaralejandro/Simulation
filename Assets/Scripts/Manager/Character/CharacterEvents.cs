using System;
using Simulation.Enums.Character;

public static class CharacterEvents
{
    public static event Action<Character> OnControlChange;
    public static void RaiseControlChange(Character character)
    {
        OnControlChange?.Invoke(character);
    }

    public static event Action<Character> OnTargetChange;
    public static void RaiseTargetChange(Character character)
    {
        OnTargetChange?.Invoke(character);
    }
}
