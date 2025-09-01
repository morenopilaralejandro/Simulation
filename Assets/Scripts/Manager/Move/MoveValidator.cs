using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public static class MoveValidator
{
    public static bool CanLearnMove(Character character, MoveData moveData)
    {
        if (moveData.AllowedElements.Count > 0 && !moveData.AllowedElements.Contains(character.Element))
            return false;

        if (moveData.AllowedPositions.Count > 0 && !moveData.AllowedPositions.Contains(character.Position))
            return false;

        if (moveData.AllowedGenders.Count > 0 && !moveData.AllowedGenders.Contains(character.Gender))
            return false;

        if (moveData.AllowedSizes.Count > 0 && !moveData.AllowedSizes.Contains(character.CharacterSize))
            return false;

        return true;
    }

    public static bool CanExecuteMove(Move move)
    {
        return move.HasValidParticipantElements() && move.HasValidParticipantMoves();
    }
}
