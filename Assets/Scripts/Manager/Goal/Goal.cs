using UnityEngine;
using Simulation.Enums.Battle;

public class Goal : MonoBehaviour
{
    public Team Team;
    public GoalPlacement GoalPlacement;

    private void Awake()
    {
        if (GoalManager.Instance != null)
            GoalManager.Instance.AddGoal(this);
        
    }

    private void OnDestroy()
    {
        if (GoalManager.Instance != null)
            GoalManager.Instance.Reset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !BattleManager.Instance.IsTimeFrozen)
        {
            //GameManager.Instance.OnGoalScored(Team);
            LogManager.Info("[Goal] OnTriggerEnter: a goal was scored", this);
        }
    }

}
