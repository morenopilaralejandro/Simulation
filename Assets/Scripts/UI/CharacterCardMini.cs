using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCardMini : MonoBehaviour
{
    [SerializeField] private CharacterPortraitBattle characterPortrait;
    [SerializeField] private Image imageElement;
    [SerializeField] private Image imagePosition;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetCharacter(Character character)
    {
        characterPortrait.SetCharacter(character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
        imagePosition.color = ColorManager.GetPositionColor(character.FormationCoord.Position);
    }

    public void SetCanvasState(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

}
