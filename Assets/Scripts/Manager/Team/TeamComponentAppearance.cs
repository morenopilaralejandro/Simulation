using UnityEngine;
using System.Threading.Tasks;

public class TeamComponentAppearance
{
    #region Sprite

    public Sprite TeamCrestSprite { get; private set; }

    #endregion

    #region Address

    private string _teamCrestAddress;

    #endregion

    #region Internal

    private Team team;
    private string defaultIdCommon = "default";
    //private string defaultIdRare = "default";

    #endregion

    public TeamComponentAppearance(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team, TeamSaveData teamSaveData = null)
    {
        this.team = team;
        _ = InitializeAsync(teamData, teamSaveData);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_teamCrestAddress)) AddressableLoader.Release(_teamCrestAddress);
    }

    private async Task InitializeAsync(TeamData teamData, TeamSaveData teamSaveData = null)
    {
        if (teamSaveData != null) 
        {
            TeamCrestSprite = await SpriteAtlasManager.Instance.GetTeamCrest(teamSaveData.CustomCrestId);
            return;
        }


        _teamCrestAddress = AddressableLoader.GetTeamCrestAddress(teamData.TeamId);
        if (_teamCrestAddress != null) 
        {
            TeamCrestSprite = await SpriteAtlasManager.Instance.GetTeamCrest(teamData.TeamId);
        } else 
        {
            //TODO check common or rare
            TeamCrestSprite = await SpriteAtlasManager.Instance.GetTeamCrest(defaultIdCommon);
        }
    }

}
