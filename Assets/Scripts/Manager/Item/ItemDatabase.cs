using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance { get; private set; }

    private readonly Dictionary<string, ItemData> itemDataDict = new();

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

    // ---- Loading ----

    public async Task LoadAllItemDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<ItemData>(
            "Items-Data",
            data => itemDataDict[data.ItemId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[ItemManager] All items loaded. Total count: {itemDataDict.Count}", this);
    }

    // ---- Get ScriptableObject Data ----

    public ItemData GetItemData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[ItemManager] Tried to GetItemData with null/empty id!");
            return null;
        }

        if (!itemDataDict.TryGetValue(id, out var itemData))
        {
            LogManager.Error($"[ItemManager] No ItemData found for id '{id}'.");
            return null;
        }

        return itemData;
    }

    /// <summary>
    /// Get ItemData cast to a specific subclass
    /// </summary>
    public T GetItemData<T>(string id) where T : ItemData
    {
        var data = GetItemData(id);
        if (data == null) return null;

        if (data is T typed)
            return typed;

        LogManager.Error($"[ItemManager] Item '{id}' is {data.GetType().Name}, not {typeof(T).Name}.");
        return null;
    }

}
