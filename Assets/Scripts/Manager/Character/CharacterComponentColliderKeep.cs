using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Battle;

[RequireComponent(typeof(Collider))]
public class CharacterComponentColliderKeep : MonoBehaviour
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
