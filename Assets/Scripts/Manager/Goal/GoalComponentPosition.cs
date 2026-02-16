using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class GoalComponentPosition : MonoBehaviour
{
    private Goal goal;

    [SerializeField] private Transform goalTransform; //inspector

    private float zHomeFull = -8;
    private float zHomeMini = -5;
    private BattleType battleType;

    private void OnDestroy()
    {

    }

    public void Initialize(Goal goal)
    {
        this.goal = goal;
        battleType = BattleManager.Instance.CurrentType;
        SetGoalPosition();
    }

    private void SetGoalPosition()
    {
        float zPosition = battleType == BattleType.Full ? zHomeFull : zHomeMini;
        if (goal.TeamSide == TeamSide.Away)
            zPosition *= -1;

        goalTransform.position = new Vector3(
            goalTransform.position.x,
            goalTransform.position.y,
            zPosition
        );
    }
}
