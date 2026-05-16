using System.Collections.Generic;
using Aremoreno.Enums.Character;

public static class CharacterAnimationInfo
{
    public static readonly Dictionary<CharacterAnimationState, int> Frames = new()
    {
        { CharacterAnimationState.Idle, 2 },
        { CharacterAnimationState.Walk, 9 },
        { CharacterAnimationState.Run, 8 },
        { CharacterAnimationState.Jump, 5 },
        { CharacterAnimationState.Combat, 2 },
        { CharacterAnimationState.Emote, 3 },
        { CharacterAnimationState.Slash, 6 },
        { CharacterAnimationState.Backslash1H, 13 },
        { CharacterAnimationState.Spellcast, 7 },
        { CharacterAnimationState.Hurt, 6 }
    };
}
