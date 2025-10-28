using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Simulation.Enums.Move;

public class MoveEvolutionGrowthProfileManager : MonoBehaviour
{
    public static MoveEvolutionGrowthProfileManager Instance { get; private set; }

    private readonly Dictionary<(GrowthType, GrowthRate), MoveEvolutionGrowthProfile> moveEvolutionGrowthProfileDict = new Dictionary<(GrowthType, GrowthRate), MoveEvolutionGrowthProfile>();

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

        await LoadAllMoveEvolutionGrowthProfileAsync();
        IsReady = true;
        LogManager.Trace($"[MoveEvolutionGrowthProfileManager] All MoveEvolutionGrowthProfile loaded. Total count: {moveEvolutionGrowthProfileDict.Count}", this);
    }

    public async Task LoadAllMoveEvolutionGrowthProfileAsync()
    {
        Addressables.LoadAssetsAsync<MoveEvolutionGrowthProfile>("Moves-Evolutions-Growth", data =>
        {
            RegisterMoveEvolutionGrowthProfile(data);
        }).Completed += handle =>
        {
            
            IsReady = true;
        };

        var handle = Addressables.LoadAssetsAsync<MoveEvolutionGrowthProfile>(
            "Moves-Evolutions-Growth",
            data => RegisterMoveEvolutionGrowthProfile(data)
        );

        await handle.Task;
    }

    private void RegisterMoveEvolutionGrowthProfile(MoveEvolutionGrowthProfile moveEvolutionGrowthProfile)
    {
        var key = (moveEvolutionGrowthProfile.growthType, moveEvolutionGrowthProfile.growthRate);
        if (!moveEvolutionGrowthProfileDict.ContainsKey(key))
            moveEvolutionGrowthProfileDict.Add(key, moveEvolutionGrowthProfile);
    }

    public MoveEvolutionGrowthProfile GetMoveEvolutionGrowthProfile(MoveData moveData)
    {
        var key = (moveData.GrowthType, moveData.GrowthRate);
        if (!moveEvolutionGrowthProfileDict.TryGetValue(key, out var moveEvolutionGrowthProfile))
        {
            LogManager.Error($"[MoveEvolutionGrowthProfileManager] No MoveEvolutionGrowthProfile found for type '{moveData.GrowthType}' and rate '{moveData.GrowthRate}'.");
            return null;
        }

        return moveEvolutionGrowthProfile;
    }
}
