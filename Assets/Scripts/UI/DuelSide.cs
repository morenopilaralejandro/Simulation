using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;
using Simulation.Enums.Move;

public class DuelSide : MonoBehaviour
{
    [SerializeField] private BarHPSP hpBar;
    [SerializeField] private BarHPSP spBar;
    [SerializeField] private CanvasGroup canvasGroupPossession;
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private List<CharacterCardMini> characterCardMiniList;
    [SerializeField] private DuelFieldDamageIndicator fieldDamageIndicator;

    public void SetSide(Character character, List<Character> supports)
    {
        HideMiniCards();
        hpBar.SetCharacter(character, Stat.Hp);
        spBar.SetCharacter(character, Stat.Sp);
        SetPossessionCanvasState(character.HasBall());
        characterCard.SetCharacter(character);
        if (supports != null)
            SetMiniCards(supports);
    }

    private void SetMiniCards(List<Character> supports) 
    {
        for (int i = 0; i < supports.Count; i++) 
        {
            Character character = supports[i];
            characterCardMiniList[i].SetCanvasState(true);
            characterCardMiniList[i].SetCharacter(character);
        }
    }

    private void HideMiniCards() 
    {
        foreach (var card in characterCardMiniList)
            card.SetCanvasState(false);
    }

    public void SetPossessionCanvasState(bool isVisible)
    {
        canvasGroupPossession.alpha = isVisible ? 1f : 0f;
        canvasGroupPossession.interactable = isVisible;
        canvasGroupPossession.blocksRaycasts = isVisible;
    }

    public void SetFieldDamage(float damage, DuelAction action) => fieldDamageIndicator.SetDamage(damage, action);

}
