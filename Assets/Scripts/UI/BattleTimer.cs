using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Battle;

public class BattleTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text textTimer;
    [SerializeField] private TMP_Text textFirstHalf;
    [SerializeField] private TMP_Text textSecondHalf;

    [SerializeField] private LocalizationComponentUIText localizationFirstHalf;
    [SerializeField] private LocalizationComponentUIText localizationSecondHalf;

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterTimer(this);
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterTimer(this);
    }

    public void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int secs = Mathf.FloorToInt(time % 60f);
        textTimer.text = string.Format("{0:00}:{1:00}", minutes, secs);
    }

    public void UpdateTimerHalfDisplay(TimerHalf timerHalf)
    {
        bool isFirstHalf = timerHalf == TimerHalf.First;
        textFirstHalf.gameObject.SetActive(isFirstHalf);
        textSecondHalf.gameObject.SetActive(!isFirstHalf);
    }
}
