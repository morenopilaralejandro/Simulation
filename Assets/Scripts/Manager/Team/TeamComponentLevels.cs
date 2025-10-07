using System.Collections.Generic;
using UnityEngine;

public class TeamComponentLevels : MonoBehaviour
{
    private Team team;

    [SerializeField] private int level;

    public int Level => level;

    public void Initialize(TeamData teamData)
    {
        level = teamData.Lv;
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }
}
