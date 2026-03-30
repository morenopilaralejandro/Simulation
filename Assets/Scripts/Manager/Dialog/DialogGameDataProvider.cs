using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Implementation for IDialogGameDataProvider.
/// </summary>
public class DialogGameDataProvider : IDialogGameDataProvider
{
    #region Fields

    private string _playerName = "Hero";
    private int _gold = 1000;

    private ItemManager itemManager;
    private Item cachedItem;

    private QuestSystemManager questSystemManager;
    private StorySystemManager storySystemManager;

    #endregion

    #region Constructor

    public DialogGameDataProvider() 
    {
        itemManager = ItemManager.Instance;
        questSystemManager = QuestSystemManager.Instance;
        storySystemManager = StorySystemManager.Instance;
    }

    #endregion

    #region Player

    public string GetPlayerName() => _playerName;
    
    #endregion

    #region Item

    public bool HasItem(string itemId) 
    {
        cachedItem = ItemFactory.CreateById(itemId);
        return itemManager.HasItem(cachedItem);
    }

    public int GetItemCount(string itemId) 
    {
        cachedItem = ItemFactory.CreateById(itemId);
        return itemManager.GetItemCount(cachedItem);
    }
    
    public void GiveItem(string itemId, int count = 1)
    {
        cachedItem = ItemFactory.CreateById(itemId);
        itemManager.AddItem(cachedItem, count);
    }
    
    public void RemoveItem(string itemId, int count = 1)
    {
        cachedItem = ItemFactory.CreateById(itemId);
        itemManager.RemoveItem(cachedItem, count);
    }

    #endregion    

    #region Currency

    public int GetGold() => _gold;
    public void GiveGold(int amount) { _gold += amount; }
    public void RemoveGold(int amount) { _gold = Mathf.Max(0, _gold - amount); }
    
    #endregion

    #region Story

    public bool GetFlag(string flagId) => storySystemManager.GetFlag(flagId);
    public void SetFlag(string flagId, bool boolValue) => storySystemManager.SetFlag(flagId, boolValue);
    public int GetVariable(string variableId) => storySystemManager.GetVariable(variableId);
    public void SetVariable(string variableId, int intValue) => storySystemManager.SetVariable(variableId, intValue);
    public void TriggerStoryEvent(string storyEventId) => storySystemManager.TriggerStoryEvent(storyEventId);

    #endregion
    
    #region Quest

    public void StartQuest(string questId) => questSystemManager.StartQuest(questId);

    #endregion

    #region Reputation

    public int GetReputation() => 0;
    public void SetReputation(int value) { return; }

    #endregion
}
