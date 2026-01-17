using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class CharacterComponentKeeper : MonoBehaviour
{
    private Character character;

    [SerializeField] private Collider keeperCollider;   //inspector
    [SerializeField] private bool isKeeper;

    private float punchingRadius = 3f;
    private float punchingMaxAngle = 25f;

    public bool IsKeeper => isKeeper;

    public void Initialize(CharacterData characterData, Character character)
    {
        this.character = character;
    }

    public void UpdateKeeperColliderState()
    {
        if (keeperCollider != null)
            keeperCollider.enabled = isKeeper;
    }

    private void OnEnable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle += HandleAssignCharacterToTeamBattle;
    }

    private void OnDisable()
    {
        TeamEvents.OnAssignCharacterToTeamBattle -= HandleAssignCharacterToTeamBattle;
    }

    private void HandleAssignCharacterToTeamBattle(
        Character character, 
        Team team, 
        FormationCoord formationCoord)
    {
        if (this.character == character)
        {
            this.isKeeper = formationCoord.Position == Position.GK ? true : false;
            if (this.isKeeper)
                GoalManager.Instance.SetKeeper(this.character, team.TeamSide);
            UpdateKeeperColliderState();
        }
    }

    public void PunchBall(Trait trait)
    {
        switch (trait)
        {
            case Trait.Punch1:
                PunchRandomArc();
                break;

            case Trait.Punch2:
                Character teammate = character.GetBestPassTeammate();
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
        Vector3 direction = GetRandomDirectionInArc(character.transform);
        Vector3 punchPosition = center + direction * punchingRadius;
        PunchBallTo(punchPosition);
    }

    private void PunchToTeammate(Character teammate) 
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
}
