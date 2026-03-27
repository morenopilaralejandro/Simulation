/// <summary>
/// Interface that connects the dialog system to your game's data.
/// Implement this with your actual inventory, quest, and player systems.
/// </summary>
public interface IDialogGameDataProvider
{
    // Player
    string GetPlayerName();
        
    // Item
    bool HasItem(string itemId);
    int GetItemCount(string itemId);
    void GiveItem(string itemId, int count);
    void RemoveItem(string itemId, int count);
        
    // Currency
    int GetGold();
    void GiveGold(int amount);
    void RemoveGold(int amount);
        
    // Story
    bool GetFlag(string flagId);
    void SetFlag(string flagId, bool boolValue);
    int GetVariable(string variableId);
    void SetVariable(string variableId, int intValue);
    void TriggerStoryEvent(string storyEventId);
        
    // Quest
    void StartQuest(string questId);

    // Reputation
    int GetReputation();
    void SetReputation(int value);
}
