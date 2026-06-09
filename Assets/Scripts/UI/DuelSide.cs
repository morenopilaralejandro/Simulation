using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Move;

public class DuelSide : MonoBehaviour
{
    [SerializeField] private BarHPSP hpBar;
    [SerializeField] private BarHPSP spBar;
    [SerializeField] private Image imagePossession;
    [SerializeField] private Image imageFatigue;
    [SerializeField] private Image imageWing;
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private List<CharacterCardMini> characterCardMiniList;
    [SerializeField] private DuelFieldDamageIndicator fieldDamageIndicator;
    [SerializeField] private Sprite spriteFatigueTired;
    [SerializeField] private Sprite spriteFatigueExhausted;
    [SerializeField] private Sprite spriteWingBoth;
    [SerializeField] private Sprite spriteWingOne;

    public void SetSide(CharacterEntityBattle character, List<CharacterEntityBattle> supports)
    {
        HideMiniCards();
        hpBar.SetCharacter(character.Character, Stat.Hp);
        spBar.SetCharacter(character.Character, Stat.Sp);
        SetPossession(character);
        SetFatigue(character);
        SetWing(character);
        characterCard.SetCharacter(character.Character, character.FormationCoord.Position);
        if (supports != null)
            SetMiniCards(supports);
    }

    private void SetMiniCards(List<CharacterEntityBattle> supports) 
    {
        for (int i = 0; i < supports.Count; i++) 
        {
            CharacterEntityBattle character = supports[i];
            characterCardMiniList[i].SetCanvasState(true);
            characterCardMiniList[i].SetCharacter(character.Character, character.FormationCoord.Position);
        }
    }

    private void HideMiniCards() 
    {
        foreach (var card in characterCardMiniList)
            card.SetCanvasState(false);
    }

    public void SetPossession(CharacterEntityBattle character)
    {
        imagePossession.enabled = character.HasBall();
    }

    public void SetFatigue(CharacterEntityBattle character)
    {
        imageFatigue.enabled = character.FatigueState != FatigueState.Normal;
        if (character.FatigueState == FatigueState.Normal) return;
        
        if(character.FatigueState == FatigueState.Tired)
            imageFatigue.sprite = spriteFatigueTired;        
        else
            imageFatigue.sprite = spriteFatigueExhausted;
    }

    public void SetWing(CharacterEntityBattle character)
    {
        imageWing.enabled = character.HasWingActivated;
        if (!character.HasWingActivated) return;

        if(character.WingTimesUsed == 0)
            imageWing.sprite = spriteWingBoth;        
        else
            imageWing.sprite = spriteWingOne;
    }

    public void SetFieldDamage(float damage, DuelAction action) => fieldDamageIndicator.SetDamage(damage, action);

}
