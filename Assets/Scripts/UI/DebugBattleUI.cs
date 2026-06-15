using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public class DebugBattleUI : MonoBehaviour
{
    [SerializeField] private GameObject panelMaximized;
    [SerializeField] private GameObject panelMinimized;
    [SerializeField] private SceneGroup debugMainMenu;
    [SerializeField] private bool isEnemyAiEnabled = false;

    void OnEnable() 
    {
        BattleEvents.OnAllCharactersReady += UpdateEnemyAI;
    }

    void Disable() 
    {
        BattleEvents.OnAllCharactersReady -= UpdateEnemyAI;
    }

    void Start()
    {
        Minimize();
    }

    public void OnButtonMaximizeTapped() => Maximize();
    public void OnButtonMinimizeTapped() => Minimize();
    public void OnButtonToggleAITapped() => ToggleAI();
    public void OnButtonExitBattleTapped() => ExitBattle();

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

    private void ExitBattle() 
    {
        BattleManager.Instance.SetBattlePhase(BattlePhase.End);
        BattleEvents.RaiseBattleEnd();
        SceneLoader.Instance.LoadGroup(debugMainMenu);
    }

    private void ToggleAI() 
    {
        isEnemyAiEnabled = !isEnemyAiEnabled;
        UpdateEnemyAI();
    }

    private void UpdateEnemyAI() 
    {
        foreach (
            CharacterEntityBattle character in 
            BattleManager.Instance.Teams[TeamSide.Away].GetCharacterEntities(BattleManager.Instance.CurrentType)
        ) 
            character.EnableAI(isEnemyAiEnabled);
    }

}
