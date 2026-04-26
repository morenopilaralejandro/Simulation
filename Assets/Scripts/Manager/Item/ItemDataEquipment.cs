using UnityEngine;
using Aremoreno.Enums.Item;

[CreateAssetMenu(fileName = "ItemDataEquipment", menuName = "ScriptableObject/Item/ItemDataEquipment")]
public class ItemDataEquipment : ItemData
{
    public EquipmentType EquipmentType;
    public int EquipmentHp;
    public int EquipmentSp;
    public int EquipmentKick;
    public int EquipmentBody;
    public int EquipmentControl;
    public int EquipmentGuard;
    public int EquipmentSpeed;
    public int EquipmentStamina;
    public int EquipmentTechnique;
    public int EquipmentLuck;
    public int EquipmentCourage;
}
