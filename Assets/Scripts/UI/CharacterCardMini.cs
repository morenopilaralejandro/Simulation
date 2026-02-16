using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class CharacterCardMini : MonoBehaviour
{
    [SerializeField] private CharacterPortraitBattle characterPortrait;
    [SerializeField] private Image imageElement;
    [SerializeField] private Image imagePosition;
    [SerializeField] private CanvasGroup canvasGroup;

    public void SetCharacter(Character character, Position position)
    {
        characterPortrait.SetCharacter(character);
        imageElement.sprite = IconManager.Instance.Element.GetIcon(character.Element);
        imagePosition.color = ColorManager.GetPositionColor(position);
    }

    public void SetCanvasState(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

}
