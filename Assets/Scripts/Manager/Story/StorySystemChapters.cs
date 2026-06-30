using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

public class StorySystemChapters
{
    private DatabaseManager db;

    private StoryChapter currentChapter;
    public StoryChapter CurrentChapter => currentChapter;


    public StorySystemChapters() 
    {
        db = DatabaseManager.Instance;
    }

    public void AdvanceChapter()
    {
        int auxInt = currentChapter.StoryChapterNumber;
        auxInt++;
        currentChapter = db.GetStoryChapter(auxInt.ToString());
        StoryEvents.RaiseChapterChanged(currentChapter);
        LogManager.Trace($"[StorySystemChapters] Advanced to Chapter {auxInt}");
    }

    public void SetChapter(int intValue)
    {
        currentChapter = db.GetStoryChapter(intValue.ToString());
        StoryEvents.RaiseChapterChanged(currentChapter);
    }

    public void Import(StorySystemSaveData saveData) 
    {
        currentChapter = db.GetStoryChapter(saveData.ChapterNumber.ToString());
    }
}
