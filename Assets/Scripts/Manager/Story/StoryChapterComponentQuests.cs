using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StoryChapterComponentQuests
{
    private QuestSystemManager questSystemManager;
    
    private List<string> chapterQuestIds = new List<string>();
    public IReadOnlyList<string> ChapterQuestIds => chapterQuestIds;

    public StoryChapterComponentQuests(StoryChapterData storyChapterData, StoryChapter storyChapter)
    {
        questSystemManager = QuestSystemManager.Instance;
        chapterQuestIds = storyChapterData.ChapterQuestIds;
    }

}
