using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class Duel
{
    public DuelMode DuelMode;
    public bool IsResolved = true;
    public bool IsKeeperDuel = false;

    public List<DuelParticipant> Participants = new List<DuelParticipant>();

    public DuelParticipant LastOffense;
    public DuelParticipant LastDefense;
    public List<DuelParticipant> OffenseParticipants = new List<DuelParticipant>();
    public List<DuelParticipant> DefenseParticipants = new List<DuelParticipant>();
    public List<Character> OffenseSupports = new List<Character>();
    public List<Character> DefenseSupports = new List<Character>();

    public float OffensePressure;
    public float DefensePressure;
    
    public void Reset()
    {
        Participants.Clear();
        OffenseParticipants.Clear();
        DefenseParticipants.Clear();
        OffenseSupports.Clear();
        DefenseSupports.Clear();
        OffensePressure = 0f;
        DefensePressure = 0f;
        LastOffense = null;
        LastDefense = null;
        IsResolved = false;
        IsKeeperDuel = false;
    }
}
