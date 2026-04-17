using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

/// <summary>
/// Bridge between dialogue system and story progress.
/// Call these methods from dialogue node callbacks.
/// </summary>
public class StoryDialogueBridge : MonoBehaviour
{
    /*
    public static StoryDialogueBridge Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Call from dialogue choices
    public void OnDialogueChoice(string choiceId)
    {
        var manager = StoryProgressManager.Instance;

        // Set a flag based on the choice
        manager.SetFlag($"choice_{choiceId}", true);
        Debug.Log($"[Dialogue] Choice made: {choiceId}");
    }

    public void SetStoryFlag(string flagName)
    {
        StoryProgressManager.Instance.SetFlag(flagName, true);
    }

    public void AddRelationship(string npcId, int amount)
    {
        StoryProgressManager.Instance.IncrementVariable($"relationship_{npcId}", amount);
    }

    public void TriggerEvent(string eventId)
    {
        StoryProgressManager.Instance.TriggerStoryEvent(eventId);
    }

    public void StartQuest(string questId)
    {
        StoryProgressManager.Instance.StartQuest(questId);
    }

    public bool CheckCondition(string flagName)
    {
        return StoryProgressManager.Instance.GetFlag(flagName);
    }

    public int GetRelationship(string npcId)
    {
        return StoryProgressManager.Instance.GetVariable($"relationship_{npcId}");
    }
    */
}
