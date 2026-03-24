using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestJournalUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject journalPanel;
    [SerializeField] private Transform questListContainer;
    [SerializeField] private GameObject questListItemPrefab;

    [Header("Quest Detail Panel")]
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Transform objectivesContainer;
    [SerializeField] private GameObject objectiveItemPrefab;

    [Header("Tabs")]
    [SerializeField] private Button activeQuestsTab;
    [SerializeField] private Button completedQuestsTab;
    [SerializeField] private Button mainQuestsTab;

    [Header("Settings")]
    [SerializeField] private StoryDatabase storyDatabase;
    [SerializeField] private KeyCode toggleKey = KeyCode.J;

    private QuestType? filterType = null;
    private bool showCompleted = false;

    private void Start()
    {
        activeQuestsTab?.onClick.AddListener(() => { showCompleted = false; RefreshQuestList(); });
        completedQuestsTab?.onClick.AddListener(() => { showCompleted = true; RefreshQuestList(); });

        StoryProgressManager.Instance.OnQuestStatusChanged += OnQuestStatusChanged;

        journalPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            ToggleJournal();
    }

    public void ToggleJournal()
    {
        journalPanel.SetActive(!journalPanel.activeSelf);
        if (journalPanel.activeSelf)
            RefreshQuestList();
    }

    private void RefreshQuestList()
    {
        // Clear existing items
        foreach (Transform child in questListContainer)
            Destroy(child.gameObject);

        var manager = StoryProgressManager.Instance;
        var quests = showCompleted ? manager.GetCompletedQuests() : manager.GetActiveQuests();

        foreach (var questState in quests)
        {
            var questData = storyDatabase.GetQuest(questState.QuestId);
            if (questData == null) continue;

            if (filterType.HasValue && questData.questType != filterType.Value)
                continue;

            var item = Instantiate(questListItemPrefab, questListContainer);
            var titleText = item.GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
                titleText.text = questData.questName;

            var button = item.GetComponent<Button>();
            string capturedId = questState.QuestId;
            button?.onClick.AddListener(() => ShowQuestDetail(capturedId));
        }
    }

    private void ShowQuestDetail(string questId)
    {
        var questData = storyDatabase.GetQuest(questId);
        var questState = StoryProgressManager.Instance.GetQuestState(questId);

        if (questData == null || questState == null) return;

        questTitleText.text = questData.questName;
        questDescriptionText.text = questData.description;

        // Clear objectives
        foreach (Transform child in objectivesContainer)
            Destroy(child.gameObject);

        // Populate objectives
        foreach (var objective in questData.objectives)
        {
            if (objective.isHidden && !questState.IsObjectiveComplete(objective.objectiveId))
                continue;

            var objItem = Instantiate(objectiveItemPrefab, objectivesContainer);
            var objText = objItem.GetComponentInChildren<TextMeshProUGUI>();

            if (objText != null)
            {
                bool isComplete = questState.IsObjectiveComplete(objective.objectiveId);
                int progress = questState.GetObjectiveProgress(objective.objectiveId);

                string statusIcon = isComplete ? "✓" : "○";
                string progressText = objective.requiredAmount > 1
                    ? $" ({Mathf.Min(progress, objective.requiredAmount)}/{objective.requiredAmount})"
                    : "";

                objText.text = $"{statusIcon} {objective.description}{progressText}";
                objText.color = isComplete ? Color.green : Color.white;
            }
        }
    }

    private void OnQuestStatusChanged(string questId, QuestStatus status)
    {
        if (journalPanel.activeSelf)
            RefreshQuestList();
    }

    private void OnDestroy()
    {
        if (StoryProgressManager.Instance != null)
            StoryProgressManager.Instance.OnQuestStatusChanged -= OnQuestStatusChanged;
    }
}
