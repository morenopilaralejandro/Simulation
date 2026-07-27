using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Input;
using Aremoreno.Enums.World;

public class MenuSideCharacterSlot : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private BarHPSP barHp;
    [SerializeField] private BarHPSP barSp;
    [SerializeField] private BarXP barXp;
    [SerializeField] private TMP_Text textLv;

    public void SetCharacter(Character character, Position position) 
    {
        characterCard.SetCharacter(character, position);
        barHp.SetCharacter(character, Stat.Hp);
        barSp.SetCharacter(character, Stat.Sp);
        barXp.SetCharacter(character);
        textLv.text = $"{character.Level}";
    }

    public void Clear()
    {
        characterCard.Clear();
        barHp.Clear();
        barSp.Clear();
        barXp.Clear();
        textLv.text = "";
    }

    public IEnumerator AnimateXp(BattleResultDataXp result)
    {
        textLv.text = result.StartLevel.ToString();
        barXp.SetData(result.StartXp, result.StartXpToNextLevel);

        yield return new WaitForSeconds(0.3f);

        textLv.text = result.EndLevel.ToString();
        barXp.SetData(result.EndXp, result.EndXpToNextLevel);

        if(result.EndLevel > result.StartLevel)
            AudioManager.Instance.PlaySfxUI("sfx-level_up");        
    }
}
