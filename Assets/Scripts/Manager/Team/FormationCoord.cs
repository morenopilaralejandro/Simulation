using UnityEngine;
using Simulation.Enums.Character;

public class FormationCoord
{
    [SerializeField] private string formationCoordId;
    public string FormationCoordId => formationCoordId;

    [SerializeField] private Vector3 defaultPosition;
    public Vector3 DefaultPosition => defaultPosition;

    [SerializeField] private Position position;
    public Position Position => position;

    public FormationCoord(string formationCoordId, Vector3 defaultPosition, Position position)
    {
        this.formationCoordId = formationCoordId;
        this.defaultPosition = defaultPosition;
        this.position = position;
    }

    public FormationCoord(FormationCoord other)
    {
        this.formationCoordId = other.formationCoordId;
        this.defaultPosition = other.defaultPosition;
        this.position = other.position;
    }

    public void FlipDefaultPosition()
    {
        defaultPosition.x *= -1;
        defaultPosition.z *= -1;
    }
}
