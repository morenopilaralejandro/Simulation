using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Simulation.Enums.Move;

public class DuelLogPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject panelPortraitCharacter;
    [SerializeField] private GameObject panelPortraitDefault;
    [SerializeField] private GameObject panelEvolution;
    [SerializeField] private CharacterPortraitBattle characterPortrait;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private Image imageBackground;

    public float SpawnTime { get; private set; }

    private void Configure(DuelLogEntry entry)
    {
        messageText.text = entry.EntryString;
        if (entry.Character != null)
            characterPortrait.SetCharacter(entry.Character);
        if (entry.Move != null && entry.Move.CurrentEvolution != MoveEvolution.None) 
        {
            imageEvolution.sprite = entry.Move.EvolutionSprite;
        }
    }

    private void UpdateActivePanels(DuelLogEntry entry)
    {
        bool hasCharacter = entry.Character != null;
        bool hasMove = entry.Move != null && entry.Move.CurrentEvolution != MoveEvolution.None;

        panelPortraitCharacter.SetActive(hasCharacter);
        panelPortraitDefault.SetActive(!hasCharacter);
        panelEvolution.SetActive(hasMove);

        UpdateBackgroundColor(hasCharacter, entry.Character);
    }

    private void UpdateBackgroundColor(bool hasCharacter, Character character) 
    {
        if (!hasCharacter) 
        {
            imageBackground.color = DuelLogManager.Instance.ColorDefault;
            return;
        }

        if(character.TeamSide == BattleManager.Instance.GetUserSide())
            imageBackground.color = DuelLogManager.Instance.ColorHome;
        else
            imageBackground.color = DuelLogManager.Instance.ColorAway;
    }

    public void Show(DuelLogEntry entry)
    {
        Configure(entry);
        UpdateActivePanels(entry);

        SpawnTime = Time.unscaledTime;
        gameObject.SetActive(true);
    }

    // Used by the menu (no timer)
    public void ShowStatic(DuelLogEntry entry)
    {
        Configure(entry);
        UpdateActivePanels(entry);

        gameObject.SetActive(true);
    }
}
