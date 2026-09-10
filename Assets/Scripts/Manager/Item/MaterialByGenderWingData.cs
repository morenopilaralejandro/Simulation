using UnityEngine;
using Aremoreno.Enums.Character;

[CreateAssetMenu(fileName = "MaterialByGenderWingData", menuName = "ScriptableObject/Item/MaterialBy/MaterialByGenderWingData")]
public class MaterialByGenderWingData : ScriptableObject
{
    public Gender Gender;
    public ItemData Material;
}
