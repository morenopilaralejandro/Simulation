using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestDatabase : MonoBehaviour
{
    public static QuestDatabase Instance { get; private set; }

    private Dictionary<string, QuestData> questDataDict = new();
    public IReadOnlyDictionary<string, QuestData> QuestDataDict => questDataDict;

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

    public async Task LoadAllQuestDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<QuestData>(
            "Quest-Data",
            data => questDataDict[data.QuestId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[QuestDatabase] All quest data loaded. Total count: {questDataDict.Count}", this);
    }

    public QuestData GetQuestData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[QuestDatabase] Tried to QuestDatabase with null/empty id!");
            return null;
        }

        if (!questDataDict.TryGetValue(id, out var questData))
        {
            LogManager.Error($"[QuestDatabase] No questData found for id '{id}'.");
            return null;
        }

        return questData;
    }
}

