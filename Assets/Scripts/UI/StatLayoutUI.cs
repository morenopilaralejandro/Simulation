using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.UI;
using Aremoreno.Enums.Move;

public class StatLayoutUI : MonoBehaviour
{
    #region Fields

    [Header("UI References - Kick")]
    [SerializeField] private TMP_Text textValueBattleKick;
    [SerializeField] private TMP_Text textValueTrainedKick;

    [Header("UI References - Body")]
    [SerializeField] private TMP_Text textValueBattleBody;
    [SerializeField] private TMP_Text textValueTrainedBody;

    [Header("UI References - Control")]
    [SerializeField] private TMP_Text textValueBattleControl;
    [SerializeField] private TMP_Text textValueTrainedControl;

    [Header("UI References - Guard")]
    [SerializeField] private TMP_Text textValueBattleGuard;
    [SerializeField] private TMP_Text textValueTrainedGuard;

    [Header("UI References - Speed")]
    [SerializeField] private TMP_Text textValueBattleSpeed;
    [SerializeField] private TMP_Text textValueTrainedSpeed;

    [Header("UI References - Stamina")]
    [SerializeField] private TMP_Text textValueBattleStamina;
    [SerializeField] private TMP_Text textValueTrainedStamina;

    [Header("UI References - Courage")]
    [SerializeField] private TMP_Text textValueBattleCourage;
    [SerializeField] private TMP_Text textValueTrainedCourage;

    [Header("UI References - Freedom")]
    [SerializeField] private TMP_Text textValueTrueFreedom;
    [SerializeField] private TMP_Text textValueTrainedFreedom;

    private Character character;

    #endregion

    #region Lifecycle

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;

        Clear();
    }

    public void Clear()
    {
        textValueBattleKick.text      = "";
        textValueTrainedKick.text   = "";

        textValueBattleBody.text      = "";
        textValueTrainedBody.text   = "";

        textValueBattleControl.text   = "";
        textValueTrainedControl.text = "";

        textValueBattleGuard.text     = "";
        textValueTrainedGuard.text  = "";

        textValueBattleSpeed.text     = "";
        textValueTrainedSpeed.text  = "";

        textValueBattleStamina.text   = "";
        textValueTrainedStamina.text = "";

        textValueBattleCourage.text   = "";
        textValueTrainedCourage.text = "";

        textValueTrueFreedom.text   = "";
        textValueTrainedFreedom.text = "";
    }

    #endregion

    #region Helpers

    public void Populate()
    {
        if (character == null) return;

        // Kick
        textValueBattleKick.text    = character.GetBattleStat(Stat.Kick).ToString();
        textValueTrainedKick.text = $"({character.GetTrainedStat(Stat.Kick)})";

        // Body
        textValueBattleBody.text    = character.GetBattleStat(Stat.Body).ToString();
        textValueTrainedBody.text = $"({character.GetTrainedStat(Stat.Body)})";

        // Control
        textValueBattleControl.text    = character.GetBattleStat(Stat.Control).ToString();
        textValueTrainedControl.text = $"({character.GetTrainedStat(Stat.Control)})";

        // Guard
        textValueBattleGuard.text    = character.GetBattleStat(Stat.Guard).ToString();
        textValueTrainedGuard.text = $"({character.GetTrainedStat(Stat.Guard)})";

        // Speed
        textValueBattleSpeed.text    = character.GetBattleStat(Stat.Speed).ToString();
        textValueTrainedSpeed.text = $"({character.GetTrainedStat(Stat.Speed)})";

        // Stamina
        textValueBattleStamina.text    = character.GetBattleStat(Stat.Stamina).ToString();
        textValueTrainedStamina.text = $"({character.GetTrainedStat(Stat.Stamina)})";

        // Courage
        textValueBattleCourage.text    = character.GetBattleStat(Stat.Courage).ToString();
        textValueTrainedCourage.text = $"({character.GetTrainedStat(Stat.Courage)})";

        // Freedom — Battle stat only, trained is left empty per spec
        textValueTrueFreedom.text    = character.TrueFreedom.ToString();
        textValueTrainedFreedom.text = "";
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        if (character != null)
        {
            Populate();
        }
    }

    private void OnDisable()
    {
        Clear();
    }

    #endregion
}
