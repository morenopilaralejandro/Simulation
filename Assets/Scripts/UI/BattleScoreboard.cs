using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;

public class BattleScoreboard : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Score")]
    [SerializeField] private TMP_Text scoreTextHome;
    [SerializeField] private TMP_Text scoreTextAway;

    [Header("Team")]
    [SerializeField] private Image teamCrestHome;
    [SerializeField] private Image teamCrestAway;
    [SerializeField] private TMP_Text teamNameHome;
    [SerializeField] private TMP_Text teamNameAway;

    private Dictionary<TeamSide, TMP_Text> scoreTextDict;
    private Dictionary<TeamSide, Image> teamCrestDict;
    private Dictionary<TeamSide, TMP_Text> teamNameDict;

    private readonly Dictionary<TeamSide, AddressableBinding<Sprite>> _crestBindings = new();

    private void Awake()
    {
        BattleUIManager.Instance?.RegisterScoreboard(this);

        scoreTextDict = new Dictionary<TeamSide, TMP_Text>
        {
            { TeamSide.Home, scoreTextHome },
            { TeamSide.Away, scoreTextAway }
        };

        teamCrestDict = new Dictionary<TeamSide, Image>
        {
            { TeamSide.Home, teamCrestHome },
            { TeamSide.Away, teamCrestAway }
        };

        teamNameDict = new Dictionary<TeamSide, TMP_Text>
        {
            { TeamSide.Home, teamNameHome },
            { TeamSide.Away, teamNameAway }
        };

        _crestBindings[TeamSide.Home] = new AddressableBinding<Sprite>();
        _crestBindings[TeamSide.Away] = new AddressableBinding<Sprite>();
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterScoreboard(this);

        Clear();
    }

    public async Task SetTeamAsync(Team team)
    {
        TeamSide side = team.TeamSide;

        Sprite emblemSprite =
            await _crestBindings[side]
                .LoadAsync(team.Emblem.EmblemAddress);

        teamCrestDict[side].sprite = emblemSprite;
        teamNameDict[side].text = team.TeamName;
    }

    public void UpdateScoreDisplay(Team team, int scoreValue)
    {
        scoreTextDict[team.TeamSide].text = scoreValue.ToString();
    }

    public void Reset()
    {
        scoreTextDict[TeamSide.Home].text = "0";
        scoreTextDict[TeamSide.Away].text = "0";
    }

    public void Clear()
    {
        foreach (var pair in teamCrestDict)
            pair.Value.sprite = null;

        foreach (var pair in teamNameDict)
            pair.Value.text = string.Empty;

        foreach (var binding in _crestBindings.Values)
        {
            binding.Release();
            binding.Cancel();
        }
    }

    private void OnEnable()
    {
        TeamEvents.OnTeamPreviewStarted += HandlePreviewStarted;
        TeamEvents.OnTeamPreviewEnded += HandlePreviewEnded;
    }

    private void OnDisable()
    {
        TeamEvents.OnTeamPreviewStarted -= HandlePreviewStarted;
        TeamEvents.OnTeamPreviewEnded -= HandlePreviewEnded;
    }

    private void HandlePreviewStarted()
    {
        SetCanvasGroupVisible(false);
    }

    private void HandlePreviewEnded()
    {
        SetCanvasGroupVisible(true);
    }

    private void SetCanvasGroupVisible(bool visible)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }
}
