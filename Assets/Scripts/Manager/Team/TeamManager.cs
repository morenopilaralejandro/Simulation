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

    }

    public void LoadAllTeams()
    {
        var handle = Addressables.LoadAssetsAsync<TeamData>("Teams", RegisterTeam);
        handle.Completed += OnTeamsLoaded;
    }

    private void OnTeamsLoaded(AsyncOperationHandle<IList<TeamData>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            LogManager.Trace($"[TeamManager] All teams loaded. Total count: {teams.Count}", this);
            BattleManager.Instance.StartBattle();
        }
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
