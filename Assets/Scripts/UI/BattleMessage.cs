using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Battle;

public class BattleMessage : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private LocalizeStringEvent localizeStringEvent;

    [Header("Localized Strings")]
    [SerializeField] private LocalizedString goalMessage;
    [SerializeField] private LocalizedString halfTimeMessage;
    [SerializeField] private LocalizedString fullTimeMessage;
    [SerializeField] private LocalizedString timeUpMessage;
    [SerializeField] private LocalizedString foulMessage;
    [SerializeField] private LocalizedString offsideMessage;

    private void Awake()
    {
        SetCanvasState(false);
        BattleUIManager.Instance?.RegisterBattleMessage(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterBattleMessage(this);
    }

    public void SetMessageActive(MessageType messageType, bool isActive)
    {
        messageText.color = ColorManager.GetBattleMessageColor(messageType);
        SetCanvasState(isActive);

        switch (messageType)
        {
            case MessageType.Goal:
                localizeStringEvent.StringReference = goalMessage;
                break;

            case MessageType.HalfTime:
                localizeStringEvent.StringReference = halfTimeMessage;
                break;

            case MessageType.FullTime:
                localizeStringEvent.StringReference = fullTimeMessage;
                break;

            case MessageType.TimeUp:
                localizeStringEvent.StringReference = timeUpMessage;
                break;

            case MessageType.Foul:
                localizeStringEvent.StringReference = foulMessage;
                break;

            case MessageType.Offside:
                localizeStringEvent.StringReference = offsideMessage;
                break;

            default:
                return;
        }
    }

    private void SetCanvasState(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = isVisible;
        canvasGroup.blocksRaycasts = isVisible;
    }

}
