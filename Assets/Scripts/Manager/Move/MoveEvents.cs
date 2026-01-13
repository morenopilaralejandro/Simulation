using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public static class MoveEvents
{
    public static event Action OnMoveCutsceneStart;
    public static void RaiseMoveCutsceneStart()
    {
        OnMoveCutsceneStart?.Invoke();
    }

    public static event Action OnMoveCutsceneEnd;
    public static void RaiseMoveCutsceneEnd()
    {
        OnMoveCutsceneEnd?.Invoke();
    }

    public static event Action<Move, Character> OnMoveUsed;
    public static void RaiseMoveUsed(Move move, Character character)
    {
        OnMoveUsed?.Invoke(move, character);
    }
}
