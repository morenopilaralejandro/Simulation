using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        SetActive(false);
        BattleUIManager.Instance.RegisterPausePanel(this);
        BattleEvents.OnBattlePaused += HandleBattlePaused;
        BattleEvents.OnBattleResumed += HandleBattleResumed;
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterPausePanel(this);
        BattleEvents.OnBattlePaused -= HandleBattlePaused;
        BattleEvents.OnBattleResumed -= HandleBattleResumed;
    }

    private void HandleBattlePaused(TeamSide teamSide) => Toggle();
    private void HandleBattleResumed() => Toggle();

    public void Toggle()
    {
        isOpen = !isOpen;
        SetActive(isOpen);
    }

    public void SetActive(bool active)
    {
        this.gameObject.SetActive(active);
    }
}
