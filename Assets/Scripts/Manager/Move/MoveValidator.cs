using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public static class MoveValidator
{
    /*
    public static bool CanLearnMove(Character character, MoveData move)
    {
        if (move.allowedElements.Count > 0 && !move.allowedElements.Contains(character.element))
            return false;

        if (move.allowedPositions.Count > 0 && !move.allowedPositions.Contains(character.position))
            return false;

        if (move.allowedGenders.Count > 0 && !move.allowedGenders.Contains(character.gender))
            return false;

        if (move.allowedSizes.Count > 0 && !move.allowedSizes.Contains(character.size))
            return false;

        return true;
    }

    public static bool CanExecuteMove(List<Character> participants, MoveData move)
    {
        if (move.hasParticipantElementRestriction)
        {
            for (int i = 0; i < move.requiredParticipantElements.Count; i++)
            {
                if (i >= participants.Count) return false;
                if (move.requiredParticipantElements[i] != participants[i].element)
                    return false;
            }
        }

        if (move.hasParticipantMoveRestriction)
        {
            for (int i = 0; i < move.requiredParticipantMoves.Count; i++)
            {
                if (i >= participants.Count) return false;
                if (!participants[i].learnedMoves.Exists(m => m.moveName == move.requiredParticipantMoves[i]))
                    return false;
            }
        }
        return true;
    }
    */
}
