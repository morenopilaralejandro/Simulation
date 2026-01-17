using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using Simulation.Enums.Duel;

public class DuelShootComboDamageIndicator : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text textDamage;
    
    void Start()
    {
        SetCanvasState(false);
    }

    private void OnEnable()
    {
        //DuelEvents.OnDuelStart += HandleDuelStart;
        DuelEvents.OnDuelEnd += HandleDuelEnd;
        DuelEvents.OnDuelCancel += HandleDuelCancel;
    }

    private void OnDisable()
    {
        //DuelEvents.OnDuelStart -= HandleDuelStart;
        DuelEvents.OnDuelEnd -= HandleDuelEnd;
        DuelEvents.OnDuelCancel -= HandleDuelCancel;
    }

    private void HandleDuelStart(DuelMode duelMode) 
    {
        //if (duelMode == DuelMode.Shoot)
    }

    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser) 
    {
        if (duelMode == DuelMode.Shoot)
            SetCanvasState(false);
    }

    private void HandleDuelCancel(DuelMode duelMode) 
    {
        if (duelMode == DuelMode.Shoot)
            SetCanvasState(false);
    }

    public void SetDamage(float damage)
    {
        SetCanvasState(true);

        string damageString = 
            damage < 0 ? 
                "0" :
                Mathf.RoundToInt(damage).ToString();

        textDamage.text = damageString;
    }

    private void SetCanvasState(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }
}
