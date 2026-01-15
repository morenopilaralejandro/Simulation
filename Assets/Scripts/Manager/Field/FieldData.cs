using UnityEngine;

[CreateAssetMenu(fileName = "FieldData", menuName = "ScriptableObject/FieldData")]
public class FieldData : ScriptableObject
{
    public string FieldId;
    public Color LineColor;
    public Texture TextureInner;
    public Texture TextureOuter;
}
