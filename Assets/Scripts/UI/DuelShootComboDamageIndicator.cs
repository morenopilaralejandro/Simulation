using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Localization;
using Simulation.Enums.Duel;

public class DuelShootComboDamageIndicator : MonoBehaviour
{
    [SerializeField] private GameObject panelIndicator;
    [SerializeField] private TMP_Text textDamage;

    private bool isOpen = false;
    public bool IsOpen => isOpen;
    
    void Start()
    {
        SetActive(false);
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
        if (duelMode == DuelMode.Shoot)
            Toggle();
    }

    private void HandleDuelEnd(DuelMode duelMode, DuelParticipant winner, DuelParticipant loser, bool isWinnerUser) 
    {
        if (duelMode == DuelMode.Shoot && isOpen)
            Toggle();
    }

    private void HandleDuelCancel(DuelMode duelMode) 
    {
        if (duelMode == DuelMode.Shoot && isOpen)
            Toggle();
    }

    public void SetDamage(float damage)
    {
        if(!isOpen) Toggle();

        string damageString = 
            damage < 0 ? 
                "0" :
                Mathf.RoundToInt(damage).ToString();

        textDamage.text = damageString;
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        SetActive(isOpen);
        Debug.LogWarning("toggle");
    }

    public void SetActive(bool active) 
    {
        panelIndicator.SetActive(active);
    }
}
