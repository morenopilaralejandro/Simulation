using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderPresence : MonoBehaviour
{
    #region Fields

    [SerializeField] private CharacterEntityBattle characterEntityBattle;
    public CharacterEntityBattle CharacterEntityBattle => characterEntityBattle;

    #endregion

    #region Unity Lifecycle

    /*
    private void OnTriggerEnter(Collider other)
    {
        TryStartDuel(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryStartDuel(other);
    }
    */

    #endregion

    #region Logic

    #endregion
}
