using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance { get; private set; }

    private readonly Dictionary<string, MoveData> moveDataDict = new();

    public bool IsReady { get; private set; } = false;

    public int SizeMax = 8;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        LoadAllMoveData();
    }

    public void LoadAllMoveData()
    {
        Addressables.LoadAssetsAsync<MoveData>("Moves-Data", data =>
        {
            RegisterMoveData(data);
        }).Completed += handle =>
        {
            LogManager.Trace($"[MoveManager] All moves loaded. Total count: {moveDataDict.Count}", this);
            IsReady = true;
        };
    }

    public void RegisterMoveData(MoveData moveData)
    {
        if (!moveDataDict.ContainsKey(moveData.MoveId))
        {
            moveDataDict.Add(moveData.MoveId, moveData);
        }
    }

    public MoveData GetMoveData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[MoveManager] Tried to GetMoveData with null/empty id!");
            return null;
        }

        if (!moveDataDict.TryGetValue(id, out var moveData))
        {
            LogManager.Error($"[MoveManager] No moveData found for id '{id}'.");
            return null;
        }

        return moveData;
    }
}
