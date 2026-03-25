using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public static class QuestFactory
{
    public static Quest Create(QuestData data)
    {
        new Quest(data);
    }

    public static Quest CreateById(string questId) 
    {
        return Create(QuestDatabase.Instance.GetQuestData(questId));
    }
}
