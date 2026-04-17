using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.World;

public class OverworldDefinitionDatabase : MonoBehaviour
{
    public static OverworldDefinitionDatabase Instance { get; private set; }

    private Dictionary<Realm, OverworldDefinition> overworldDefinitionDataDict = new();
    public IReadOnlyDictionary<Realm, OverworldDefinition> OverworldDefinitionDataDict => overworldDefinitionDataDict;

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

    public async Task LoadAllOverworldDefinitionDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<OverworldDefinition>(
            "OverworldDefinition-Data",
            data => overworldDefinitionDataDict[data.Realm] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[OverworldDefinitionDatabase] All overworldDefinition data loaded. Total count: {overworldDefinitionDataDict.Count}", this);
    }

    public OverworldDefinition GetOverworldDefinitionByRealm(Realm id)
    {
        if (!overworldDefinitionDataDict.TryGetValue(id, out var overworldDefinitionData))
        {
            LogManager.Error($"[OverworldDefinitionDatabase] No overworldDefinitionData found for id '{id}'.");
            return null;
        }

        return overworldDefinitionData;
    }
}

