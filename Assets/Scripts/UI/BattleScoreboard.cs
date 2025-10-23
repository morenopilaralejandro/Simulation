using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Character;

public class BattleScoreboard : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreTextHome;
    [SerializeField] private TMP_Text scoreTextAway;
    [SerializeField] private Image teamCrestHome;
    [SerializeField] private Image teamCrestAway;
    [SerializeField] private TMP_Text teamNameHome;
    [SerializeField] private TMP_Text teamNameAway;

    private Dictionary<TeamSide, TMP_Text> scoreTextDict;
    private Dictionary<TeamSide, Image> teamCrestDict;
    private Dictionary<TeamSide, TMP_Text> teamNameDict;

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
    }

    private void OnDestroy()
    {
        if (BattleUIManager.Instance != null)
            BattleUIManager.Instance.UnregisterScoreboard(this);
    }

    public void SetTeam(Team team)
    {
        teamCrestDict[team.TeamSide].sprite = team.TeamCrestSprite;
        teamNameDict[team.TeamSide].text = team.TeamName;
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

}
