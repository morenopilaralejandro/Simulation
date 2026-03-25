using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemChapters
{
    private StoryChapterDatabase storyChapterDatabase;

    private StoryChapters currentChapter;
    public StoryChapters CurrentChapter => currentChapter;


    public StorySystemChapters() 
    {

    }

    public void AdvanceChapter()
    {
        int auxInt = currentChapter.StoryChapterNumber;
        auxInt++;
        currentChapter = storyChapterDatabase.GetStoryChapter(auxInt);
        StoryEvents.RaiseChapterChanged(currentChapter.StoryChapterId);
        LogManager.Trace($"[StorySystemChapters] Advanced to Chapter {auxInt}");
    }

    public void SetChapter(int intValue)
    {
        currentChapter = storyChapterDatabase.GetStoryChapter(intValue);
        StoryEvents.RaiseChapterChanged(currentChapter.StoryChapterId);
    }
}
