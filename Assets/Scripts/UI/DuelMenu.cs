using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;
using Simulation.Enums.Duel;

public class DuelMenu : MonoBehaviour
{

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterDuelMenu(this);
        Hide();
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterDuelMenu(this);
    }

    public void Show() 
    {
        this.gameObject.SetActive(true);
    }

    public void Hide() 
    {
        this.gameObject.SetActive(false);
    }

    public void OnCommandMeleeTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxMenuTap");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            BattleManager.Instance.GetUserSide(), 
            DuelCommand.Melee, 
            null);
    }

    public void OnCommandRangedTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxMenuTap");
        DuelSelectionManager.Instance.SelectionMadeHuman(
            BattleManager.Instance.GetUserSide(), 
            DuelCommand.Ranged, 
            null);
    }


    public void OnCommandMoveTapped()
    {
        //AudioManager.Instance.PlaySfx("SfxSecretCommand");
        //SetPanelMoveVisible(true);
        //SetPanelCommandVisible(false);
        /*
        //TODO user
        DuelSelectionManager.Instance.GetUserCategory();
        DuelSelectionManager.Instance.GetUserCharacter();
        var localSel = _duelSelections[GameManager.Instance.GetLocalTeamIndex()];
        if (localSel.Player != null && panelSecret != null)
        {
            var secretPanel = panelSecret.GetComponent<SecretPanel>();
            if (secretPanel != null)
                secretPanel.UpdateSecretSlots(localSel.Player.CurrentSecret, localSel.Player.GetStat(PlayerStats.Sp), localSel.Category);
        }
        */
    }

    /*
    public void OnMoveSlotTapped(SecretCommandSlot secretCommandSlot)
    {
        if (secretCommandSlot?.Secret == null) return;
        var localSel = _duelSelections[GameManager.Instance.GetLocalTeamIndex()];
        if (localSel.Player.GetStat(PlayerStats.Sp) < secretCommandSlot.Secret.Cost)
        {
            AudioManager.Instance.PlaySfx("SfxForbidden");
            return;
        }

        SetPanelSecretVisible(false);
        SetPanelCommandVisible(false);

        AudioManager.Instance.PlaySfx("SfxSecretSelect");
        DuelSelectionMade(GameManager.Instance.GetLocalTeamIndex(), DuelCommand.Secret, secretCommandSlot.Secret);
        DuelSelectionManager.Instance.SelectionMadeHuman
    }
    */

}
