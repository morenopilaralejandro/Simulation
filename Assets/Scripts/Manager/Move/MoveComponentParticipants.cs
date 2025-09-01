using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentParticipants
{
    public int TotalParticipantCount { get; private set; }
    public int RequiredParticipantCount { get; private set; }
    public Character[] SelectedParticipants { get; private set; }

    public MoveComponentParticipants(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        TotalParticipantCount = moveData.Participants;
        RequiredParticipantCount = moveData.Participants - 1;
        SelectedParticipants = new Character[RequiredParticipantCount];
    }

    public void SetParticipant(int participantIndex, Character character)
    {
        SelectedParticipants[participantIndex] = character;
    }

    public bool IsParticipantSelected(int participantIndex) => SelectedParticipants[participantIndex] != null;

    public List<Character> GetFinalParticipants(List<Character> teammates)
    {
        List<Character> finalParticipants = new List<Character>();

        for (int i = 0; i < RequiredParticipantCount; i++)
        {
            if (!IsParticipantSelected(i))
            {
                Character randomCharacter = teammates[Random.Range(0, BattleManager.Instance.CurrentTeamSize)];
                while (finalParticipants.Contains(randomCharacter))
                    randomCharacter = teammates[Random.Range(0, BattleManager.Instance.CurrentTeamSize)];
                finalParticipants.Add(randomCharacter);
            }
            else
            {
                finalParticipants.Add(SelectedParticipants[i]);
            }
        }

        return finalParticipants;
    }
}
