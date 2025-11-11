using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

public class DebugBattleUI : MonoBehaviour
{
    [SerializeField] private GameObject panelMaximized;
    [SerializeField] private GameObject panelMinimized;

    void OnEnable() 
    {
        BattleManager.Instance.OnAllCharactersReady += ToggleAI;
    }

    void Disable() 
    {
        BattleManager.Instance.OnAllCharactersReady -= ToggleAI;
    }

    void Start()
    {
        Minimize();
    }

    public void OnButtonMaximizeTapped() => Maximize();
    public void OnButtonMinimizeTapped() => Minimize();
    public void OnButtonToggleAITapped() => ToggleAI();
    public void OnButtonRestartBattleTapped() => RestartBattle();

    private void Maximize() 
    {
        panelMinimized.SetActive(false);
        panelMaximized.SetActive(true);
    }

    private void Minimize() 
    {
        panelMaximized.SetActive(false);
        panelMinimized.SetActive(true);
    }

    private void RestartBattle() 
    {
        BattleManager.Instance.StartBattle();
    }

    private void ToggleAI() 
    {
        foreach (
            Character character in 
            BattleManager.Instance.Teams[TeamSide.Away].CharacterList
        ) 
            character.EnableAI(!character.IsAIEnabled);
    }

}
