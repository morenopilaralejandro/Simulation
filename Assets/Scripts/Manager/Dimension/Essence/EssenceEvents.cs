using System;
using UnityEngine;
using Aremoreno.Enums.Character;

public static class EssenceEvents
{
    public static event Action<CharacterEntityBattle> OnCharacterUnderwentEssenceOverflow;
    public static void RaiseCharacterUnderwentEssenceOverflow(CharacterEntityBattle characterEntityBattle)
    {
        OnCharacterUnderwentEssenceOverflow?.Invoke(characterEntityBattle);
    }

    public static event Action<Transform> OnPlayEssenceVfxRequested;
    public static void RaisePlayEssenceVfxRequested(Transform characterTransform)
    {
        OnPlayEssenceVfxRequested?.Invoke(characterTransform);
    }

    public static event Action<TeamSide, int> OnEssenceBattleLimitReached;
    public static void RaiseEssenceBattleLimitReached(TeamSide teamSide, int essenceOverflowUnderwent)
    {
        OnEssenceBattleLimitReached?.Invoke(teamSide, essenceOverflowUnderwent);
    }
}
