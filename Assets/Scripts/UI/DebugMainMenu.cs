using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;

public class DebugMainMenu : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneBattle;
    [SerializeField] private SceneGroup sceneDebugMainMenu;
    [SerializeField] private SceneGroup sceneWorld;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;
    }

    public void OnButton1Tapped() {
        HandleButton1();
    }

    public void OnButton2Tapped() {
        HandleButton2();
    }

    public void OnButton3Tapped() {
        HandleButton3();
    }

    private void HandleButton1() {
        BattleArgs.SetFull(
            "faith_selection", 
            "crimson_selection");
        sceneLoader.LoadGroup(sceneBattle);
    }

    private void HandleButton2() {
        BattleArgs.SetMini(
            "faith_selection", 
            "crimson_selection");
        sceneLoader.LoadGroup(sceneBattle);
    }

    private void HandleButton3() {
        CharacterManager.Instance.FirstTimeInitialize();
        sceneLoader.LoadGroup(sceneWorld);
    }
}
