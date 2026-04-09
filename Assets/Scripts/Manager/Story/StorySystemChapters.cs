using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class StorySystemChapters
{
    private StoryChapterDatabase storyChapterDatabase;

    private StoryChapter currentChapter;
    public StoryChapter CurrentChapter => currentChapter;


    public StorySystemChapters() 
    {
        storyChapterDatabase = StoryChapterDatabase.Instance;
    }

    public void AdvanceChapter()
    {
        int auxInt = currentChapter.StoryChapterNumber;
        auxInt++;
        currentChapter = storyChapterDatabase.GetStoryChapter(auxInt);
        StoryEvents.RaiseChapterChanged(currentChapter);
        LogManager.Trace($"[StorySystemChapters] Advanced to Chapter {auxInt}");
    }

    public void SetChapter(int intValue)
    {
        currentChapter = storyChapterDatabase.GetStoryChapter(intValue);
        StoryEvents.RaiseChapterChanged(currentChapter);
    }

    public void Import(StorySystemSaveData saveData) 
    {
        currentChapter = storyChapterDatabase.GetStoryChapter(saveData.ChapterNumber);
    }
}
