using System.Collections.Generic;
using UnityEngine;

public class TeamComponentLevels
{
    private Team team;

    public int Level { get; private set; }

    public TeamComponentLevels(TeamData teamData, Team team)
    {
        Initialize(teamData, team);
    }

    public void Initialize(TeamData teamData, Team team)
    {
        this.team = team;
        if (teamData != null) 
        {
            this.Level = teamData.Lv;
        } else 
        {
            this.Level = 1;
        }
    }

    public void SetLevel(int level) => Level = level;

}
