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
    [SerializeField] private Image possessionImage;
    [SerializeField] private CharacterCard characterCard;
    [SerializeField] private List<CharacterCardMini> characterCardMiniList;
    [SerializeField] private DuelFieldDamageIndicator fieldDamageIndicator;

    public void SetSide(Character character, List<Character> supports)
    {
        HideMiniCards();
        hpBar.SetCharacter(character, Stat.Hp);
        spBar.SetCharacter(character, Stat.Sp);
        possessionImage.gameObject.SetActive(character.HasBall());
        characterCard.SetCharacter(character);
        if (supports != null)
            SetMiniCards(supports);
    }

    private void SetMiniCards(List<Character> supports) 
    {
        for (int i = 0; i < supports.Count; i++) 
        {
            Character character = supports[i];
            characterCardMiniList[i].gameObject.SetActive(true);
            characterCardMiniList[i].SetCharacter(character);
        }
    }

    private void HideMiniCards() 
    {
        foreach (var card in characterCardMiniList)
            card.gameObject.SetActive(false);
    }

    public void SetFieldDamage(float damage, DuelAction action) => fieldDamageIndicator.SetDamage(damage, action);

}
