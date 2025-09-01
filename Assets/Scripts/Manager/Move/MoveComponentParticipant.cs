using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveComponentParticipant
{
    public int Participants { get; private set; }
    public Character[] SelectedParticipants { get; private set; }

    public MoveComponentParticipant(MoveData moveData)
    {
        Initialize(moveData);
    }

    public void Initialize(MoveData moveData)
    {
        Participants = moveData.Participants;

        SelectedParticipants = new Character[Participants];
    }

    public void SetParticipant(int participantIndex, Character character)
    {
        SelectedParticipants[participantIndex] = character;
    }

    public bool IsParticipantSelected(int participantIndex) => SelectedParticipants[participantIndex] != null;

    public List<Character> GetFinalParticipants(List<Character> teammates)
    {
        List<Character> finalParticipants = new List<Character>();

        for (int i = 0; i < Participants; i++)
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
