using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implementation for IDialogGameDataProvider.
/// </summary>
public class DialogGameDataProvider : IDialogGameDataProvider
{
    private string _playerName = "Hero";
    private int _gold = 1000;
    private int _reputation = 0;

    private ItemStorageManager _itemStorageManager;
    private Item _cachedItem;

    //placeholder
    private Dictionary<string, int> _inventory = new Dictionary<string, int>();
    private Dictionary<string, bool> _flags = new Dictionary<string, bool>();

    public DialogGameDataProvider() 
    {
        _itemStorageManager = ItemStorageManager.Instance;
    }

    public string GetPlayerName() => _playerName;
    
    public bool HasItem(string itemId) 
    {
        _cachedItem = ItemFactory.CreateById(itemId);
        return _itemStorageManager.HasItem(_cachedItem);
    }

    public int GetItemCount(string itemId) 
    {
        _cachedItem = ItemFactory.CreateById(itemId);
        return _itemStorageManager.GetItemCount(_cachedItem);
    }
    
    public void GiveItem(string itemId, int count = 1)
    {
        _cachedItem = ItemFactory.CreateById(itemId);
        _itemStorageManager.AddItem(_cachedItem, count);
    }
    
    public void RemoveItem(string itemId, int count = 1)
    {
        _cachedItem = ItemFactory.CreateById(itemId);
        _itemStorageManager.RemoveItem(_cachedItem, count);
    }
    
    public int GetGold() => _gold;
    public void GiveGold(int amount) { _gold += amount; }
    public void RemoveGold(int amount) { _gold = Mathf.Max(0, _gold - amount); }
    
    public bool GetFlag(string flagName) => 
        _flags.TryGetValue(flagName, out bool val) && val;
    
    public void SetFlag(string flagName, bool value) => _flags[flagName] = value;
    
    public int GetReputation() => _reputation;
    public void SetReputation(int value) => _reputation = value;
}
