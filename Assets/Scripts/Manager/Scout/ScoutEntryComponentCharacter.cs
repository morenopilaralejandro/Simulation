using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Scout;

public class ScoutEntryComponentCharacter
{
    public string CharacterId { get; private set; }
    public int Cost { get; private set; }
    public int Level { get; private set; }
    public Character Character { get; private set; }
    public bool IsOwned { get; private set; }
    public bool IsAffordable { get; private set; }

    public ScoutEntryComponentCharacter(ScoutEntryData data)
    {
        CharacterId = data.CharacterId;
        Cost = data.Cost;
        Level = data.Level;

        Character = new Character(DatabaseManager.Instance.GetCharacterData(CharacterId));
        Character.SetLevel(Level);
        Character.SetKit(
            TeamManager.Instance.ActiveLoadout.Kit,
            Variant.Home,
            Character.GetKitRole(Character.Position));

        IsOwned = CharacterManager.Instance.HasCharacterById(CharacterId);
        IsAffordable = ItemManager.Instance.CanAfford(CurrencyType.Gold, Cost);
    }
}
