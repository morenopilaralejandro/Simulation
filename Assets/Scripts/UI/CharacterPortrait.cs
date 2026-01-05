using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Simulation.Enums.Character;

public class CharacterPortraitBattle : MonoBehaviour
{
    [SerializeField] private Image imageKitPortraitBase;
    [SerializeField] private Image imageKitPortraitDetail;
    [SerializeField] private Image imageKitPortraitNeck;
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private KitPortraitLibrary kitPortraitLibrary;

    private PortraitSize _cachedSize;
    private string _cachedId;

    public void SetCharacter(Character character)
    {        
        if(character.CharacterId != _cachedId)
            UpdateCharacterPortraitSprite(character);
        if(character.PortraitSize != _cachedSize)
            UpdateKitPortraitSprites(character);
        UpdateKitPortraitColors(character);
    }

    public void UpdateCharacterPortraitSprite(Character character)
    {
        imageCharacterPortrait.sprite = character.PortraitSprite;
        _cachedId = character.CharacterId;
    }

    public void UpdateKitPortraitSprites(Character character)
    {
        var size = character.PortraitSize;
        var kitPortraitSprites = kitPortraitLibrary.Get(size);
        imageKitPortraitBase.sprite = kitPortraitSprites.SpriteBase;
        imageKitPortraitDetail.sprite = kitPortraitSprites.SpriteDetail;
        imageKitPortraitNeck.sprite = kitPortraitSprites.SpriteNeck;
        _cachedSize = size;
    }

    public void UpdateKitPortraitColors(Character character)
    {
        var kitColor = character.GetTeam().Kit.GetColors(character.GetKitVariant(character.GetTeam()), character.GetKitRole());

        imageKitPortraitBase.color = kitColor.Base;
        imageKitPortraitDetail.color = kitColor.Detail;
        imageKitPortraitNeck.color = character.BodyColor;
    }

}
