using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Item;
using Simulation.Enums.Localization;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }

    #region Fields

    private ItemManagerStorage storageSystem;
    private ItemManagerCurrency currencySystem;
    private ItemManagerPersistance persistanceSystem;

    #endregion

    #region Lifecycle

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

    private void OnDestroy() 
    {
        //encounterSystem.Unsubscribe();
    }


    private void Start()
    {
        storageSystem = new ItemManagerStorage();
        currencySystem = new ItemManagerCurrency();
        persistanceSystem = new ItemManagerPersistance();
        
        //encounterSystem.Subscribe();
        //InitializeAsync();
    }

    //private async void InitializeAsync() { }

    #endregion

    #region API

    // storageSystem
    public void FirstTimeInitialize() => storageSystem.FirstTimeInitialize();
    public void AddItem(Item item, int count = 1) => storageSystem.AddItem(item, count);
    public bool RemoveItem(Item item, int count = 1) => storageSystem.RemoveItem(item, count);
    public bool HasItem(Item item) => storageSystem.HasItem(item);
    public int GetItemCount(Item item) => storageSystem.GetItemCount(item);
    public SaveDataItemStorage ExportStorageSystem() => storageSystem.Export();
    public void ImportStorageSystem(SaveDataItemStorage saveData) => storageSystem.Import(saveData);

    // currencySystem
    public IReadOnlyDictionary<CurrencyType, int> CurrencyDict => currencySystem.CurrencyDict;
    public int GetGold() => currencySystem.GetGold();
    public int GetAmount(CurrencyType type) => currencySystem.GetAmount(type);
    public void Add(CurrencyType type, int amount) => currencySystem.Add(type, amount);
    public bool Spend(CurrencyType type, int amount) => currencySystem.Spend(type, amount);
    public bool CanAfford(CurrencyType type, int amount) => currencySystem.CanAfford(type, amount);
    public SaveDataCurrencySystem ExportCurrencySystem() => currencySystem.Export();
    public void ImportCurrencySystem(SaveDataCurrencySystem saveData) => currencySystem.Import(saveData);

    // persistanceSystem
    public SaveDataItemSystem Export() => persistanceSystem.Export();
    public void Import(SaveDataItemSystem saveData) => persistanceSystem.Import(saveData);

    #endregion

    #region Event

    //private void OnEnable() { }
    //private void OnDisable() { }

    #endregion
}
