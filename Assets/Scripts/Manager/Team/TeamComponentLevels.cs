using System.Collections.Generic;
using UnityEngine;

public class TeamComponentLevels
{
    private Team team;

    private int level;

    public int Level => level;

    public TeamComponentLevels(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        this.level = teamData.Lv;
    }

}
