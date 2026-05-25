using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmblemDatabase : MonoBehaviour
{
    public static EmblemDatabase Instance { get; private set; }

    private readonly Dictionary<string, Emblem> emblems = new();
    public IReadOnlyDictionary<string, Emblem> Emblems => emblems;

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

    public async Task LoadAllEmblemsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<EmblemData>(
            "Emblems-Data",
            data => RegisterEmblem(data)
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[EmblemDatabase] All emblems loaded. Total count: {emblems.Count}", this);
    }

    private void RegisterEmblem(EmblemData data)
    {
        if (!emblems.ContainsKey(data.EmblemId))
        {
            var emblem = new Emblem(data);
            emblems.Add(emblem.EmblemId, emblem);
        }
    }

    public Emblem GetEmblem(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[EmblemDatabase] Tried to GetEmblem with null/empty id!");
            return null;
        }

        if (!emblems.TryGetValue(id, out var emblem))
        {
            LogManager.Error($"[EmblemDatabase] No emblem found for id '{id}'.");
            return null;
        }

        return emblem;
    }
}
