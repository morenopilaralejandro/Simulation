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
    public bool IsKeeper = false;

    public List<DuelParticipant> Participants = new List<DuelParticipant>();

    public DuelParticipant LastOffense;
    public DuelParticipant LastDefense;

    public List<DuelParticipant> OffenseParticipants = new List<DuelParticipant>();
    public List<DuelParticipant> DefenseParticipants = new List<DuelParticipant>();

    public float AttackPressure;
    public float TotalDefense;
    
    public void Reset()
    {
        Participants.Clear();
        OffenseParticipants.Clear();
        DefenseParticipants.Clear();
        AttackPressure = 0f;
        TotalDefense = 0f;
        LastOffense = null;
        LastDefense = null;
        IsResolved = false;
        IsKeeper = false;
    }
}
