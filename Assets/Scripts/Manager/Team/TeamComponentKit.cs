using System.Collections.Generic;
using UnityEngine;

public class TeamComponentKit : MonoBehaviour
{
    private Team team;

    [SerializeField] private Kit kit;

    public Kit Kit => kit;

    public void Initialize(TeamData teamData)
    {
        kit = KitManager.Instance.GetKit(teamData.KitId);
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }

    public void SetKit(Kit kit)
    {
        if (kit == null) return;
        this.kit = kit;
        TeamEvents.RaiseOnKitChanged(team, kit);
    }
}
