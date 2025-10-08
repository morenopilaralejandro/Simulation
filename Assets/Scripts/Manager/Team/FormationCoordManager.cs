using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FormationCoordManager : MonoBehaviour
{
    public static FormationCoordManager Instance { get; private set; }

    private readonly Dictionary<string, FormationCoordData> formationCoordDataDict = new();

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

        LoadAllFormationCoordData();
    }

    private void LoadAllFormationCoordData()
    {
        Addressables.LoadAssetsAsync<FormationCoordData>("FormationCoords-Data", data =>
        {
            if (!formationCoordDataDict.ContainsKey(data.FormationCoordId))
                formationCoordDataDict.Add(data.FormationCoordId, data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[FormationCoordManager] All formationCoordData loaded. Total count: {formationCoordDataDict.Count}", this);
            IsReady = true;
            FormationManager.Instance.LoadAllFormations();
        };
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
