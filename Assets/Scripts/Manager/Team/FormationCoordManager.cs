using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class FormationCoordManager : MonoBehaviour
{
    public static FormationCoordManager Instance { get; private set; }

    private readonly Dictionary<string, FormationCoordData> formationCoordDict = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllFormationCoords();
    }

    private void LoadAllFormationCoords()
    {
        Addressables.LoadAssetsAsync<FormationCoordData>("FormationCoords", data =>
        {
            if (!formationCoordDict.ContainsKey(data.FormationCoordId))
                formationCoordDict.Add(data.FormationCoordId, data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[FormationCoordManager] All formationCoords loaded. Total count: {formationCoordDict.Count}", this);
            FormationManager.Instance.LoadAllFormations();
        };
    }

    public FormationCoordData GetFormationCoordData(string id)
    {
        if (formationCoordDict.TryGetValue(id, out var cd))
            return cd;
        return null;
    }
}
