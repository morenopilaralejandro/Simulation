using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// UI element representing a single team loadout in the list.
/// </summary>
public class LoadoutListItem : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button selectButton;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject activeIndicator;
    //[SerializeField] private Text characterCountText;      // e.g., "5 / 8"
    private Action<Team> onClickCallback;
    private Team team;

    public void Initialize(Team team, bool isActive, Action<Team> onClick)
    {
        this.team = team;
        this.onClickCallback = onClick;

        // Populate UI
        if (nameText != null)
            nameText.text = team.TeamName;

        if (activeIndicator != null)
            activeIndicator.SetActive(isActive);

        if (selectButton != null)
            selectButton.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (selectButton != null)
            selectButton.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        // Invokes the one pased in Initialize.
        onClickCallback?.Invoke(team);
    }
}
