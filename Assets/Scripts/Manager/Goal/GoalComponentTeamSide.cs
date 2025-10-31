using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class GoalComponentTeamSide : MonoBehaviour
{
    private Goal goal;

    [SerializeField] private TeamSide teamSide; //inspector

    public TeamSide TeamSide => teamSide;


    private void OnDestroy()
    {
        GoalManager.Instance.Reset();
    }

    public void Initialize(Goal goal)
    {
        
        this.goal = goal;
        GoalManager.Instance.SetGoal(this.goal);  
    }
}
