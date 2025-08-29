using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    private readonly Dictionary<string, Team> teams = new();

    public int SizeMax = 16;
    public int SizeBattle = 11;
    public int SizeMiniBattle = 4;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        Addressables.LoadAssetsAsync<TeamData>("Teams", RegisterTeam);
    }

    public void RegisterTeam(TeamData data)
    {
        if (!teams.ContainsKey(data.TeamId))
        {
            var team = new Team();
            team.Initialize(data);
            teams.Add(team.TeamId, team);
        }
    }

    public Team GetTeam(string id)
    {
        teams.TryGetValue(id, out var team);
        return team;
    }
}
