using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestTrackerHUD : MonoBehaviour
{
    /*
    [SerializeField] private Transform trackerContainer;
    [SerializeField] private GameObject trackerEntryPrefab;
    [SerializeField] private StoryDatabase storyDatabase;
    [SerializeField] private int maxTrackedQuests = 3;

    private Dictionary<string, GameObject> trackedEntries = new Dictionary<string, GameObject>();

    private void Start()
    {
        var manager = StoryProgressManager.Instance;
        manager.OnQuestStatusChanged += OnQuestStatusChanged;
        RefreshTracker();
    }

    private void RefreshTracker()
    {
        // Clear
        foreach (var entry in trackedEntries.Values)
            Destroy(entry);
        trackedEntries.Clear();

        var activeQuests = StoryProgressManager.Instance.GetActiveQuests();
        int count = 0;

        foreach (var quest in activeQuests)
        {
            if (count >= maxTrackedQuests) break;

            var questData = storyDatabase.GetQuest(quest.QuestId);
            if (questData == null) continue;

            var entry = Instantiate(trackerEntryPrefab, trackerContainer);
            var texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

            if (texts.Length > 0)
                texts[0].text = $"<b>{questData.questName}</b>";

            // Show current objective
            if (texts.Length > 1)
            {
                string objectiveText = GetCurrentObjectiveText(questData, quest);
                texts[1].text = objectiveText;
            }

            trackedEntries[quest.QuestId] = entry;
            count++;
        }
    }

    private string GetCurrentObjectiveText(QuestData questData, QuestState questState)
    {
        foreach (var obj in questData.objectives)
        {
            if (!questState.IsObjectiveComplete(obj.objectiveId))
            {
                int progress = questState.GetObjectiveProgress(obj.objectiveId);
                string progressStr = obj.requiredAmount > 1
                    ? $" ({progress}/{obj.requiredAmount})"
                    : "";
                return $"  → {obj.description}{progressStr}";
            }
        }
        return "  → Return to quest giver";
    }

    private void OnQuestStatusChanged(string questId, QuestStatus status)
    {
        RefreshTracker();
    }

    private void OnDestroy()
    {
        if (StoryProgressManager.Instance != null)
            StoryProgressManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
    }
    */
}
