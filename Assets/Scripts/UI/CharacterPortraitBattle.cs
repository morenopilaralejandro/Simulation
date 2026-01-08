using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Simulation.Enums.Character;
using Simulation.Enums.SpriteLayer;

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
        imageKitPortraitBase.color = character.SpriteLayerState.Colors[CharacterSpriteLayer.KitBase];
        imageKitPortraitDetail.color = character.SpriteLayerState.Colors[CharacterSpriteLayer.KitDetail];
        imageKitPortraitNeck.color = character.SpriteLayerState.Colors[CharacterSpriteLayer.Body];
    }

}
