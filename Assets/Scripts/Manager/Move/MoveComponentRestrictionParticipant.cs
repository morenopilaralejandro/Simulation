using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentRestrictionParticipant
{
    public List<Element> RequiredParticipantElements { get; private set; }
    public List<string> RequiredParticipantMoves { get; private set; }

    public MoveComponentRestrictionParticipant(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        RequiredParticipantElements = moveData.RequiredParticipantElements;
        RequiredParticipantMoves = moveData.RequiredParticipantMoves;
    }

    public bool HasParticipantElementRestriction => RequiredParticipantElements.Count > 0;
    public bool HasParticipantMoveRestriction => RequiredParticipantMoves.Count > 0;


    //TODO I need to get participant data
    public bool HasValidParticipantElements() 
    {
        if (HasParticipantElementRestriction)
        {
            for (int i = 0; i < RequiredParticipantElements.Count; i++)
            {
                if (i >= participants.Count) return false;
                if (RequiredParticipantElements[i] != participants[i].element)
                    return false;
            }
        } else {
            return true;
        }
    }

}
