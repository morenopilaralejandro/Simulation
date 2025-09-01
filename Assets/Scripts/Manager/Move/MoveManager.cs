using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance { get; private set; }

    private readonly Dictionary<string, MoveData> moveDataDict = new();

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
        Addressables.LoadAssetsAsync<MoveData>("Moves", RegisterMoveData);
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
