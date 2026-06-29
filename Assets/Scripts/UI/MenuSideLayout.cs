using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.World;

public class MenuSideLayout : MonoBehaviour
{
    [Header("UI References - Time")]
    [SerializeField] private TMP_Text textTime;

    [Header("UI References - Zone")]
    [SerializeField] private TMP_Text textZoneName;

    [Header("UI References - Currency")]
    [SerializeField] private TMP_Text textGold;

    [Header("UI References - Character")]
    [SerializeField] private MenuSideCharacterSlot character0;
    [SerializeField] private MenuSideCharacterSlot character1;
    [SerializeField] private MenuSideCharacterSlot character2;
    [SerializeField] private MenuSideCharacterSlot character3;

    private Kit currentKit;
    private Variant currentVariant = Variant.Home;

    public void Populate()
    {
        if (textTime != null)
            textTime.text = WorldManager.Instance?.GetTimeAsString();

        if (textZoneName != null)
            textZoneName.text = WorldManager.Instance != null ? WorldManager.Instance.ZoneName : string.Empty;

        if (textGold != null)
        {
            int gold = 0;

            if (ItemManager.Instance != null)
                gold = ItemManager.Instance.GetAmount(CurrencyType.Gold);

            textGold.text = gold.ToString();
        }

        if (TeamManager.Instance == null ||
            TeamManager.Instance.ActiveLoadout == null ||
            TeamManager.Instance.ActiveLoadout.MiniBattleCharacterGuids == null ||
            CharacterManager.Instance == null)
        {
            ClearCharacters();
            return;
        }

        var guids = TeamManager.Instance.ActiveLoadout.MiniBattleCharacterGuids;
        currentKit = TeamManager.Instance.ActiveLoadout.Kit;

        SetSlot(character0, guids, 0, Position.GK);
        SetSlot(character1, guids, 1, Position.FW);
        SetSlot(character2, guids, 2, Position.FW);
        SetSlot(character3, guids, 3, Position.FW);
    }

    public void Clear()
    {
        if (textTime != null)
            textTime.text = string.Empty;

        if (textZoneName != null)
            textZoneName.text = string.Empty;

        if (textGold != null)
            textGold.text = string.Empty;

        ClearCharacters();
    }

    private void SetSlot(MenuSideCharacterSlot slot, IList<string> guids, int index, Position position)
    {
        if (slot == null)
            return;

        if (guids == null || index < 0 || index >= guids.Count || string.IsNullOrEmpty(guids[index]))
        {
            slot.Clear();
            return;
        }

        var character = CharacterManager.Instance.GetCharacter(guids[index]);

        character.SetKit(
            currentKit, 
            currentVariant, 
            character.GetKitRole(position)
        );

        if (character == null)
            slot.Clear();
        else
            slot.SetCharacter(character, position);
    }

    private void ClearCharacters()
    {
        character0?.Clear();
        character1?.Clear();
        character2?.Clear();
        character3?.Clear();
    }
}
