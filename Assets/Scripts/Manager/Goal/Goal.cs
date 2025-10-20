using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class Goal : MonoBehaviour
{
    [SerializeField] private TeamSide teamSide;

    public TeamSide TeamSide => teamSide;

    private void Awake()
    {
        GoalManager.Instance.SetGoal(this);   
    }

    private void OnDestroy()
    {
        GoalManager.Instance.Reset();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball") && !BattleManager.Instance.IsTimeFrozen)
        {
            //GameManager.Instance.OnGoalScored(Team);
            LogManager.Info("[Goal] OnTriggerEnter: a goal was scored. TeamSide: {teamSide}", this);
        }
    }

}
