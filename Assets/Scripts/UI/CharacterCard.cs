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
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetCharacter(Character character, Position position)
    {
        _ = characterPortrait.SetCharacterAsync(character);
        if (imageElement != null)
            imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
        if (imageGender != null)
            imageGender.sprite = IconManager.Instance.Gender.GetIcon(character.Gender);
        if (imagePosition != null) 
            imagePosition.color = character.IsFainted ? ColorManager.FaintedColor : ColorManager.GetPositionColor(position);
        if (textName != null)
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

    public void SetVisible(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

}
