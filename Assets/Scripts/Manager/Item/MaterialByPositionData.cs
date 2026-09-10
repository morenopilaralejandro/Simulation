using UnityEngine;
using Aremoreno.Enums.Character;

[CreateAssetMenu(fileName = "MaterialByPositionData", menuName = "ScriptableObject/Item/MaterialBy/MaterialByPositionData")]
public class MaterialByPositionData : ScriptableObject
{
    public Position Position;
    public ItemData Material;
}
