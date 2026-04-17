using System;
using Aremoreno.Enums.Character;

public static class CharacterEvents
{
    public static event Action<CharacterEntityBattle, TeamSide> OnControlChange;
    public static void RaiseControlChange(CharacterEntityBattle characterEntityBattle, TeamSide teamSide)
    {
        OnControlChange?.Invoke(characterEntityBattle, teamSide);
    }

    public static event Action<CharacterEntityBattle, TeamSide> OnTargetChange;
    public static void RaiseTargetChange(CharacterEntityBattle characterEntityBattle, TeamSide teamSide)
    {
        OnTargetChange?.Invoke(characterEntityBattle, teamSide);
    }

    public static event Action<Character> OnSpeechBubbleShown;
    public static void RaiseSpeechBubbleShown(Character character)
    {
        OnSpeechBubbleShown?.Invoke(character);
    }

    public static event Action<Character> OnSpeechBubbleHidden;
    public static void RaiseSpeechBubbleHidden(Character character)
    {
        OnSpeechBubbleHidden?.Invoke(character);
    }

    public static event Action<Character> OnCharacterAdded;
    public static void RaiseCharacterAdded(Character character)
    {
        OnCharacterAdded?.Invoke(character);
    }

    public static event Action<Character> OnCharacterRemoved;
    public static void RaiseCharacterRemoved(Character character)
    {
        OnCharacterRemoved?.Invoke(character);
    }

}
