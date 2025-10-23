using System;
using Simulation.Enums.Character;

public static class CharacterEvents
{
    public static event Action<Character, TeamSide> OnControlChange;
    public static void RaiseControlChange(Character character, TeamSide teamSide)
    {
        OnControlChange?.Invoke(character, teamSide);
    }

    public static event Action<Character, TeamSide> OnTargetChange;
    public static void RaiseTargetChange(Character character, TeamSide teamSide)
    {
        OnTargetChange?.Invoke(character, teamSide);
    }
}
