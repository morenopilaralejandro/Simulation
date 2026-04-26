using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class CharacterCard : MonoBehaviour
{
    [SerializeField] private CharacterPortraitBattle characterPortrait;
    [SerializeField] private Image imageElement;
    [SerializeField] private Image imageGender;
    [SerializeField] private Image imagePosition;
    [SerializeField] private TMP_Text textName;

    public void SetCharacter(Character character, Position position)
    {
        characterPortrait.SetCharacter(character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
        imageGender.sprite = IconManager.Instance.Gender.GetIcon(character.Gender);
        imagePosition.color = ColorManager.GetPositionColor(position);
        textName.text = character.CharacterNick;
    }

    public void Clear()
    {
        characterPortrait?.Clear();

        if (imageElement != null) imageElement.sprite = null;
        if (imageGender != null) imageGender.sprite = null;
        if (imagePosition != null) imagePosition.color = Color.white;
        if (textName != null) textName.text = string.Empty;
    }

}
