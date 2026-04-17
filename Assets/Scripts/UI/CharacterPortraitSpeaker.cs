using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.SpriteLayer;

public class CharacterPortraitSpeaker : MonoBehaviour
{
    [SerializeField] private Image imageKitPortraitBase;
    [SerializeField] private Image imageKitPortraitDetail;
    [SerializeField] private Image imageKitPortraitNeck;
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private KitPortraitLibrary kitPortraitLibrary;

    private PortraitSize _cachedSize;
    private string _cachedId;

    public void SetSpeaker(Speaker speaker)
    {
        if(speaker.SpeakerId == _cachedId) return;

        UpdateCharacterPortraitSprite(speaker);

        UpdateKitVisibility(speaker.HasKit);

        if(!speaker.HasKit) return;
        if(speaker.PortraitSize != _cachedSize)
            UpdateKitPortraitSprites(speaker);
        UpdateKitPortraitColors(speaker);
    }

    private void UpdateCharacterPortraitSprite(Speaker speaker)
    {
        imageCharacterPortrait.sprite = speaker.PortraitSprite;
        _cachedId = speaker.SpeakerId;
    }

    private void UpdateKitPortraitSprites(Speaker speaker)
    {
        var size = speaker.PortraitSize;
        var kitPortraitSprites = kitPortraitLibrary.Get(size);
        imageKitPortraitBase.sprite = kitPortraitSprites.SpriteBase;
        imageKitPortraitDetail.sprite = kitPortraitSprites.SpriteDetail;
        imageKitPortraitNeck.sprite = kitPortraitSprites.SpriteNeck;
        _cachedSize = size;
    }

    private void UpdateKitPortraitColors(Speaker speaker)
    {
        imageKitPortraitBase.color = speaker.SpriteLayerState.Colors[CharacterSpriteLayer.KitBase];
        imageKitPortraitDetail.color = speaker.SpriteLayerState.Colors[CharacterSpriteLayer.KitDetail];
        imageKitPortraitNeck.color = speaker.SpriteLayerState.Colors[CharacterSpriteLayer.Body];
    }

    private void UpdateKitVisibility(bool enable) 
    {
        imageKitPortraitBase.enabled = enable;
        imageKitPortraitDetail.enabled = enable;
        imageKitPortraitNeck.enabled = enable;
    }

}
