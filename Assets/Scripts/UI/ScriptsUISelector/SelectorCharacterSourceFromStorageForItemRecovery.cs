using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;

public class SelectorCharacterSourceFromStorageForItemRecovery : ISelectorSource<Character>
{
    private readonly List<Character> _results = new();

    private readonly ItemRecovery itemRecovery;

    public SelectorCharacterSourceFromStorageForItemRecovery(ItemRecovery itemRecovery)
    {
        this.itemRecovery = itemRecovery;
    }

    public IEnumerable<Character> Enumerate()
    {
        _results.Clear();

        bool recoversHp = itemRecovery.RecoveryAmountHp > 0;
        bool recoversSp = itemRecovery.RecoveryAmountSp > 0;

        foreach (Character character in CharacterManager.Instance.Characters.Values)
        {
            if ((recoversHp && character.GetBattleStat(Stat.Hp) != character.GetTrueStat(Stat.Hp)) ||
                (recoversSp && character.GetBattleStat(Stat.Sp) != character.GetTrueStat(Stat.Sp)))
            {
                _results.Add(character);
            }
        }

        return _results;
    }
}
