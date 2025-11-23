using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePanel : MonoBehaviour
{
    [SerializeField] private bool isOpen;
    public bool IsOpen => isOpen;

    private void Awake()
    {
        SetActive(false);
        BattleUIManager.Instance.RegisterPausePanel(this);
        BattleEvents.OnPauseBattle += Toggle;
        BattleEvents.OnResumeBattle += Toggle;
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterPausePanel(this);
        BattleEvents.OnPauseBattle -= Toggle;
        BattleEvents.OnResumeBattle -= Toggle;
    }

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
