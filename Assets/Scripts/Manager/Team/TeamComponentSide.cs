using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class TeamComponentSide
{
    private Team team;

    public TeamSide TeamSide { get; private set; }
    public Variant Variant { get; private set; }

    public TeamComponentSide(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
    }

    public void Deinitialize()
    {
    }

    public void SetSide(TeamSide teamSide) => TeamSide = teamSide;
    public void SetVariant(Variant variant) => Variant = variant;
    public void ResetSideAndVariant() 
    {
        TeamSide = TeamSide.Home;
        Variant = Variant.Home;
    }
}
