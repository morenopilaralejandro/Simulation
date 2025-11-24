using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DuelLogPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private GameObject panelPortraitCharacter;
    [SerializeField] private GameObject panelPortraitDefault;
    [SerializeField] private GameObject panelEvolution;
    [SerializeField] private CharacterPortrait characterPortrait;
    [SerializeField] private Image imageEvolution;

    public float SpawnTime { get; private set; }

    private void Configure(DuelLogEntry entry)
    {
        messageText.text = entry.EntryString;
        if (entry.Character != null)
            characterPortrait.SetCharacter(entry.Character);
        if (entry.Move != null)
            imageEvolution.sprite = entry.Move.EvolutionSprite;
    }

    private void UpdateActivePanels(DuelLogEntry entry)
    {
        bool hasCharacter = entry.Character != null;
        bool hasMove = entry.Move != null;

        panelPortraitCharacter.SetActive(hasCharacter);
        panelPortraitDefault.SetActive(!hasCharacter);
        panelEvolution.SetActive(hasMove);
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
