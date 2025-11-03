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
    private string defaultId = "default";
    #endregion

    public TeamComponentAppearance(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        _ = InitializeAsync(teamData);
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_teamCrestAddress)) AddressableLoader.Release(_teamCrestAddress);
    }

    private async Task InitializeAsync(TeamData teamData)
    {
        _teamCrestAddress = AddressableLoader.GetTeamCrestAddress(teamData.TeamId);
        TeamCrestSprite = await AddressableLoader.LoadAsync<Sprite>(_teamCrestAddress);
        if (!TeamCrestSprite) 
        {
            _teamCrestAddress = AddressableLoader.GetTeamCrestAddress(defaultId);
            TeamCrestSprite = await AddressableLoader.LoadAsync<Sprite>(_teamCrestAddress);
        }
    }

}
