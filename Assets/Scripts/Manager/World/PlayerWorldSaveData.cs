using UnityEngine;
using Simulation.Enums.World;

[System.Serializable]
public class PlayerWorldSaveData
{
    public string lastScene;
    public Vector3 lastPosition;
    public FacingDirection lastFacing;
    public float playTime;
    //money etc
}
