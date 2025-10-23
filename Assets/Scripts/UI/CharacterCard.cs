using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] private CharacterPortrait characterPortrait;
    [SerializeField] private Image imageElement;
    [SerializeField] private Image imageGender;
    [SerializeField] private TMP_Text textName;

    public void SetCharacter(Character character)
    {
        characterPortrait.SetCharacter(character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
        imageGender.sprite = IconManager.Instance.Gender.GetIcon(character.Gender);
        textName.text = character.CharacterNick;
    }

}
