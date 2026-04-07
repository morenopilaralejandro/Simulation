using UnityEngine;
using System;
using System.Collections.Generic;

public class ChestStateManager : MonoBehaviour
{
    public static ChestStateManager Instance;

    private HashSet<string> openedChests = new HashSet<string>();
    private PersistenceManager persistenceManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        persistenceManager = PersistenceManager.Instance;
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
        return new ChestStateSaveData 
        {
            OpenedChestIdList = persistenceManager.ParseHashSet<string>(openedChests)
        };
    }

    public void Import(ChestStateSaveData saveData)
    {
        openedChests.Clear();

        if (saveData?.OpenedChestIdList == null) return;

        foreach (string chestId in saveData.OpenedChestIdList)
        {
            openedChests.Add(chestId);
        }
    }

    #endregion
}
