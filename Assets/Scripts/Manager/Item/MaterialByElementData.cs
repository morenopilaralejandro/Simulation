using UnityEngine;
using Aremoreno.Enums.Character;

[CreateAssetMenu(fileName = "MaterialByElementData", menuName = "ScriptableObject/Item/MaterialBy/MaterialByElementData")]
public class MaterialByElementData : ScriptableObject
{
    public Element Element;
    public ItemData Material;
}
