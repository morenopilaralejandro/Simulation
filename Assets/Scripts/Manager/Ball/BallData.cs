using UnityEngine;

[CreateAssetMenu(fileName = "BallData", menuName = "ScriptableObject/BallData")]
public class BallData : ScriptableObject
{
    public string BallId;
    public Texture Texture;
}
