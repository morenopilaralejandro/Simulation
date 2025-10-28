using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class GoalComponentTrigger : MonoBehaviour
{
    private Goal goal;

    public void Initialize(Goal goal)
    {
        this.goal = goal; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && 
            !BattleManager.Instance.IsTimeFrozen)
        {
            BattleManager.Instance.GoalScored(goal);
            LogManager.Info("[GoalComponentTrigger] A goal was scored. TeamSide: {goal.TeamSide}", this);
        }
    }
}
