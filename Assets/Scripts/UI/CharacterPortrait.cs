using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortrait : MonoBehaviour
{
    [SerializeField] private Image imageKitPortraitSprite;
    [SerializeField] private Image imageCharacterPortrait;

    public void SetCharacter(Character character)
    {
        SetKitPortrait(character);
        SetCharacterPortrait(character);
    }

    public void SetKitPortrait(Character character)
    {
        imageKitPortraitSprite.sprite = character.KitPortraitSprite;
    }

    public void SetCharacterPortrait(Character character)
    {
        imageCharacterPortrait.sprite = character.CharacterPortraitSprite;
    }

}
