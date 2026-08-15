using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Move;

public class DuelParticipantsPanel : MonoBehaviour
{
    #region Field

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image categoryImage;
    [SerializeField] private DuelSide duelSideHome;
    [SerializeField] private DuelSide duelSideAway;
    [SerializeField] private DuelShootComboDamageIndicator comboDamageIndicator;

    private Dictionary<TeamSide, DuelSide> duelSideDict;

    #endregion

    #region LifeCycle

    private void Start()
    {
        Hide();

        if (BattleManager.Instance == null) return;

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
    }

    #endregion

    #region Logic

    private void Show() => SetVisibility(true);
    private void Hide() => SetVisibility(false);

    private void SetVisibility(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }



    private void SetSide(CharacterEntityBattle character, List<CharacterEntityBattle> supports) => duelSideDict[character.TeamSide].SetSide(character, supports);
    private void SetCategory(Category category) => categoryImage.sprite = IconManager.Instance.Category.GetIcon(category);
    private void SetComboDamage(float damage) => comboDamageIndicator.SetDamage(damage);
    //private void SetFieldDamage(CharacterEntityBattle character, float damage, DuelAction action) => duelSideDict[character.TeamSide].SetFieldDamage(damage, action);

    #endregion

    #region Event

    private void OnEnable()
    {
        UIEvents.OnDuelParticipantShowRequested += HandleDuelParticipantShowRequested;
        UIEvents.OnDuelParticipantHideRequested += HandleDuelParticipantHideRequested;
        UIEvents.OnDuelParticipantSetSideRequested += HandleDuelParticipantSetSideRequested;
        UIEvents.OnDuelParticipantSetCategoryRequested += HandleDuelParticipantSetCategoryRequested;
        UIEvents.OnDuelParticipantSetComboDamageRequested += HandleDuelParticipantSetComboDamageRequested;
    }

    private void OnDisable()
    {
        UIEvents.OnDuelParticipantShowRequested -= HandleDuelParticipantShowRequested;
        UIEvents.OnDuelParticipantHideRequested -= HandleDuelParticipantHideRequested;
        UIEvents.OnDuelParticipantSetSideRequested -= HandleDuelParticipantSetSideRequested;
        UIEvents.OnDuelParticipantSetCategoryRequested -= HandleDuelParticipantSetCategoryRequested;
        UIEvents.OnDuelParticipantSetComboDamageRequested -= HandleDuelParticipantSetComboDamageRequested;
    }

    private void HandleDuelParticipantShowRequested()
    {
        Show();
    }

    private void HandleDuelParticipantHideRequested()
    {
        Hide();
    }

    private void HandleDuelParticipantSetSideRequested(
        CharacterEntityBattle character,
        List<CharacterEntityBattle> supports)
    {
        SetSide(character, supports);
    }

    private void HandleDuelParticipantSetCategoryRequested(
        Category category)
    {
        SetCategory(category);
    }

    private void HandleDuelParticipantSetComboDamageRequested(
        float damage)
    {
        SetComboDamage(damage);
    }

    #endregion

}
