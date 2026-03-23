using UnityEngine;
using Simulation.Enums.Item;

public static class ItemDataFactory
{
    public static ItemData CreateByCategory(ItemCategory category)
    {
        switch (category)
        {
            case ItemCategory.Equipment:
                return ScriptableObject.CreateInstance<ItemDataEquipment>();

            case ItemCategory.Formation:
                return ScriptableObject.CreateInstance<ItemDataFormation>();

            case ItemCategory.Important:
                return ScriptableObject.CreateInstance<ItemDataImportant>();

            case ItemCategory.Kit:
                return ScriptableObject.CreateInstance<ItemDataKit>();

            case ItemCategory.Material:
                return ScriptableObject.CreateInstance<ItemDataMaterial>();

            case ItemCategory.Misc:
                return ScriptableObject.CreateInstance<ItemDataMisc>();

            case ItemCategory.Move:
                return ScriptableObject.CreateInstance<ItemDataMove>();

            case ItemCategory.Recovery:
                return ScriptableObject.CreateInstance<ItemDataRecovery>();

            default:
                LogManager.Error($"[ItemDataFactory] No data class for {category.ToString()}.");
                return null;
        }
    }
}
