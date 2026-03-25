using Simulation.Enums.Item;

public static class ItemFactory
{
    private static ItemData auxItemData;

    public static Item Create(ItemData data)
    {
        switch (data)
        {
            case ItemDataEquipment itemDataEquipment:
                return new ItemEquipment(itemDataEquipment);

            case ItemDataFormation itemDataFormation:
                return new ItemFormation(itemDataFormation);

            case ItemDataImportant itemDataImportant:
                return new ItemImportant(itemDataImportant);

            case ItemDataKit itemDataKit:
                return new ItemKit(itemDataKit);

            case ItemDataMaterial itemDataMaterial:
                return new ItemMaterial(itemDataMaterial);

            case ItemDataMisc itemDataMisc:
                return new ItemMisc(itemDataMisc);

            case ItemDataMove itemDataMove:
                return new ItemMove(itemDataMove);

            case ItemDataRecovery itemDataRecovery:
                return new ItemRecovery(itemDataRecovery);

            default:
                LogManager.Warning($"[ItemFactory] No runtime class for {data.GetType().Name}. Using base Item.");
                return new Item(data);
        }
    }

    public static Item CreateById(string itemId) 
    {
        auxItemData = ItemManager.Instance.GetItemData(itemId);
        return Create(auxItemData);
    }

    public static Item CreateByIdAndCategory(string itemId, ItemCategory category) 
    {
        auxItemData = category switch
        {
            ItemCategory.Equipment  => ItemManager.Instance.GetItemData<ItemDataEquipment>(itemId),
            ItemCategory.Formation  => ItemManager.Instance.GetItemData<ItemDataFormation>(itemId),
            ItemCategory.Important  => ItemManager.Instance.GetItemData<ItemDataImportant>(itemId),
            ItemCategory.Kit        => ItemManager.Instance.GetItemData<ItemDataKit>(itemId),
            ItemCategory.Material   => ItemManager.Instance.GetItemData<ItemDataMaterial>(itemId),
            ItemCategory.Misc       => ItemManager.Instance.GetItemData<ItemDataMisc>(itemId),
            ItemCategory.Move       => ItemManager.Instance.GetItemData<ItemDataMove>(itemId),
            ItemCategory.Recovery   => ItemManager.Instance.GetItemData<ItemDataRecovery>(itemId),
            _ => null
        };

        return Create(auxItemData);
    }
}
