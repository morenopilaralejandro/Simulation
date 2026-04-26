using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

/// <summary>
/// Example showing how to integrate the story system into gameplay.
/// </summary>
public class StoryExampleGameplay : MonoBehaviour
{
    /*
    private void Start()
    {
        var story = StoryProgressManager.Instance;

        // Listen for story events
        story.OnQuestStatusChanged += (questId, status) =>
        {
            Debug.Log($"Quest '{questId}' is now {status}");

            if (status == QuestStatus.Completed)
                ShowCompletionPopup(questId);
        };

        story.OnChapterChanged += chapter =>
        {
            Debug.Log($"Chapter {chapter} begins!");
            // Play chapter intro cutscene
        };

        story.OnStoryFlagChanged += flag =>
        {
            if (flag == "boss_defeated")
            {
                // Unlock new area
                Debug.Log("New area unlocked!");
            }
        };
    }

    // Called when player kills an enemy
    public void OnEnemyKilled(string enemyId)
    {
        var story = StoryProgressManager.Instance;

        // Update any active kill quests
        foreach (var quest in story.GetActiveQuests())
        {
            // You'd check objective targets here
            story.UpdateQuestObjectiveProgress(quest.QuestId, $"kill_{enemyId}", 1);
        }
    }

    // Called when player picks up an item
    public void OnItemCollected(string itemId)
    {
        var story = StoryProgressManager.Instance;

        foreach (var quest in story.GetActiveQuests())
        {
            story.UpdateQuestObjectiveProgress(quest.QuestId, $"collect_{itemId}", 1);
        }
    }

    // Called when player enters a location
    public void OnLocationEntered(string locationId)
    {
        var story = StoryProgressManager.Instance;
        story.SetFlag($"visited_{locationId}", true);

        foreach (var quest in story.GetActiveQuests())
        {
            story.CompleteQuestObjective(quest.QuestId, $"explore_{locationId}");
        }
    }

    private void ShowCompletionPopup(string questId)
    {
        Debug.Log($"[UI] Quest Complete: {questId}!");
    }
    */
}
