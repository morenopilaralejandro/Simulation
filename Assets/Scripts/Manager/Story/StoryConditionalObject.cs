using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

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
    [SerializeField] private float checkInterval = 1f;

    private float checkTimer;

    private void Start()
    {
        if (targetObject == null)
            targetObject = gameObject;

        EvaluateConditions();

        // Listen for changes
        StoryProgressManager.Instance.OnStoryFlagChanged += _ => EvaluateConditions();
        StoryProgressManager.Instance.OnQuestStatusChanged += (_, __) => EvaluateConditions();
        StoryProgressManager.Instance.OnChapterChanged += _ => EvaluateConditions();
    }

    private void Update()
    {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0)
        {
            checkTimer = checkInterval;
            EvaluateConditions();
        }
    }

    private void EvaluateConditions()
    {
        bool conditionsMet = StoryProgressManager.Instance.CheckPrerequisites(showConditions);
        bool shouldBeActive = hideWhenConditionsMet ? !conditionsMet : conditionsMet;

        if (targetObject.activeSelf != shouldBeActive)
            targetObject.SetActive(shouldBeActive);
    }

    private void OnDestroy()
    {
        if (StoryProgressManager.Instance != null)
        {
            StoryProgressManager.Instance.OnStoryFlagChanged -= _ => EvaluateConditions();
            StoryProgressManager.Instance.OnQuestStatusChanged -= (_, __) => EvaluateConditions();
            StoryProgressManager.Instance.OnChapterChanged -= _ => EvaluateConditions();
        }
    }
}
