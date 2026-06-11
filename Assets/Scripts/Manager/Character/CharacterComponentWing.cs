using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class CharacterComponentWing
{
    #region Fields

    private Character character;

    public Wing Wing { get; private set; }
    public bool HasWingActivated { get; private set; }
    public int WingTimesUsed { get; private set; }
    private string wingIdDefault;

    #endregion

    #region LifeCycle

    public CharacterComponentWing(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;
        HasWingActivated = false;
        wingIdDefault = characterData.WingId;

        if (characterSaveData != null) 
        {
            EquipWing(WingManager.Instance.GetWing(characterSaveData.EquippedWingGuid));
        } else 
        {
            Wing = null;
        }
    }

    #endregion

    #region Equip

    public bool HasWingEquipped => Wing != null;

    public void EquipWing(Wing wing)
    {
        WingManager.Instance.EquipWing(character, wing);
    }

    public void UnequipWing()
    {
        WingManager.Instance.UnequipWing(character);
    }

    public void SetWingEquipped(Wing wing) 
    {
        Wing = wing;
        if (wing != null) character.SetWing(wing);
    }

    public void ForceEquipWing(Wing wing)
    {
        SetWingEquipped(wing);
    }

    public void TryEquipWingDefault()
    {
        if (string.IsNullOrEmpty(wingIdDefault)) return;
        if (!BattleArgs.AwayTeamCanUseWing) return;

        ForceEquipWing(WingFactory.CreateFromData(wingIdDefault));
    }

    #endregion

    #region Activation

    public void SetWingActive(bool boolValue)
    {
        HasWingActivated = boolValue;
    }

    #endregion

    #region Evolution

    public void ForceMaxEvolutionOnEquippedWing() 
    {
        Wing.ForceMaxEvolution();
    }

    #endregion

    #region Refinement

    public bool CanApplyWingElementMatchBonus(Element element) 
    {
        if (!character.HasWingActivated) return false;
        return character.Wing.ContainsElement(element);
    }

    #endregion

    #region Stats

    #endregion

    #region Usage

    public void IncreaseWingTimesUsed() 
    {
        WingTimesUsed++;
    }

    public void ResetWingTimesUsed() 
    {
        WingTimesUsed = 0;
    }

    #endregion

}
