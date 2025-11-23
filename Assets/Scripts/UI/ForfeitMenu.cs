using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using Simulation.Enums.Input;

public class ForfeitMenu : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private bool isForfeitMenuOpen;

    public bool IsForfeitMenuOpen => isForfeitMenuOpen;

    private void Awake()
    {
        BattleUIManager.Instance.RegisterForfeitMenu(this);
    }

    private void OnDestroy()
    {
        BattleUIManager.Instance.UnregisterForfeitMenu(this);
    }

    void Start()
    {
        SetForfeitMenuActive(false);
    }

    public void ToggleForfeitMenu()
    {
        isForfeitMenuOpen = !isForfeitMenuOpen;
        SetForfeitMenuActive(isForfeitMenuOpen);

        if (isForfeitMenuOpen) 
            EventSystem.current.SetSelectedGameObject(firstSelected);

    }

    public void SetForfeitMenuActive(bool active)
    {
        this.gameObject.SetActive(active);
    }

    public void OnButtonConfimTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        BattleManager.Instance.ForfeitBattle();
        ToggleForfeitMenu();
    }

    public void OnButtonCancelTapped()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        ToggleForfeitMenu();
    }

    public void OnButtonSelected() 
    {
        AudioManager.Instance.PlaySfx("sfx-menu_change");
    }

}
