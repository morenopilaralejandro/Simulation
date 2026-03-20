using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Item;
using Simulation.Enums.Localization;

public class ItemStorageManager : MonoBehaviour
{
    public static ItemStorageManager Instance { get; private set; }

    private ItemStorage storage;

    public ItemStorage Storage => storage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        storage = new ItemStorage();
    }

    #region API Storage

    public void FirstTimeInitialize() => storage.FirstTimeInitialize();
    public void AddItem(Item item, int count = 1) => storage.AddItem(item, count);
    public bool RemoveItem(Item item, int count = 1) => storage.RemoveItem(item, count);

    #endregion

    #region API Usage

    #endregion

    #region Persistence

    public ItemStorageSaveData Export() => storage.Export();
    public void Import(ItemStorageSaveData saveData) => storage.Import(saveData);

    #endregion

    #region Event

    private void OnEnable() 
    {

    }

    private void OnDisable() 
    {

    }

    #endregion
}
