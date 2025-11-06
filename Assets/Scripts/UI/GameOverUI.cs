using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;

    private void Start() 
    {
        AudioManager.Instance.PlayBgm("bgm-gameover");    
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    public void OnButtonContinueTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        SceneLoader.UnloadGameOver();
        SceneLoader.LoadMainMenu();
    }

}
