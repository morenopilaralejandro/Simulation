using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentTeamMember : MonoBehaviour
{
    [SerializeField] private int teamIndex;
    [SerializeField] private FormationCoord formationCoord;

    public int GetTeamIndex() => teamIndex;
    public FormationCoord GetFormationCoord() => formationCoord;
}
