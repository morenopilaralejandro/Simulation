using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class GoalComponentTrigger : MonoBehaviour
{
    private Goal goal;
    [SerializeField] private BoxCollider goalCollider;

    public BoxCollider GoalCollider => goalCollider;


    public void Initialize(Goal goal)
    {
        this.goal = goal; 
        goalCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && 
            !BattleManager.Instance.IsTimeFrozen)
        {
            BattleManager.Instance.GoalScored(goal);
            other.GetComponent<Ball>().SlowDown();
            LogManager.Info("[GoalComponentTrigger] A goal was scored. TeamSide: {goal.TeamSide}", this);
        }
    }
}
