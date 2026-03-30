namespace Simulation.Enums.Quest
{
    public enum QuestState
    {
        NotStarted,
        Started,
        Active,
        Completed,
        Failed,
        Locked
    }

    public enum QuestType
    {
        MainQuest,
        SideQuest,
        DailyQuest,
        BasicQuest,
        HiddenQuest
    }

    public enum ObjectiveType
    {
        Talk,
        Interact,
        Defeat,
        Kill,
        Collect,
        Explore,
        Discover,
        Escort,
        Survive,
        Custom
    }
}
