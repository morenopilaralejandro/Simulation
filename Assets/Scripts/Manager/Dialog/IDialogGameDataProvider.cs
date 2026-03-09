/// <summary>
/// Interface that connects the dialog system to your game's data.
/// Implement this with your actual inventory, quest, and player systems.
/// </summary>
public interface IDialogGameDataProvider
{
    // Player
    string GetPlayerName();
        
    // Inventory
    bool HasItem(string itemId);
    int GetItemCount(string itemId);
    void GiveItem(string itemId, int count);
    void RemoveItem(string itemId, int count);
        
    // Currency
    int GetGold();
    void GiveGold(int amount);
    void RemoveGold(int amount);
        
    // Flags & Quests
    bool GetFlag(string flagName);
    void SetFlag(string flagName, bool value);
        
    // Stats
    int GetReputation();
    void SetReputation(int value);
}
