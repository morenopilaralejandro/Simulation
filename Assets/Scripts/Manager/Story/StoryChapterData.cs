using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.Story;

[CreateAssetMenu(fileName = "StoryChapterData", menuName = "ScriptableObject/Story/StoryChapterData")]
public class StoryChapterData : ScriptableObject
{
    public string StoryChapterId;
    public int StoryChapterNumber; //location title description
    public string IntroEventId;
    public List<string> ChapterQuestIds = new List<string>();
}
