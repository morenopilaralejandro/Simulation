using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public static class MoveEvents
{
    public static event Action<Move> OnMoveCutsceneStart;
    public static void RaiseMoveCutsceneStart(Move move)
    {
        OnMoveCutsceneStart?.Invoke(move);
    }

    public static event Action OnMoveCutsceneEnd;
    public static void RaiseMoveCutsceneEnd()
    {
        OnMoveCutsceneEnd?.Invoke();
    }

    public static event Action<Move, CharacterEntityBattle> OnMoveUsed;
    public static void RaiseMoveUsed(Move move, CharacterEntityBattle character)
    {
        OnMoveUsed?.Invoke(move, character);
    }
}
