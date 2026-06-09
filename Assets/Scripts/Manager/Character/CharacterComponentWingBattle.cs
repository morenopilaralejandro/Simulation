using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class CharacterComponentWingBattle : MonoBehaviour
{
    #region Fields

    private CharacterEntityBattle character;

    private bool hasPendingWingActivation;
    private bool hasPendingWingCutscene;

    public bool HasPendingWingActivation => hasPendingWingActivation;
    public bool HasPendingWingCutscene => hasPendingWingCutscene;

    #endregion

    #region LifeCycle

    public void Initialize(CharacterEntityBattle character)
    {
        this.character = character;
    }

    #endregion

    #region Activation

    public void ActivateWings()
    {
        if (!character.HasWingEquipped) return;
        if (character.HasWingActivated) return;
        character.ResetWingTimesUsed();
        character.SetWingActive(true);
        SetPendingWingActivation(false);
        DuelLogManager.Instance.AddActionWingActivate(character.Character, character.TeamSide, character.Wing);
        //WingEvents.RaiseWingActivated(character, character.Wing); -> called on DuelManager TryPlayWingCutscene

        foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
        {
            if (stat == Stat.Hp || stat == Stat.Sp) continue;
            character.ModifyBattleStat(stat, character.Wing.GetTrueStat(stat));
        }

    }

    public void DeactivateWings()
    {
        if (!character.HasWingActivated) return;
        character.SetWingActive(false);
        DuelLogManager.Instance.AddActionWingDeactivate(character.Character, character.TeamSide, character.Wing);

        foreach (Stat stat in Enum.GetValues(typeof(Stat))) 
        {
            if (stat == Stat.Hp || stat == Stat.Sp) continue;
            character.ModifyBattleStat(stat, -character.Wing.GetTrueStat(stat));
        }
    }

    public void TryDeactivateWings()
    {
        if (!character.HasWingActivated) return;
        character.IncreaseWingTimesUsed();
        if (character.WingTimesUsed >= BattleWingManager.MAX_USAGES) 
        {
            DeactivateWings();
            HideWings();
        }
    }

    public bool CanActivateWings() => character.HasWingEquipped && BattleWingManager.Instance.CanActivateWings(character.TeamSide) && character.HasWingActivated;

    #endregion

    #region Show

    public void ShowWings()
    {
        _ = character.LoadWingAsync();
    }

    public void HideWings()
    {
        character.UnloadWing();
    }

    #endregion

    #region Pending

    public void SetPendingWingActivation(bool boolValue)
    {
        hasPendingWingActivation = boolValue;
    }

    public void SetPendingWingCutscene(bool boolValue)
    {
        hasPendingWingCutscene = boolValue;
    }

    #endregion
}
