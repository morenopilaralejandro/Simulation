using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implementation for IDialogGameDataProvider.
/// </summary>
public class DialogGameDataProvider : MonoBehaviour, IDialogGameDataProvider
{
    [SerializeField] private string _playerName = "Hero";
    [SerializeField] private int _gold = 1000;
    
    private Dictionary<string, int> _inventory = new Dictionary<string, int>();
    private Dictionary<string, bool> _flags = new Dictionary<string, bool>();
    private int _reputation = 0;

    public string GetPlayerName() => _playerName;
    
    public bool HasItem(string itemId) => 
        _inventory.ContainsKey(itemId) && _inventory[itemId] > 0;
    
    public int GetItemCount(string itemId) => 
        _inventory.TryGetValue(itemId, out int count) ? count : 0;
    
    public void GiveItem(string itemId, int count)
    {
        if (!_inventory.ContainsKey(itemId)) _inventory[itemId] = 0;
            _inventory[itemId] += count;
        LogManager.Trace($"[DialogGameDataProvider] Gave {count}x {itemId}. Total: {_inventory[itemId]}");
    }
    
    public void RemoveItem(string itemId, int count)
    {
        if (_inventory.ContainsKey(itemId))
        {
            _inventory[itemId] = Mathf.Max(0, _inventory[itemId] - count);
            LogManager.Trace($"[DialogGameDataProvider] Removed {count}x {itemId}. Remaining: {_inventory[itemId]}");
        }
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
