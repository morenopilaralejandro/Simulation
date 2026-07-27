using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class MenuSideCharacterGroupLayout : MonoBehaviour
{
    [SerializeField] private MenuSideCharacterSlot character0;
    [SerializeField] private MenuSideCharacterSlot character1;
    [SerializeField] private MenuSideCharacterSlot character2;
    [SerializeField] private MenuSideCharacterSlot character3;

    private readonly MenuSideCharacterSlot[] _slots = new MenuSideCharacterSlot[4];

    private void Awake()
    {
        _slots[0] = character0;
        _slots[1] = character1;
        _slots[2] = character2;
        _slots[3] = character3;
    }

    public void Populate()
    {
        if (TeamManager.Instance == null ||
            TeamManager.Instance.ActiveLoadout == null ||
            TeamManager.Instance.ActiveLoadout.MiniBattleCharacterGuids == null ||
            CharacterManager.Instance == null)
        {
            Clear();
            return;
        }

        var loadout = TeamManager.Instance.ActiveLoadout;
        var guids = loadout.MiniBattleCharacterGuids;

        for (int i = 0; i < _slots.Length; i++)
            SetSlot(_slots[i], guids, i, loadout.Kit);
    }

    public void Clear()
    {
        foreach (var slot in _slots)
            slot?.Clear();
    }

    private void SetSlot(MenuSideCharacterSlot slot, IList<string> guids, int index, Kit kit)
    {
        if (slot == null)
            return;

        if (guids == null ||
            index >= guids.Count ||
            string.IsNullOrEmpty(guids[index]))
        {
            slot.Clear();
            return;
        }

        var character = CharacterManager.Instance.GetCharacter(guids[index]);

        if (character == null)
        {
            slot.Clear();
            return;
        }

        var position = TeamManager.Instance.ActiveLoadout
            .GetFormation(BattleType.Mini)
            .FormationCoords[index]
            .Position;

        character.SetKit(
            kit,
            Variant.Home,
            character.GetKitRole(position));

        slot.SetCharacter(character, position);
    }


    /*

    public void Populate(List<BattleResultDataXp> xpResult)
    {
        if (TeamManager.Instance == null ||
            TeamManager.Instance.ActiveLoadout == null ||
            TeamManager.Instance.ActiveLoadout.MiniBattleCharacterGuids == null ||
            CharacterManager.Instance == null)
        {
            Clear();
            return;
        }

        var loadout = TeamManager.Instance.ActiveLoadout;
        var guids = loadout.MiniBattleCharacterGuids;

        for (int i = 0; i < _slots.Length; i++)
            SetSlot(_slots[i], guids, i, loadout.Kit);
    }

    */

    public void AnimateXp(List<BattleResultDataXp> xpResult)
    {
        for (int i = 0; i < _slots.Length; i++)
            StartCoroutine(_slots[i].AnimateXp(xpResult[i]));
    }
    
}
