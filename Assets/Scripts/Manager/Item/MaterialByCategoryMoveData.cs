using UnityEngine;
using Aremoreno.Enums.Move;

[CreateAssetMenu(fileName = "MaterialByCategoryMoveData", menuName = "ScriptableObject/Item/MaterialBy/MaterialByCategoryMoveData")]
public class MaterialByCategoryMoveData : ScriptableObject
{
    public Category Category;
    public ItemData Material;
}
