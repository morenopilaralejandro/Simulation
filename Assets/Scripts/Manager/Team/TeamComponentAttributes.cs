using System.Collections.Generic;
using UnityEngine;

public class TeamComponentAttributes : MonoBehaviour
{
    [SerializeField] private string teamId;

    public string TeamId => teamId;

    public void Initialize(TeamData teamData)
    {
        teamId = teamData.TeamId;
    }
}
