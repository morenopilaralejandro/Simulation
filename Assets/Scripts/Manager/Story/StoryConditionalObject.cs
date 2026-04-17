using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

/// <summary>
/// Activates/deactivates GameObjects based on story conditions.
/// Useful for NPCs, doors, cutscene triggers, etc.
/// </summary>
public class StoryConditionalObject : MonoBehaviour
{
    [Header("Conditions")]
    [SerializeField] private List<StoryPrerequisite> showConditions = new List<StoryPrerequisite>();
    [SerializeField] private bool hideWhenConditionsMet = false;

    [Header("Target")]
    [SerializeField] private GameObject targetObject;

    private QuestSystemManager questSystemManager;

    private void Start()
    {
        questSystemManager = QuestSystemManager.Instance;
        EvaluateConditions();
        SubscribeEvents();
    }

    private void EvaluateConditions()
    {
        bool conditionsMet = questSystemManager.CheckPrerequisites(showConditions);
        bool shouldBeActive = hideWhenConditionsMet ? !conditionsMet : conditionsMet;

        if (targetObject.activeSelf != shouldBeActive)
            targetObject.SetActive(shouldBeActive);
    }

    #region Events

    private void SubscribeEvents() 
    {
        StoryEvents.OnStoryFlagChanged += HandleStoryFlagChanged;
        StoryEvents.OnStoryVariableChanged += HandleStoryVariableChanged;
        StoryEvents.OnChapterChanged += HandleChapterChanged;
        StoryEvents.OnStoryEventTriggered += HandleStoryEventTriggered;
        QuestEvents.OnQuestStateChanged += HandleQuestStateChanged;
    }

    private void OnDestroy()
    {
        StoryEvents.OnStoryFlagChanged -= HandleStoryFlagChanged;
        StoryEvents.OnStoryVariableChanged -= HandleStoryVariableChanged;
        StoryEvents.OnChapterChanged -= HandleChapterChanged;
        StoryEvents.OnStoryEventTriggered -= HandleStoryEventTriggered;
        QuestEvents.OnQuestStateChanged -= HandleQuestStateChanged;
    }

    private void HandleStoryFlagChanged(string flagId) => EvaluateConditions();
    private void HandleStoryVariableChanged(string varibaleId, int intValue) => EvaluateConditions();
    private void HandleChapterChanged(StoryChapter storyChapter) => EvaluateConditions();
    private void HandleStoryEventTriggered(string storyEventId) => EvaluateConditions();
    private void HandleQuestStateChanged(Quest quest) => EvaluateConditions();

    #endregion

}
