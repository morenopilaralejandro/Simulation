using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;

public class MoveManager : MonoBehaviour
{
    public static MoveManager Instance { get; private set; }

    private readonly Dictionary<string, Move> moves = new();

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

        LoadAllMoves();
    }

    public void LoadAllMoves()
    {
        Addressables.LoadAssetsAsync<MoveData>("Moves", RegisterMove);
    }

    public void RegisterMove(MoveData data)
    {
        if (!moves.ContainsKey(data.MoveId))
        {
            var move = new Move();
            move.Initialize(data);
            moves.Add(move.MoveId, move);
        }
    }

    public Move GetMove(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[MoveManager] Tried to GetMove with null/empty id!");
            return null;
        }

        if (!moves.TryGetValue(id, out var move))
        {
            LogManager.Error($"[MoveManager] No move found for id '{id}'.");
            return null;
        }

        return move;
    }
}
