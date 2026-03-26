using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObject/Quest/QuestData")]
public class QuestData : ScriptableObject
{
    public string QuestId; //location: name, description
    public QuestType QuestType;
    public int RecommendedLevel;

    [Header("Objectives")]
    public List<string> ObjectiveIds = new List<string>();

    [Header("Prerequisites")]
    public List<StoryPrerequisite> StoryPrerequisites = new List<StoryPrerequisite>();

    [Header("Rewards")]
    public int RewardExp;
    public int RewardGold;
    public List<ItemReward> ItemRewards = new List<ItemReward>();
    public List<StoryEffect> StoryEffects = new List<StoryEffect>();

    [Header("Flow")]
    public List<string> FollowUpQuestIds = new List<string>();

    /*
    [Header("Dialogue")]
    public string startDialogueId;
    public string completeDialogueId;
    public string inProgressDialogueId;
    */
}
