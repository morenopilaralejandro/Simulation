using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;

public class FieldDuelHandler : IDuelHandler 
{
    private Duel duel;
    List<DuelParticipant> supporters = new List<DuelParticipant>();

    public FieldDuelHandler(Duel duel) 
    { 
        this.duel = duel;
    }

    public void StartDuel() {
        
    }

    public void AddParticipant(DuelParticipant p) {
        //if is support
        duel.Participants.Add(p);
        if (duel.Participants.Count >= 2) {
            Resolve();
        }
    }

    public void Resolve() 
    { 
        var offense = duel.Participants.First(x => x.Action == DuelAction.Offense);
        var defense = duel.Participants.First(x => x.Action == DuelAction.Defense);
    }

    public void Cancel() { /* cleanup */ }
}
