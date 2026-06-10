using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;

public class BattleManagerWing
{
    #region Fields

    private Dictionary<TeamSide, bool> hasActivatedWing = new();
    public const int MAX_USAGES = 2;

    #endregion

    #region Constructor

    public BattleManagerWing()
    {
        hasActivatedWing[TeamSide.Home] = false;
        hasActivatedWing[TeamSide.Away] = false;

        InitializeForBattle();
    }

    #endregion

    #region Logic

    public void InitializeForBattle()
    {
        hasActivatedWing[TeamSide.Home] = false;
        hasActivatedWing[TeamSide.Away] = false;
    }

    public bool CanActivateWings(TeamSide teamside) 
    {
        return !hasActivatedWing[teamside];
    }

    #endregion

    #region Helpers

    #endregion

    #region Events

    public void Subscribe()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        WingEvents.OnWingActivated += HandleWingActivated;
    }

    public void Unsubscribe()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;
        WingEvents.OnWingActivated -= HandleWingActivated;
    }

    private void HandleBattleStart(BattleType battleType) 
    {
        InitializeForBattle();
    }

    private void HandleWingActivated(CharacterEntityBattle character, Wing wing)
    {
        hasActivatedWing[character.TeamSide] = true;
    }

    #endregion

}
