using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

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
