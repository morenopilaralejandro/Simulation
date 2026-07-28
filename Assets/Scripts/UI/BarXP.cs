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
            textNumber.text = $"{character.CurrentXp}/{character.XpToNextLevel}";
        else
            textNumber.text = "";
    }

    public void SetData(int currentXp, int xpToNextLevel)
    {
        textNumber.text = $"{currentXp}/{xpToNextLevel}";
    }

    public void Clear() 
    {
        textNumber.text = "";
    }
}
