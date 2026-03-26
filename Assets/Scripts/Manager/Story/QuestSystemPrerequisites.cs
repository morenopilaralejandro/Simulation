using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestSystemPrerequisites
{
    private QuestSystemManager questSystemManager;
    private StorySystemManager storySystemManager;

    private QuestSystemPrerequisites()
    {
        questSystemManager = QuestSystemManager.Instance;
        storySystemManager = StorySystemManager.Instance;
    }

    public bool CheckQuestPrerequisites(string questId)
    {
        Quest quest = questSystemManager.GetQuest(questId);
        if (quest == null) return true;

        return CheckPrerequisites(quest.StoryPrerequisites);
    }

    public bool CheckPrerequisites(IReadOnlyList<StoryPrerequisite> prerequisites)
    {
        if (prerequisites == null || prerequisites.Count == 0) return true;

        foreach (var prereq in prerequisites)
        {
            if (!EvaluatePrerequisite(prereq))
                return false;
        }
        return true;
    }

    public bool EvaluatePrerequisite(StoryPrerequisite prereq)
    {
        switch (prereq.PrerequisiteType)
        {
            case PrerequisiteType.FlagIsTrue:
                return storySystemManager.GetFlag(prereq.TargetId);

            case PrerequisiteType.FlagIsFalse:
                return !storySystemManager.GetFlag(prereq.TargetId);

            case PrerequisiteType.VariableEquals:
                return storySystemManager.GetVariable(prereq.TargetId) == prereq.IntValue;

            case PrerequisiteType.VariableGreaterThan:
                return storySystemManager.GetVariable(prereq.TargetId) > prereq.IntValue;

            case PrerequisiteType.VariableLessThan:
                return storySystemManager.GetVariable(prereq.TargetId) < prereq.IntValue;

            case PrerequisiteType.QuestCompleted:
                return questSystemManager.GetQuestState(prereq.TargetId) == QuestState.Completed;

            case PrerequisiteType.QuestActive:
                return questSystemManager.GetQuestState(prereq.TargetId) == QuestState.Active;

            case PrerequisiteType.QuestNotStarted:
                return questSystemManager.GetQuestState(prereq.TargetId) == QuestState.NotStarted;

            case PrerequisiteType.ChapterReached:
                return storySystemManager.CurrentChapter.StoryChapterNumber >= prereq.IntValue;

            case PrerequisiteType.EventCompleted:
                return storySystemManager.IsEventCompleted(prereq.TargetId);

            default:
                return true;
        }
    }

}
