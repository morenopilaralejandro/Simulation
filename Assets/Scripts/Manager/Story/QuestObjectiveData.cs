using Simulation.Enums.Quest;
using Simulation.Enums.Story;

[CreateAssetMenu(fileName = "QuestObjectiveData", menuName = "ScriptableObject/Quest/QuestObjectiveData")]
public class QuestObjectiveData: ScriptableObject
{
    public string ObjectiveId; //location: description
    public ObjectiveType ObjectiveType;
    public string TargetId; // e.g., enemy ID, item ID, NPC ID, location ID
    public int RequiredAmount = 1; //RequiredAmount of progress to consider completed. Complements CurrentAmount
    public bool IsOptional;
    public bool IsHidden; // revealed during gameplay
}
