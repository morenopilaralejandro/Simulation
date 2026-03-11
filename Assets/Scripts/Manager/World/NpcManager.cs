using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NpcManager : MonoBehaviour
{
    public static NpcManager Instance { get; private set; }

    private readonly Dictionary<string, NpcData> npcDataDict = new();

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

    public async Task LoadAllNpcDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<NpcData>(
            "Npcs-Data",
            data => npcDataDict[data.NpcId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[NpcManager] All npcs loaded. Total count: {npcDataDict.Count}", this);
    }

    public NpcData GetNpcData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[NpcManager] Tried to GetNpcData with null/empty id!");
            return null;
        }

        if (!npcDataDict.TryGetValue(id, out var npcData))
        {
            LogManager.Error($"[NpcManager] No npcData found for id '{id}'.");
            return null;
        }

        return npcData;
    }
}
