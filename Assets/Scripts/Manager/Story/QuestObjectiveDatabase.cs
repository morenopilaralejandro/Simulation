using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.QuestObjective;
using Simulation.Enums.Story;

public class QuestObjectiveDatabase : MonoBehaviour
{
    public static QuestObjectiveDatabase Instance { get; private set; }

    private readonly Dictionary<string, QuestObjectiveData> questObjectiveDataDict = new();

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

    public async Task LoadAllQuestObjectiveDataAsync()
    {
        var handle = Addressables.LoadAssetsAsync<QuestObjectiveData>(
            "Quests-Objective-Data",
            data => questObjectiveDataDict[data.QuestObjectiveId] = data
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[QuestObjectiveDatabase] All questObjective data loaded. Total count: {questObjectiveDataDict.Count}", this);
    }

    public QuestObjectiveData GetQuestObjectiveData(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            LogManager.Error("[QuestObjectiveDatabase] Tried to QuestObjectiveDatabase with null/empty id!");
            return null;
        }

        if (!questObjectiveDataDict.TryGetValue(id, out var questObjectiveData))
        {
            LogManager.Error($"[QuestObjectiveDatabase] No questObjectiveData found for id '{id}'.");
            return null;
        }

        return questObjectiveData;
    }
}

