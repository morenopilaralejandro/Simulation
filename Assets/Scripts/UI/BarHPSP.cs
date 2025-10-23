using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class BarHPSP : MonoBehaviour
{
    [SerializeField] private TMP_Text textNumber;

    public void SetCharacter(Character character, Stat stat)
    {
        if (character != null)
        {
            textNumber.text = $"{character.GetBattleStat(stat)}/{character.GetTrueStat(stat)}";
        }
        else
        {
            textNumber.text = "";
        }
    }

}
