namespace Simulation.Enums.Story
{
    public enum PrerequisiteType
    {
        FlagIsTrue,
        FlagIsFalse,
        VariableEquals,
        VariableGreaterThan,
        VariableLessThan,
        QuestCompleted,
        QuestActive,
        QuestNotStarted,
        QuestStarted,
        ChapterReached,
        EventCompleted
    }

    public enum StoryEffectType
    {
        SetFlag,
        SetVariable,
        IncrementVariable,
        StartQuest,
        CompleteQuest,
        AdvanceChapter
    }
}
