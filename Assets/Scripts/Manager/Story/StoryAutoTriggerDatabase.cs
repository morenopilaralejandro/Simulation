using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryAutoTriggerDatabase : MonoBehaviour
{
    public static StoryAutoTriggerDatabase Instance { get; private set; }

    private Dictionary<string, StoryAutoTriggerData> storyAutoTriggerDataDict = new();
    public IReadOnlyDictionary<string, StoryAutoTriggerData> StoryAutoTriggerDataDict => storyAutoTriggerDataDict;

    private Dictionary<string, StoryAutoTrigger> storyAutoTriggerDict = new();
    public IReadOnlyDictionary<string, StoryAutoTrigger> StoryAutoTriggerDict => storyAutoTriggerDict;

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

    public async Task LoadAllStoryAutoTriggerDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<StoryAutoTriggerData>(
            "StoryAutoTrigger-Data",
            data => RegisterEntry(data)
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[StoryAutoTriggerDatabase] All storyAutoTrigger data loaded. Total count: {storyAutoTriggerDataDict.Count}", this);
    }

    private void RegisterEntry(StoryAutoTriggerData data)
    {
        if (storyAutoTriggerDataDict.ContainsKey(data.StoryAutoTriggerId)) return;

        storyAutoTriggerDataDict[data.StoryAutoTriggerId] = data;
        var storyAutoTrigger = new StoryAutoTrigger(data);
        storyAutoTriggerDict[data.StoryAutoTriggerId] = storyAutoTrigger;
    }


    public StoryAutoTriggerData GetStoryAutoTriggerData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[StoryAutoTriggerDatabase] Tried to StoryAutoTriggerDatabase with null/empty id!");
            return null;
        }

        if (!storyAutoTriggerDataDict.TryGetValue(id, out var storyAutoTriggerData))
        {
            LogManager.Error($"[StoryAutoTriggerDatabase] No storyAutoTriggerData found for id '{id}'.");
            return null;
        }

        return storyAutoTriggerData;
    }

    public StoryAutoTrigger GetStoryAutoTrigger(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[StoryAutoTriggerDatabase] Tried to StoryAutoTriggerDatabase with null/empty id!");
            return null;
        }

        if (!storyAutoTriggerDict.TryGetValue(id, out var storyAutoTrigger))
        {
            LogManager.Error($"[StoryAutoTriggerDatabase] No storyAutoTrigger found for id '{id}'.");
            return null;
        }

        return storyAutoTrigger;
    }
}

