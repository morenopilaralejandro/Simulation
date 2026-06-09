using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;

public class BattleWingManager : MonoBehaviour
{
    public static BattleWingManager Instance { get; private set; }

    private Dictionary<TeamSide, bool> hasActivatedWing = new();
    public const int MAX_USAGES = 2;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        hasActivatedWing[TeamSide.Home] = false;
        hasActivatedWing[TeamSide.Away] = false;

        InitializeForBattle();
    }

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

    #region Events

    private void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        WingEvents.OnWingActivated += HandleWingActivated;
    }

    private void OnDisable()
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
