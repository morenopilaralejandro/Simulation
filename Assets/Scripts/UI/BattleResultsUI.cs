using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class BattleResultsUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelected;

    private void Start() 
    {
        AudioManager.Instance.PlayBgm("bgm-fanfare");    
        EventSystem.current.SetSelectedGameObject(defaultSelected);
    }

    public void OnButtonContinueTapped() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        SceneLoader.UnloadBattleResults();
        SceneLoader.LoadMainMenu();
    }

}
