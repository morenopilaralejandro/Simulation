using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class BattleResultsUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneBattleResults;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    private void Start() 
    {
        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;

        audioManager.PlayBgm("bgm-fanfare");    
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    public void OnButtonContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        sceneLoader.LoadGroup(sceneMainMenu);
    }

}
