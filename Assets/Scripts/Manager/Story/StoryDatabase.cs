using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[CreateAssetMenu(fileName = "StoryDatabase", menuName = "RPG/Story/Story Database")]
public class StoryDatabase : ScriptableObject
{
    [Header("Quests")]
    public List<QuestData> allQuests = new List<QuestData>();

    [Header("Story Events")]
    public List<StoryEvent> allStoryEvents = new List<StoryEvent>();

    [Header("Auto-Triggers")]
    public List<StoryAutoTrigger> autoTriggers = new List<StoryAutoTrigger>();

    [Header("Chapters")]
    public List<ChapterData> chapters = new List<ChapterData>();

    // Cached lookups
    private Dictionary<string, QuestData> questLookup;
    private Dictionary<string, StoryEvent> eventLookup;

    public void Initialize()
    {
        questLookup = allQuests.ToDictionary(q => q.questId, q => q);
        eventLookup = allStoryEvents.ToDictionary(e => e.eventId, e => e);
    }

    public QuestData GetQuest(string questId)
    {
        if (questLookup == null) Initialize();
        return questLookup.TryGetValue(questId, out var quest) ? quest : null;
    }

    public StoryEvent GetStoryEvent(string eventId)
    {
        if (eventLookup == null) Initialize();
        return eventLookup.TryGetValue(eventId, out var evt) ? evt : null;
    }

    public List<QuestData> GetQuestsByType(QuestType type)
    {
        return allQuests.Where(q => q.questType == type).ToList();
    }
}

