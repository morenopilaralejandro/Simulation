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
    [SerializeField] private TMP_Text textValueTrueKick;
    [SerializeField] private TMP_Text textValueTrainedKick;

    [Header("UI References - Body")]
    [SerializeField] private TMP_Text textValueTrueBody;
    [SerializeField] private TMP_Text textValueTrainedBody;

    [Header("UI References - Control")]
    [SerializeField] private TMP_Text textValueTrueControl;
    [SerializeField] private TMP_Text textValueTrainedControl;

    [Header("UI References - Guard")]
    [SerializeField] private TMP_Text textValueTrueGuard;
    [SerializeField] private TMP_Text textValueTrainedGuard;

    [Header("UI References - Speed")]
    [SerializeField] private TMP_Text textValueTrueSpeed;
    [SerializeField] private TMP_Text textValueTrainedSpeed;

    [Header("UI References - Stamina")]
    [SerializeField] private TMP_Text textValueTrueStamina;
    [SerializeField] private TMP_Text textValueTrainedStamina;

    [Header("UI References - Courage")]
    [SerializeField] private TMP_Text textValueTrueCourage;
    [SerializeField] private TMP_Text textValueTrainedCourage;

    [Header("UI References - Freedom")]
    [SerializeField] private TMP_Text textValueTrueFreedom;
    [SerializeField] private TMP_Text textValueTrainedFreedom;

    private Character character;

    #endregion

    #region Lifecycle

    private void Awake()
    {
    }

    private void Start()
    {
    }

    #endregion

    #region Initialize

    public void Initialize(Character character)
    {
        this.character = character;

        Clear();
    }

    public void Clear()
    {
        textValueTrueKick.text      = "";
        textValueTrainedKick.text   = "";

        textValueTrueBody.text      = "";
        textValueTrainedBody.text   = "";

        textValueTrueControl.text   = "";
        textValueTrainedControl.text = "";

        textValueTrueGuard.text     = "";
        textValueTrainedGuard.text  = "";

        textValueTrueSpeed.text     = "";
        textValueTrainedSpeed.text  = "";

        textValueTrueStamina.text   = "";
        textValueTrainedStamina.text = "";

        textValueTrueCourage.text   = "";
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
        textValueTrueKick.text    = character.GetTrueStat(Stat.Kick).ToString();
        textValueTrainedKick.text = $"({character.GetTrainedStat(Stat.Kick)})";

        // Body
        textValueTrueBody.text    = character.GetTrueStat(Stat.Body).ToString();
        textValueTrainedBody.text = $"({character.GetTrainedStat(Stat.Body)})";

        // Control
        textValueTrueControl.text    = character.GetTrueStat(Stat.Control).ToString();
        textValueTrainedControl.text = $"({character.GetTrainedStat(Stat.Control)})";

        // Guard
        textValueTrueGuard.text    = character.GetTrueStat(Stat.Guard).ToString();
        textValueTrainedGuard.text = $"({character.GetTrainedStat(Stat.Guard)})";

        // Speed
        textValueTrueSpeed.text    = character.GetTrueStat(Stat.Speed).ToString();
        textValueTrainedSpeed.text = $"({character.GetTrainedStat(Stat.Speed)})";

        // Stamina
        textValueTrueStamina.text    = character.GetTrueStat(Stat.Stamina).ToString();
        textValueTrainedStamina.text = $"({character.GetTrainedStat(Stat.Stamina)})";

        // Courage
        textValueTrueCourage.text    = character.GetTrueStat(Stat.Courage).ToString();
        textValueTrainedCourage.text = $"({character.GetTrainedStat(Stat.Courage)})";

        // Freedom — true stat only, trained is left empty per spec
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
