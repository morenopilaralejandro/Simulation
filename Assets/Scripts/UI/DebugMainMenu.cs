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

    public void OnButton2Tapped() {
        HandleButton2();
    }

    private void HandleButton1() {
        BattleArgs.SetFull(
            "faith_selection", 
            "crimson_selection");

        SceneLoader.UnloadDebugMainMenu();
        SceneLoader.LoadBattle();
    }

    private void HandleButton2() {
        BattleArgs.SetMini(
            "faith_selection", 
            "crimson_selection");

        SceneLoader.UnloadDebugMainMenu();
        SceneLoader.LoadBattle();
    }
}
