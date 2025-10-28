using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class FormationManager : MonoBehaviour
{
    public static FormationManager Instance { get; private set; }

    private readonly Dictionary<string, Formation> formations = new();

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

    public async Task LoadAllFormationsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<FormationData>(
            "Formations-Data",
            data => RegisterFormation(data)
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[FormationManager] All formations loaded. Total count: {formations.Count}", this);
    }

    private void RegisterFormation(FormationData data)
    {
        if (!formations.ContainsKey(data.FormationId))
        {
            var f = new Formation();
            f.Initialize(data);
            formations.Add(f.FormationId, f);
        }
    }

    public Formation GetFormation(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[FormationManager] Tried to GetFormation with null/empty id!");
            return null;
        }

        if (!formations.TryGetValue(id, out var formation))
        {
            LogManager.Error($"[FormationManager] No formation found for id '{id}'.");
            return null;
        }

        return formation;
    }
}
