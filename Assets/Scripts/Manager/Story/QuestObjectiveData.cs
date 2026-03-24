using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[System.Serializable]
public class QuestObjective
{
    public string objectiveId;
    //public string description; location
    public ObjectiveType objectiveType;
    public string targetId; // e.g., enemy ID, item ID, NPC ID, location ID
    public int requiredAmount = 1;
    public bool isOptional;
    public bool isHidden; // revealed during gameplay
}
