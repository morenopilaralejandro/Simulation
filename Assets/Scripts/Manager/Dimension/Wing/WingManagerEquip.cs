using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingManagerEquip
{
    /*
    private Dictionary<string, Character> characters = new();

    public IReadOnlyDictionary<string, Character> Characters => characters;
    public int Count => characters.Count;
    */

    #region Constuctor

    public WingManagerEquip() { }

    #endregion

    #region Equip

    public void EquipWing(Character character, Wing wing)
    {
        character.SetWingEquipped(wing);
        wing.SetEquippedCharacter(character);
    }

    public void UnequipWing(Character character)
    {
        character.Wing.SetEquippedCharacter(null);
        character.SetWingEquipped(null);
    }

    public void UnequipWing(Wing wing)
    {
        wing.EquippedCharacter.SetWingEquipped(null);
        wing.SetEquippedCharacter(null);
    }

    #endregion

}
