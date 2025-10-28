using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FormationCoordManager : MonoBehaviour
{
    public static FormationCoordManager Instance { get; private set; }

    private readonly Dictionary<string, FormationCoordData> formationCoordDataDict = new();

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

        await LoadAllFormationCoordDataAsync();
        IsReady = true;
        LogManager.Trace($"[FormationCoordManager] All formationCoordData loaded. Total count: {formationCoordDataDict.Count}", this);
    }

    public async Task LoadAllFormationCoordDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<FormationCoordData>(
            "FormationCoords-Data",
            data => formationCoordDataDict[data.FormationCoordId] = data
        );

        await handle.Task;
    }

    public FormationCoordData GetFormationCoordData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[FormationCoordManager] Tried to GetFormationCoordData with null/empty id!");
            return null;
        }

        if (!formationCoordDataDict.TryGetValue(id, out var formationCoordData))
        {
            LogManager.Error($"[FormationCoordManager] No formationCoordData found for id '{id}'.");
            return null;
        }

        return formationCoordData;
    }
}
