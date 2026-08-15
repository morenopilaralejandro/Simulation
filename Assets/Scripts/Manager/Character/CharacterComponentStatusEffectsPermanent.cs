using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Character;

public class CharacterComponentStatusEffectsPermanent
{
    #region Field
    //the other one is volatile
    private Character character;

    private StatusEffectPermanent activeStatusEffectPermanent;

    public StatusEffectPermanent ActiveStatusEffectPermanent => activeStatusEffectPermanent;
    public bool IsFainted => activeStatusEffectPermanent == StatusEffectPermanent.Fainted;

    #endregion

    #region Lifecycle

    public CharacterComponentStatusEffectsPermanent(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;

        if (characterSaveData == null) return;
        activeStatusEffectPermanent = characterSaveData.StatusEffectPermanent;
    }

    #endregion

    #region Logic

    public void SetStatusPermanent(StatusEffectPermanent effect)
    {
        activeStatusEffectPermanent = effect;
    }

    public void ClearStatusPermanent()
    {
        activeStatusEffectPermanent = StatusEffectPermanent.None;
    }

    #endregion
}
