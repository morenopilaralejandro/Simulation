using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance { get; private set; }

    private readonly Dictionary<string, BallData> ballDataDict = new();

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
        LoadAllData();
    }

    private void LoadAllData()
    {
        Addressables.LoadAssetsAsync<BallData>("Balls-Data", data =>
        {
            if (!ballDataDict.ContainsKey(data.BallId))
                ballDataDict.Add(data.BallId, data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[BallManager] All balls loaded. Total count: {ballDataDict.Count}", this);
            IsReady = true;
        };
    }

    public BallData GetBallData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[BallManager] Tried to GetBallData with null/empty id!");
            return null;
        }

        if (!ballDataDict.TryGetValue(id, out var ballData))
        {
            LogManager.Error($"[BallManager] No ballData found for id '{id}'.");
            return null;
        }

        return ballData;
    }

}
