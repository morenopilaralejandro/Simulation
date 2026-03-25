using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryExampleNPCQuestGiver : MonoBehaviour
{
    [Header("NPC Info")]
    [SerializeField] private string npcId;
    [SerializeField] private string npcName;

    [Header("Quests")]
    [SerializeField] private List<string> availableQuestIds = new List<string>();

    [Header("Visual Indicators")]
    [SerializeField] private GameObject questAvailableIndicator; // !
    [SerializeField] private GameObject questInProgressIndicator; // ?
    [SerializeField] private GameObject questCompleteIndicator; // ✓

    private void Update()
    {
        UpdateIndicators();
    }

    private void UpdateIndicators()
    {
        var manager = StoryProgressManager.Instance;
        bool hasAvailable = false;
        bool hasInProgress = false;
        bool hasComplete = false;

        foreach (var questId in availableQuestIds)
        {
            var status = manager.GetQuestStatus(questId);

            switch (status)
            {
                case QuestStatus.NotStarted:
                    if (manager.CheckQuestPrerequisites(questId))
                        hasAvailable = true;
                    break;
                case QuestStatus.Active:
                    hasInProgress = true;
                    // Check if ready to turn in
                    // (you'd check objectives here)
                    break;
                case QuestStatus.Completed:
                    hasComplete = true;
                    break;
            }
        }

        if (questAvailableIndicator) questAvailableIndicator.SetActive(hasAvailable && !hasInProgress);
        if (questInProgressIndicator) questInProgressIndicator.SetActive(hasInProgress);
        if (questCompleteIndicator) questCompleteIndicator.SetActive(hasComplete);
    }

    public void Interact()
    {
        var manager = StoryProgressManager.Instance;

        foreach (var questId in availableQuestIds)
        {
            var status = manager.GetQuestStatus(questId);

            if (status == QuestStatus.Active)
            {
                // Show in-progress dialogue
                Debug.Log($"[NPC {npcName}] Quest '{questId}' is in progress.");
                // DialogueManager.Instance.StartDialogue(inProgressDialogueId);
                return;
            }

            if (status == QuestStatus.NotStarted && manager.CheckQuestPrerequisites(questId))
            {
                // Offer quest
                Debug.Log($"[NPC {npcName}] Offering quest '{questId}'.");
                manager.StartQuest(questId);
                // DialogueManager.Instance.StartDialogue(startDialogueId);
                return;
            }
        }

        Debug.Log($"[NPC {npcName}] No quests available.");
    }
}
