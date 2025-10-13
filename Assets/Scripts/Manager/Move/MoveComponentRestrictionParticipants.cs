using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentRestrictionParticipants
{
    private Move move;

    public List<Element> RequiredParticipantElements { get; private set; }
    public List<string> RequiredParticipantMoves { get; private set; }

    public MoveComponentRestrictionParticipants(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
        RequiredParticipantElements = moveData.RequiredParticipantElements ?? new List<Element>();
        RequiredParticipantMoves = moveData.RequiredParticipantMoves ?? new List<string>();
    }

    public bool IsCharacterValidForIndex(Character character, int index)
    {
        if (index < RequiredParticipantElements.Count &&
            character.Element != RequiredParticipantElements[index])
            return false;

        if (index < RequiredParticipantMoves.Count &&
            !character.IsMoveEquipped(RequiredParticipantMoves[index]))
            return false;

        return true;
    }

    public bool MeetsAllParticipantRestrictions(Character[] participants)
    {
        for (int i = 1; i < participants.Length; i++) // skip user at index 0
        {
            if (!IsCharacterValidForIndex(participants[i], i - 1))
                return false;
        }
        return true;
    }
}
