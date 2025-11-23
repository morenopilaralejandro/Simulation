using UnityEngine;
using System;

public class AutoBattleManager : MonoBehaviour
{
    public static AutoBattleManager Instance { get; private set; }
    public bool IsAutoBattleEnabled { get; private set; }

    private const string AUTO_BATTLE_PREF_KEY = "AutoBattleEnabled";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadPreference();
    }

    public void ToggleAutoBattle(bool enable)
    {
        if (IsAutoBattleEnabled == enable)
            return;

        IsAutoBattleEnabled = enable;
        SavePreference();
        SettingsEvents.RaiseAutoBattleToggled(enable);
    }

    public void ToggleAutoBattle()
    {
        ToggleAutoBattle(!IsAutoBattleEnabled);
    }

    private void LoadPreference()
    {
        // PlayerPrefs uses int (0 = false, 1 = true)
        IsAutoBattleEnabled = PlayerPrefs.GetInt(AUTO_BATTLE_PREF_KEY, 0) == 1;
    }

    private void SavePreference()
    {
        PlayerPrefs.SetInt(AUTO_BATTLE_PREF_KEY, IsAutoBattleEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }
}
