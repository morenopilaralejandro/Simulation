using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneGameOver;
    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    private void Start() 
    {
        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;

        audioManager.PlayBgm("bgm-gameover");    
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    public void OnButtonContinueTapped() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        sceneLoader.LoadGroup(sceneMainMenu);
    }

}
