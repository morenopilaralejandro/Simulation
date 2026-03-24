using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[System.Serializable]
public class ChapterData
{
    public int chapterNumber;
    //location
    //public string chapterTitle;
    //public string chapterDescription;
    public string introEventId;
    public List<string> chapterQuestIds = new List<string>();
}
