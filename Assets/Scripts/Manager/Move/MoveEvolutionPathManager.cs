using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Simulation.Enums.Move;

public class MoveEvolutionPathManager : MonoBehaviour
{
    public static MoveEvolutionPathManager Instance { get; private set; }

    private readonly Dictionary<GrowthType, MoveEvolutionPath> moveEvolutionPathDict = new Dictionary<GrowthType, MoveEvolutionPath>();

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

        LoadAllMoveEvolutionPath();
    }

    public void LoadAllMoveEvolutionPath()
    {
        Addressables.LoadAssetsAsync<MoveEvolutionPath>("Moves-Evolutions-Path", data =>
        {
            RegisterMoveEvolutionPath(data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[MoveEvolutionPathManager] All MoveEvolutionPath loaded. Total count: {moveEvolutionPathDict.Count}", this);
            IsReady = true;
        };
    }

    public void RegisterMoveEvolutionPath(MoveEvolutionPath moveEvolutionPath)
    {
        if (!moveEvolutionPathDict.ContainsKey(moveEvolutionPath.growthType))
            moveEvolutionPathDict.Add(moveEvolutionPath.growthType, moveEvolutionPath);
    }

    public MoveEvolutionPath GetMoveEvolutionPath(Move move)
    {
        if (!moveEvolutionPathDict.TryGetValue(move.GrowthType, out var moveEvolutionPath))
        {
            LogManager.Error($"[MoveEvolutionPathManager] No MoveEvolutionPath found for type '{move.GrowthType}'.");
            return null;
        }

        return moveEvolutionPath;
    }
}
