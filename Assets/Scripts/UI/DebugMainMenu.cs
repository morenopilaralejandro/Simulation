using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

public class DebugMainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnButton1Tapped() {
        HandleButton1();
    }

    private void HandleButton1() {
        BattleArgs.TeamId0 = "faith";
        BattleArgs.TeamId1 = "crimson";
        BattleArgs.BattleType = BattleType.Battle;

        SceneLoader.UnloadDebugMainMenu();
        SceneLoader.LoadBattle();
    }
}
