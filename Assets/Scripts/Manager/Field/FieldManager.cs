using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FieldManager : MonoBehaviour
{
    public static FieldManager Instance { get; private set; }

    private readonly Dictionary<string, FieldData> fieldDataDict = new();

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

    public async Task LoadAllFieldDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<FieldData>(
            "Fields-Data",
            data => fieldDataDict[data.FieldId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[FieldManager] All fields loaded. Total count: {fieldDataDict.Count}", this);
    }

    public FieldData GetFieldData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[FieldManager] Tried to GetFieldData with null/empty id!");
            return null;
        }

        if (!fieldDataDict.TryGetValue(id, out var fieldData))
        {
            LogManager.Error($"[FieldManager] No fieldData found for id '{id}'.");
            return null;
        }

        return fieldData;
    }

}
