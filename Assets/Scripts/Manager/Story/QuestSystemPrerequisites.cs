using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Quest;
using Simulation.Enums.Story;

public class QuestSystemPrerequisites
{
    private QuestSystemManager manager;

    private QuestSystemPrerequisites()
    {
        manager = QuestSystemManager.Instance;
    }

    public bool CheckQuestPrerequisites(string questId)
    {
        Quest quest = manager.GetQuest(questId);
        if (quest == null) return true;

        return CheckPrerequisites(quest.Prerequisites);
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
        switch (prereq.type)
        {
            case PrerequisiteType.FlagIsTrue:
                return GetFlag(prereq.targetName);

            case PrerequisiteType.FlagIsFalse:
                return !GetFlag(prereq.targetName);

            case PrerequisiteType.VariableEquals:
                return GetVariable(prereq.targetName) == prereq.intValue;

            case PrerequisiteType.VariableGreaterThan:
                return GetVariable(prereq.targetName) > prereq.intValue;

            case PrerequisiteType.VariableLessThan:
                return GetVariable(prereq.targetName) < prereq.intValue;

            case PrerequisiteType.QuestCompleted:
                return GetQuestStatus(prereq.targetName) == QuestStatus.Completed;

            case PrerequisiteType.QuestActive:
                return GetQuestStatus(prereq.targetName) == QuestStatus.Active;

            case PrerequisiteType.QuestNotStarted:
                return GetQuestStatus(prereq.targetName) == QuestStatus.NotStarted;

            case PrerequisiteType.ChapterReached:
                return currentChapter >= prereq.intValue;

            case PrerequisiteType.EventCompleted:
                return IsEventCompleted(prereq.targetName);

            default:
                return true;
        }
    }

}
