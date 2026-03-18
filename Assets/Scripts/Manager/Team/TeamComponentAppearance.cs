using UnityEngine;
using System.Threading.Tasks;

public class TeamComponentAppearance
{
    #region Sprite

    public Sprite TeamCrestSprite { get; private set; }
    public string TeamCrestId { get; private set; }

    #endregion

    #region Internal

    private Team team;

    #endregion

    public TeamComponentAppearance(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team, teamSaveData);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;

        if (teamSaveData != null) 
        {
            TeamCrestId = teamSaveData.CustomCrestId;
            _ = InitializeAsync(TeamCrestId);
            return;
        }

        TeamCrestId = teamData.TeamId;
        _ = InitializeAsync(TeamCrestId);

        /*
        if (teamSaveData != null) 
        {
            TeamCrestId = teamSaveData.CustomCrestId;
            _ = InitializeAsync(TeamCrestId);
            return;
        }


        if (BattleArgs.Context == RandomEncounter) 
        {
            TeamCrestId = isRare ? TeamLoadoutManager.TEAM_CREST_ID_RARE : TeamLoadoutManager.TEAM_CREST_ID_COMMON
        } else 
        {
            TeamCrestId = teamData.TeamId;
        }

        _ = InitializeAsync(TeamCrestId);

        */

    }

    private async Task InitializeAsync(string teamCrestId)
    { 
        TeamCrestSprite = await SpriteAtlasManager.Instance.GetTeamCrest(teamCrestId);
        TeamEvents.RaiseTeamCrestSpriteUpdated(team);
    }

    public void UpdateAppeariance(string teamCrestId) 
    {
        TeamCrestId = teamCrestId;
        _ = InitializeAsync(TeamCrestId);
    }

}
