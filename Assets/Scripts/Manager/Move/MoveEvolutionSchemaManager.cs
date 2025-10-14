using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Simulation.Enums.Move;

public class MoveEvolutionGrowthProfileManager : MonoBehaviour
{
    public static MoveEvolutionGrowthProfileManager Instance { get; private set; }

    private readonly Dictionary<(GrowthType, GrowthRate), MoveEvolutionGrowthProfile> moveEvolutionGrowthProfileDict = new Dictionary<(GrowthType, GrowthRate), MoveEvolutionGrowthProfile>();

    public bool IsReady { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadAllMoveEvolutionGrowthProfile();
    }

    public void LoadAllMoveEvolutionGrowthProfile()
    {
        Addressables.LoadAssetsAsync<MoveEvolutionGrowthProfile>("Moves-Evolutions-GrowthProfile", data =>
        {
            RegisterMoveEvolutionGrowthProfile(data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[MoveEvolutionGrowthProfileManager] All MoveEvolutionGrowthProfile loaded. Total count: {moveEvolutionGrowthProfileDict.Count}", this);
            IsReady = true;
        };
    }

    public void RegisterMoveEvolutionGrowthProfile(MoveEvolutionGrowthProfile moveEvolutionGrowthProfile)
    {
        var key = (moveEvolutionGrowthProfile.growthType, moveEvolutionGrowthProfile.growthRate);
        if (!moveEvolutionGrowthProfileDict.ContainsKey(key))
            moveEvolutionGrowthProfileDict.Add(key, moveEvolutionGrowthProfile);
    }

    public MoveEvolutionGrowthProfile GetMoveEvolutionGrowthProfile(Move move)
    {
        var key = (move.GrowthType, move.GrowthRate);
        if (!moveEvolutionGrowthProfileDict.TryGetValue(key, out var moveEvolutionGrowthProfile))
        {
            LogManager.Error($"[MoveEvolutionGrowthProfileManager] No MoveEvolutionGrowthProfile found for type '{move.GrowthType}' and rate '{move.GrowthRate}'.");
            return null;
        }

        return moveEvolutionGrowthProfile;
    }
}
