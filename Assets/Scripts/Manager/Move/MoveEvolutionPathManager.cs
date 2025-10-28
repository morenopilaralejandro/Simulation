using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Move;

public class MoveEvolutionPathManager : MonoBehaviour
{
    public static MoveEvolutionPathManager Instance { get; private set; }

    private readonly Dictionary<GrowthType, MoveEvolutionPath> moveEvolutionPathDict = new Dictionary<GrowthType, MoveEvolutionPath>();

    public bool IsReady { get; private set; } = false;

    private async void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        await LoadAllMoveEvolutionPathAsync();
        IsReady = true;
        LogManager.Trace($"[MoveEvolutionPathManager] All MoveEvolutionPath loaded. Total count: {moveEvolutionPathDict.Count}", this);
    }

    public async Task LoadAllMoveEvolutionPathAsync()
    {
        var handle = Addressables.LoadAssetsAsync<MoveEvolutionPath>(
            "Moves-Evolutions-Path",
            data => RegisterMoveEvolutionPath(data)
        );

        await handle.Task;
    }

    public void RegisterMoveEvolutionPath(MoveEvolutionPath moveEvolutionPath)
    {
        if (!moveEvolutionPathDict.ContainsKey(moveEvolutionPath.growthType))
            moveEvolutionPathDict.Add(moveEvolutionPath.growthType, moveEvolutionPath);
    }

    public MoveEvolutionPath GetMoveEvolutionPath(MoveData moveData)
    {
        if (!moveEvolutionPathDict.TryGetValue(moveData.GrowthType, out var moveEvolutionPath))
        {
            LogManager.Error($"[MoveEvolutionPathManager] No MoveEvolutionPath found for type '{moveData.GrowthType}'.");
            return null;
        }

        return moveEvolutionPath;
    }
}
