using UnityEngine;
using Aremoreno.Enums.World;
using Aremoreno.Enums.Animation;

[System.Serializable]
public class PlayerWorldSaveData
{
    public string lastScene;
    public Vector3 lastPosition;
    public CharacterDirection lastFacing;
    public float playTime;
    //money etc
}
