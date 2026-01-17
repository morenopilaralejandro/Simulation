using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;
using Simulation.Enums.Move;

public class DuelParticantsPanel : MonoBehaviour
{
    [SerializeField] private Image categoryImage;
    [SerializeField] private DuelSide duelSideHome;
    [SerializeField] private DuelSide duelSideAway;
    [SerializeField] private DuelShootComboDamageIndicator comboDamageIndicator;

    private Dictionary<TeamSide, DuelSide> duelSideDict;

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterDuelParticipantsPanel(this);

        //flip so user is always on the left
        if (BattleManager.Instance.GetUserSide() == TeamSide.Home) 
        {
            duelSideDict = new Dictionary<TeamSide, DuelSide>
            {
                { TeamSide.Home, duelSideHome },
                { TeamSide.Away, duelSideAway }
            };
        } else {
            duelSideDict = new Dictionary<TeamSide, DuelSide>
            {
                { TeamSide.Away, duelSideHome },
                { TeamSide.Home, duelSideAway }
            };
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterDuelParticipantsPanel(this);
    }

    public void Show() 
    {
        this.gameObject.SetActive(true);
    }

    public void Hide() 
    {
        this.gameObject.SetActive(false);
    }

    public void SetSide(Character character, List<Character> supports) 
    {
        duelSideDict[character.TeamSide].SetSide(character, supports);
    }

    public void SetCategory(Category category) => categoryImage.sprite = IconManager.Instance.Category.GetIcon(category);
    public void SetComboDamage(float damage) => comboDamageIndicator.SetDamage(damage);
    public void SetFieldDamage(Character character, float damage, DuelAction action) => duelSideDict[character.TeamSide].SetFieldDamage(damage, action);

}
