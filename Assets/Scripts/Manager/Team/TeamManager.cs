using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance { get; private set; }

    private readonly Dictionary<string, Team> teams = new();

    public bool IsReady { get; private set; } = false;

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
    }

    public async Task LoadAllTeamsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<TeamData>(
            "Teams-Data",
            data => RegisterTeam(data)
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[TeamManager] All teams loaded. Total count: {teams.Count}", this);
    }

    private void RegisterTeam(TeamData data)
    {
        if (!teams.ContainsKey(data.TeamId))
        {
            var team = new Team(data);
            teams.Add(team.TeamId, team);
        }
    }

    public Team GetTeam(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[TeamManager] Tried to GetTeam with null/empty id!");
            return null;
        }

        if (!teams.TryGetValue(id, out var team))
        {
            LogManager.Error($"[TeamManager] No team found for id '{id}'.");
            return null;
        }

        return team;
    }
}
