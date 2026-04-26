using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public class DebugBattleUI : MonoBehaviour
{
    [SerializeField] private GameObject panelMaximized;
    [SerializeField] private GameObject panelMinimized;

    void OnEnable() 
    {
        BattleEvents.OnAllCharactersReady += ToggleAI;
    }

    void Disable() 
    {
        BattleEvents.OnAllCharactersReady -= ToggleAI;
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
            CharacterEntityBattle character in 
            BattleManager.Instance.Teams[TeamSide.Away].GetCharacterEntities(BattleManager.Instance.CurrentType)
        ) 
            character.EnableAI(!character.IsAIEnabled);
    }

}
