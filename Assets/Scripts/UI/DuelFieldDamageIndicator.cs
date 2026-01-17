using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Duel;
using Simulation.Enums.Move;

public class DuelFieldDamageIndicator : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textDamage;
    [SerializeField] private LocalizeStringEvent localizeStringEvent;

    [SerializeField] private LocalizedString offenseString;
    [SerializeField] private LocalizedString defenseString;
    
    void Start()
    {
        SetCanvasState(false);
    }

    private void OnEnable()
    {
        DuelEvents.OnDuelEnd += HandleDuelEnd;
    }

    private void OnDisable()
    {
        DuelEvents.OnDuelEnd -= HandleDuelEnd;
    }

    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser) 
    {
        if (duelMode == DuelMode.Field)
            SetCanvasState(false);
    }

    public void SetDamage(float damage, DuelAction action)
    {
        SetCanvasState(true);

        string damageString = 
            damage < 0 ? 
                "0" :
                Mathf.RoundToInt(damage).ToString();

        textDamage.text = damageString;

        localizeStringEvent.StringReference = 
            action == DuelAction.Offense ? 
                offenseString :
                defenseString;
    }

    private void SetCanvasState(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}
