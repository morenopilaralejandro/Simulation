using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;

public class BattleMessage : MonoBehaviour
{
    [SerializeField] private GameObject panelGoal;
    [SerializeField] private GameObject panelHalfTime;
    [SerializeField] private GameObject panelFullTime;
    [SerializeField] private GameObject panelTimeUp;
    [SerializeField] private GameObject panelFoul;
    [SerializeField] private GameObject panelOffside;

    private void Awake()
    {
        panelGoal.SetActive(false);
        panelHalfTime.SetActive(false);
        panelFullTime.SetActive(false);
        BattleUIManager.Instance?.RegisterBattleMessage(this);
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterBattleMessage(this);
    }

    public void SetMessageActive(MessageType messageType, bool isActive)
    {
        switch (messageType)
        {
            case MessageType.Goal:
                panelGoal.SetActive(isActive);
                break;

            case MessageType.HalfTime:
                panelHalfTime.SetActive(isActive);
                break;

            case MessageType.FullTime:
                panelFullTime.SetActive(isActive);
                break;

            case MessageType.TimeUp:
                panelTimeUp.SetActive(isActive);
                break;

            case MessageType.Foul:
                panelFoul.SetActive(isActive);
                break;

            case MessageType.Offside:
                panelOffside.SetActive(isActive);
                break;

            default:
                LogManager.Warning($"[BattleMessage] Unhandled message type: {messageType}");
                break;
        }
    }

}
