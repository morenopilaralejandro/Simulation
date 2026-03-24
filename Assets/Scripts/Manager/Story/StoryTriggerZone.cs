using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryTriggerZone : MonoBehaviour
{
    [Header("Trigger Settings")]
    [SerializeField] private string triggerId;
    [SerializeField] private bool triggerOnce = true;
    [SerializeField] private bool requireInteraction = false;
    [SerializeField] private string playerTag = "Player";

    [Header("Prerequisites")]
    [SerializeField] private List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();

    [Header("On Trigger")]
    [SerializeField] private string storyEventId;
    [SerializeField] private string flagToSet;
    [SerializeField] private string questToStart;
    [SerializeField] private string questObjectiveToComplete;
    [SerializeField] private string questIdForObjective;

    [Header("Unity Events")]
    [SerializeField] private UnityEvent onTriggered;
    [SerializeField] private UnityEvent onPrerequisitesNotMet;

    private bool hasTriggered = false;
    private bool playerInZone = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        playerInZone = true;

        if (!requireInteraction)
            TryTrigger();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
            playerInZone = false;
    }

    private void Update()
    {
        if (requireInteraction && playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TryTrigger();
        }
    }

    public void TryTrigger()
    {
        if (triggerOnce && hasTriggered) return;

        var manager = StoryProgressManager.Instance;

        if (!manager.CheckPrerequisites(prerequisites))
        {
            onPrerequisitesNotMet?.Invoke();
            return;
        }

        hasTriggered = true;
        ExecuteTrigger();
    }

    private void ExecuteTrigger()
    {
        var manager = StoryProgressManager.Instance;

        if (!string.IsNullOrEmpty(storyEventId))
            manager.TriggerStoryEvent(storyEventId);

        if (!string.IsNullOrEmpty(flagToSet))
            manager.SetFlag(flagToSet, true);

        if (!string.IsNullOrEmpty(questToStart))
            manager.StartQuest(questToStart);

        if (!string.IsNullOrEmpty(questObjectiveToComplete) && !string.IsNullOrEmpty(questIdForObjective))
            manager.CompleteQuestObjective(questIdForObjective, questObjectiveToComplete);

        onTriggered?.Invoke();
    }
}
