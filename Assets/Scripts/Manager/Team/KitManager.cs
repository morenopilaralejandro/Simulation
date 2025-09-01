using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class KitManager : MonoBehaviour
{
    public static KitManager Instance { get; private set; }

    private readonly Dictionary<string, Kit> kits = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadAllTeams();
    }

    public void LoadAllTeams()
    {
        Addressables.LoadAssetsAsync<KitData>("Kits", RegisterKit);
    }

    public void RegisterKit(KitData data)
    {
        if (!kits.ContainsKey(data.KitId))
        {
            var kit = new Kit();
            kit.Initialize(data);
            kits.Add(kit.KitId, kit);
        }
    }

    public Kit GetKit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[KitManager] Tried to GetKit with null/empty id!");
            return null;
        }

        if (!kits.TryGetValue(id, out var kit))
        {
            LogManager.Error($"[KitManager] No kit found for id '{id}'.");
            return null;
        }

        return kit;
    }
}
