using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardMini : MonoBehaviour
{
    [SerializeField] private CharacterPortrait characterPortrait;
    [SerializeField] private Image imageElement;

    public void SetCharacter(Character character)
    {
        characterPortrait.SetCharacter(character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
    }

}
