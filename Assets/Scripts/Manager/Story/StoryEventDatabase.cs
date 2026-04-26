using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryEventDatabase : MonoBehaviour
{
    public static StoryEventDatabase Instance { get; private set; }

    private Dictionary<string, StoryEventData> storyEventDataDict = new();
    public IReadOnlyDictionary<string, StoryEventData> StoryEventDataDict => storyEventDataDict;

    public bool IsReady { get; private set; } = false;

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

    public async Task LoadAllStoryEventDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<StoryEventData>(
            "StoryEvent-Data",
            data => storyEventDataDict[data.StoryEventId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[StoryEventDatabase] All storyEvent data loaded. Total count: {storyEventDataDict.Count}", this);
    }

    public StoryEventData GetStoryEventData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[StoryEventDatabase] Tried to StoryEventDatabase with null/empty id!");
            return null;
        }

        if (!storyEventDataDict.TryGetValue(id, out var storyEventData))
        {
            LogManager.Error($"[StoryEventDatabase] No storyEventData found for id '{id}'.");
            return null;
        }

        return storyEventData;
    }
}

