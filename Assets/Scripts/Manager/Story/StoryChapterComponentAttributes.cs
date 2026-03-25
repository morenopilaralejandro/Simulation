using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StoryChapterComponentAttributes
{
    public string StoryChapterId { get; private set; }
    public int StoryChapterNumber { get; private set; }

    public StoryChapterComponentAttributes(StoryChapterData storyChapterData, StoryChapter storyChapter)
    {
        StoryChapterId = storyChapterData.StoryChapterId;
        StoryChapterNumber = storyChapterData.StoryChapterNumber;
    }
}
