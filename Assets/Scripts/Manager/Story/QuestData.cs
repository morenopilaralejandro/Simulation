using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObject/Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string QuestId;
    //location
    //public string questName;
    //public string description;
    public QuestType questType;
    public int recommendedLevel;

    [Header("Objectives")]
    public List<QuestObjective> objectives = new List<QuestObjective>();

    [Header("Prerequisites")]
    public List<StoryPrerequisite> prerequisites = new List<StoryPrerequisite>();

    [Header("Rewards")]
    public QuestRewards rewards;

    [Header("Flow")]
    public List<string> followUpQuestIds = new List<string>();

    /*
    [Header("Dialogue")]
    public string startDialogueId;
    public string completeDialogueId;
    public string inProgressDialogueId;
    */
}
