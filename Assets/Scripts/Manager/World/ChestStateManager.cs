using UnityEngine;
using System;
using System.Collections.Generic;

public class ChestStateManager : MonoBehaviour
{
    public static ChestStateManager Instance;

    private HashSet<string> openedChests = new HashSet<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public bool IsOpened(string chestId)
    {
        return openedChests.Contains(chestId);
    }

    public void Open(string chestId)
    {
        openedChests.Add(chestId);
    }

    #region Persistence
    
    public ChestStateSaveData Export()
    {
        ChestStateSaveData saveData = new ChestStateSaveData();
        saveData.OpenedChestIds = new List<string>(openedChests);
        return saveData;
    }

    public void Import(ChestStateSaveData saveData)
    {
        openedChests.Clear();

        if (saveData?.OpenedChestIds == null) return;

        foreach (string chestId in saveData.OpenedChestIds)
        {
            openedChests.Add(chestId);
        }
    }

    #endregion
}
