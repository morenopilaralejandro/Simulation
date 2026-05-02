using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class BarXP : MonoBehaviour
{
    [SerializeField] private TMP_Text textNumber;

    public void SetCharacter(Character character)
    {
        if (character != null)
            textNumber.text = $"{character.CurrentExp}/{character.ExpToNextLevel}";
        else
            textNumber.text = "";
    }

    public void Clear() 
    {
        textNumber.text = "";
    }
}
