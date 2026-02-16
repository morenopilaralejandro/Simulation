using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using System.Collections;

public class CharacterComponentKeeper : MonoBehaviour
{
    #region Fields

    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private Collider keeperCollider;   //inspector
    [SerializeField] private bool isKeeper;
    [SerializeField] private bool hasBallInHand;

    private float punchingRadius = 3f;
    private float punchingMaxAngle = 25f;
    private float durationBallInHand = 2f;
    private Coroutine hasBallInHandCoroutine;

    public bool IsKeeper => isKeeper;
    public bool HasBallInHand => hasBallInHand;

    #endregion

    #region Lifecycle

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    #endregion

    #region Events
    
    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;
        BallEvents.OnReleased += HandleOnReleased;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
        BallEvents.OnReleased += HandleOnReleased;
    }

    private void HandleAssignCharacterToTeamBattle(
        CharacterEntityBattle character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.characterEntityBattle == character)
        {
            this.isKeeper = formationCoord.Position == Position.GK ? true : false;
            if (this.isKeeper)
                GoalManager.Instance.SetKeeper(this.characterEntityBattle, team.TeamSide);
            UpdateKeeperColliderState();
        }
    }

    private void HandleOnReleased(CharacterEntityBattle character)
    {
        if (!hasBallInHand) return;
        if (this.characterEntityBattle != character) return;

        DeactivateBallInHand();
    }

    #endregion

    #region Collider

    public void UpdateKeeperColliderState()
    {
        if (keeperCollider != null)
            keeperCollider.enabled = isKeeper;
    }

    #endregion

    #region Punching tonchi wo kika sete Sliding

    public void PunchBall(Trait trait)
    {
        switch (trait)
        {
            case Trait.Punch1:
                PunchRandomArc();
                break;

            case Trait.Punch2:
                CharacterEntityBattle teammate = characterEntityBattle.GetBestPassTeammate();
                if (teammate != null)
                    PunchToTeammate(teammate);
                else
                    PunchRandomArc();
                break;
        }
    }

    private void PunchRandomArc() 
    {
        Vector3 center = BattleManager.Instance.Ball.transform.position;
        Vector3 direction = GetRandomDirectionInArc(characterEntityBattle.transform);
        Vector3 punchPosition = center + direction * punchingRadius;
        PunchBallTo(punchPosition);
    }

    private void PunchToTeammate(CharacterEntityBattle teammate) 
    {
        PunchBallTo(teammate.transform.position);
    }

    private void PunchBallTo(Vector3 targetPos) 
    {
        BattleManager.Instance.Ball.KickBallTo(targetPos);
        //BattleEvents.RaisePassPerformed(this);
        //StartKick();
    }

    private Vector3 GetRandomDirectionInArc(Transform transform)
    {
        // Random angle within the arc
        float angle = Random.Range(-punchingMaxAngle, punchingMaxAngle);

        // Rotate forward vector around the Y axis (for ground-based characters)
        Quaternion rotation = Quaternion.AngleAxis(angle, transform.up);
        Vector3 orientation = 
            transform.position.z > 0 ?
                -transform.forward :
                transform.forward;

        return rotation * orientation;
    }

    #endregion

    #region HasBallInHand
    public void ActivateBallInHand()
    {
        hasBallInHand = true;

        if (hasBallInHandCoroutine != null)
            StopCoroutine(hasBallInHandCoroutine);

        hasBallInHandCoroutine = StartCoroutine(HasBallInHandTimer());
    }

    private void DeactivateBallInHand()
    {
        if (hasBallInHandCoroutine != null)
        {
            StopCoroutine(hasBallInHandCoroutine);
            hasBallInHandCoroutine = null;
        }

        hasBallInHand = false;
    }

    private IEnumerator HasBallInHandTimer()
    {
        yield return new WaitForSeconds(durationBallInHand);

        hasBallInHand = false;
        hasBallInHandCoroutine = null;
    }
    #endregion
}
