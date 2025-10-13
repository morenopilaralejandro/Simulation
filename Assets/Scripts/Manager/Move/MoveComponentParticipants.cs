using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentParticipants
{
    private Move move;

    public int TotalParticipantCount { get; private set; }
    public int RequiredParticipantCount { get; private set; }
    public Character[] SelectedParticipants { get; private set; }
    public List<Character> FinalParticipants { get; private set; }

    public MoveComponentParticipants(MoveData moveData, Move move)
    {
        Initialize(moveData, move);
    }

    public void Initialize(MoveData moveData, Move move)
    {
        this.move = move;
        TotalParticipantCount = moveData.Participants;
        RequiredParticipantCount = moveData.Participants - 1;
        SelectedParticipants = new Character[RequiredParticipantCount]; 
    }

    public bool TryFinalizeParticipants(Character user, List<Character> teammates)
    {
        FinalParticipants = new List<Character> { user };
        HashSet<Character> used = new HashSet<Character> { user };

        for (int i = 0; i < RequiredParticipantCount; i++)
        {
            Character participant = SelectedParticipants[i];

            if (participant == null)
                participant = FindValidParticipant(i, teammates, used);

            if (participant == null)
                return false; // couldn’t satisfy restriction

            FinalParticipants.Add(participant);
            used.Add(participant);
        }

        return this.move.MeetsAllParticipantRestrictions(FinalParticipants.ToArray());
    }

    private Character FindValidParticipant(int index, List<Character> teammates, HashSet<Character> used)
    {
        foreach (var candidate in teammates.Where(t => !used.Contains(t)))
        {
            if (this.move.IsCharacterValidForIndex(candidate, index))
                return candidate;
        }
        return null;
    }
}
