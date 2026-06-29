using UnityEngine;
using Aremoreno.Enums.Field;

[CreateAssetMenu(fileName = "FieldData", menuName = "ScriptableObject/FieldData")]
public class FieldData : ScriptableObject
{
    public string FieldId;
    public FieldLineColor FieldLineColor;
    public string TextureInnerAddress;
    public string TextureOuterAddress;
}
