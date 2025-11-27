using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        SetActive(false);
        BattleUIManager.Instance.RegisterPausePanel(this);
        BattleEvents.OnBattlePause += HandleBattlePause;
        BattleEvents.OnBattleResume += HandleBattleResume;
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterPausePanel(this);
        BattleEvents.OnBattlePause -= HandleBattlePause;
        BattleEvents.OnBattleResume -= HandleBattleResume;
    }

    private void HandleBattlePause(TeamSide teamSide) => Toggle();
    private void HandleBattleResume() => Toggle();

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
