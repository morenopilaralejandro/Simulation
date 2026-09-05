using UnityEngine;
using Aremoreno.Enums.Character;

[CreateAssetMenu(fileName = "MaterialByGenderData", menuName = "ScriptableObject/Item/MaterialBy/MaterialByGenderData")]
public class MaterialByGenderData : ScriptableObject
{
    public Gender Gender;
    public ItemData Material;
}
