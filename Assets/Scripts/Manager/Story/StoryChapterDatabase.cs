using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.StoryChapter;
using Simulation.Enums.Story;

public class StoryChapterDatabase : MonoBehaviour
{
    public static StoryChapterDatabase Instance { get; private set; }

    private Dictionary<int, StoryChapter> storyChapterDict = new();
    public Dictionary<int, StoryChapter> StoryChapterDict => storyChapterDict;

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

    public async Task LoadAllStoryChapterDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<StoryChapterData>(
            "StoryChapters-Data",
            data => RegisterEntry(data)
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[StoryChapterDatabase] All storyChapter data loaded. Total count: {storyChapterDataDict.Count}", this);
    }

    private void RegisterEntry(KitData data)
    {
        if (storyChapterDict.ContainsKey(data.StoryChapterNumber)) return;

        var storyChapter = new StoryChapter(data);
        storyChapterDict[data.StoryChapterNumber] = storyChapter;
    }


    public StoryChapterData GetStoryChapter(int id)
    {
        if (id == null)
        {
            LogManager.Error("[StoryChapterDatabase] Tried to StoryChapterDatabase with null/empty id!");
            return null;
        }

        if (!storyChapterDict.TryGetValue(id, out var storyChapter))
        {
            LogManager.Error($"[StoryChapterDatabase] No storyChapter found for id '{id}'.");
            return null;
        }

        return storyChapter;
    }
}

