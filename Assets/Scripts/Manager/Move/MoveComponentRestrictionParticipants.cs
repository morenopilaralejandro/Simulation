using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentRestrictionParticipants
{
    public List<Element> RequiredParticipantElements { get; private set; }
    public List<string> RequiredParticipantMoves { get; private set; }

    public MoveComponentRestrictionParticipants(MoveData moveData)
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


    public bool HasValidParticipantElements(Character[] selectedParticipants) 
    {
        if (HasParticipantElementRestriction)
        {
            for (int i = 0; i < selectedParticipants.Length; i++)
            {
                if (RequiredParticipantElements[i] != selectedParticipants[i].Element)
                    return false;
            }
            return true;
        } else {
            return true;
        }
    }

    public bool HasValidParticipantMoves(Character[] selectedParticipants) 
    {
        /*
        if (HasParticipantMoveRestriction)
        {
            for (int i = 0; i < selectedParticipants.Length; i++)
            {
                //if (!selectedParticipants[i].learnedMoves.Exists(move => move.MoveId == RequiredParticipantMoves[i]))
                    return false;
            }
            return true;
        } else {
            return true;
        }
        */
        return true; //delete this
    }
}
