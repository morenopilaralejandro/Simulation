using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Battle;

public class Goal : MonoBehaviour
{
    #region Components
    [SerializeField] private GoalComponentTeamSide teamSideComponent;
    [SerializeField] private GoalComponentTrigger triggerComponent;
    #endregion

    #region Initialize
    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        teamSideComponent.Initialize(this);
        triggerComponent.Initialize(this);
    }
    #endregion

    #region API
    //teamSideComponent
    public TeamSide TeamSide => teamSideComponent.TeamSide;
    //triggerComponent
    public BoxCollider GoalCollider => triggerComponent.GoalCollider;
    #endregion

}
