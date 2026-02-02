using UnityEngine;
using Simulation.Enums.Character;

[System.Serializable]
public struct FormationCoord
{
    private static readonly Quaternion FlippedRotation = Quaternion.Euler(0f, 180f, 0f);
    private static readonly Quaternion DefaultRot = Quaternion.identity;

    [SerializeField] private string formationCoordId;
    public string FormationCoordId => formationCoordId;

    [SerializeField] private Vector3 defaultPosition;
    public Vector3 DefaultPosition => defaultPosition;

    [SerializeField] private Quaternion defaultRotation;
    public Quaternion DefaultRotation => defaultRotation;

    [SerializeField] private Position position;
    public Position Position => position;

    public FormationCoord(string formationCoordId, Vector3 defaultPosition, Position position)
    {
        this.formationCoordId = formationCoordId;
        this.defaultPosition = defaultPosition;
        this.position = position;
        this.defaultRotation = DefaultRot;
    }

    public FormationCoord(FormationCoord other)
    {
        formationCoordId = other.formationCoordId;
        defaultPosition = other.defaultPosition;
        defaultRotation = other.defaultRotation;
        position = other.position;
    }

    public void FlipDefaultPosition()
    {
        defaultPosition.x = -defaultPosition.x;
        defaultPosition.z = -defaultPosition.z;
        defaultRotation = FlippedRotation;
    }
}
