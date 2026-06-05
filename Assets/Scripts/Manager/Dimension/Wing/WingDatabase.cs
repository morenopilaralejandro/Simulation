using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using Aremoreno.Enums.Wing;

public class WingDatabase : MonoBehaviour
{
    public static WingDatabase Instance { get; private set; }

    private readonly Dictionary<string, WingData> wingDataDict = new();
    private readonly Dictionary<(WingGrowthType, WingGrowthRate), WingEvolutionGrowthProfile> wingEvolutionGrowthProfileDict = new Dictionary<(WingGrowthType, WingGrowthRate), WingEvolutionGrowthProfile>();
    private readonly Dictionary<WingGrowthType, WingEvolutionPath> wingEvolutionPathDict = new Dictionary<WingGrowthType, WingEvolutionPath>();

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
    }

    // data

    public async Task LoadAllWingDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<WingData>(
            "Wing-Data",
            data => wingDataDict[data.WingId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[WingData] All data loaded. Total count: {wingDataDict.Count}", this);
    }

    public WingData GetWingData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[WingDatabase] Tried to get data with null/empty id!");
            return null;
        }

        if (!wingDataDict.TryGetValue(id, out var wingData))
        {
            LogManager.Error($"[WingDatabase] No data found for id '{id}'.");
            return null;
        }

        return wingData;
    }

    // growthProfile
    public async Task LoadAllWingEvolutionGrowthProfileAsync()
    {
        var handle = Addressables.LoadAssetsAsync<WingEvolutionGrowthProfile>(
            "Wing-Evolution-Growth",
            data => RegisterWingEvolutionGrowthProfile(data)
        );

        await handle.Task;
    }

    private void RegisterWingEvolutionGrowthProfile(WingEvolutionGrowthProfile wingEvolutionGrowthProfile)
    {
        var key = (wingEvolutionGrowthProfile.growthType, wingEvolutionGrowthProfile.growthRate);
        if (!wingEvolutionGrowthProfileDict.ContainsKey(key)) 
        {
            wingEvolutionGrowthProfile.Initialize();
            wingEvolutionGrowthProfileDict.Add(key, wingEvolutionGrowthProfile);    
        }
    }

    public WingEvolutionGrowthProfile GetWingEvolutionGrowthProfile(WingData wingData)
    {
        var key = (wingData.WingGrowthType, wingData.WingGrowthRate);
        if (!wingEvolutionGrowthProfileDict.TryGetValue(key, out var wingEvolutionGrowthProfile))
        {
            LogManager.Error($"[WingDatabase] No GrowthProfile found for type '{wingData.WingGrowthType}' and rate '{wingData.WingGrowthRate}'.");
            return null;
        }

        return wingEvolutionGrowthProfile;
    }

    // path
    public async Task LoadAllWingEvolutionPathAsync()
    {
        var handle = Addressables.LoadAssetsAsync<WingEvolutionPath>(
            "Wing-Evolution-Path",
            data => RegisterWingEvolutionPath(data)
        );

        await handle.Task;
    }

    public void RegisterWingEvolutionPath(WingEvolutionPath wingEvolutionPath)
    {
        if (!wingEvolutionPathDict.ContainsKey(wingEvolutionPath.growthType)) 
        {
            wingEvolutionPath.Initialize();
            wingEvolutionPathDict.Add(wingEvolutionPath.growthType, wingEvolutionPath);
        }
    }

    public WingEvolutionPath GetWingEvolutionPath(WingData wingData)
    {
        if (!wingEvolutionPathDict.TryGetValue(wingData.WingGrowthType, out var wingEvolutionPath))
        {
            LogManager.Error($"[WingDatabase] No Path found for type '{wingData.WingGrowthType}'.");
            return null;
        }

        return wingEvolutionPath;
    }
}
