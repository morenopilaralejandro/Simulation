using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Simulation.Enums.Battle;

public class BattleResultsUI : MonoBehaviour
{
    [SerializeField] private GameObject defaultSelectedDrop;
    [SerializeField] private GameObject defaultSelectedSummary;

    [Header("Scenes")]
    [SerializeField] private SceneGroup sceneMainMenu;
    [SerializeField] private SceneGroup sceneWorld;

    [Header("Panels")]
    [SerializeField] private GameObject panelDrop;
    [SerializeField] private GameObject panelSummary;

    private SceneLoader sceneLoader;
    private AudioManager audioManager;

    private void Awake() 
    {
        panelDrop.SetActive(false);
        panelSummary.SetActive(false);
    }

    private void Start() 
    {
        sceneLoader = SceneLoader.Instance;
        audioManager = AudioManager.Instance;

        audioManager.PlayBgm("bgm-fanfare");    
        switch (BattleArgs.BattleResultsType) 
        {
            case BattleResultsType.Drop:
                panelDrop.SetActive(true);
                EventSystem.current.SetSelectedGameObject(defaultSelectedDrop);
                break;
            default:
                panelSummary.SetActive(true);
                EventSystem.current.SetSelectedGameObject(defaultSelectedSummary);
                break;
        }

    }

    public void OnButtonTappedContinueFromSummary() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        sceneLoader.LoadGroup(sceneMainMenu);
    }

    public void OnButtonTappedContinueFromDrop() 
    {
        audioManager.PlaySfx("sfx-menu_tap");
        sceneLoader.LoadGroup(sceneWorld);
    }

}
