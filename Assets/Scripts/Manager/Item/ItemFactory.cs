using Aremoreno.Enums.Item;

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
        auxItemData = DatabaseManager.Instance.GetItemData(itemId);
        return CreateByIdAndCategory(auxItemData.ItemId, auxItemData.Category);
    }

    public static Item CreateByIdAndCategory(string itemId, ItemCategory category) 
    {
        auxItemData = category switch
        {
            ItemCategory.Equipment  => DatabaseManager.Instance.GetItemData<ItemDataEquipment>(itemId),
            ItemCategory.Formation  => DatabaseManager.Instance.GetItemData<ItemDataFormation>(itemId),
            ItemCategory.Important  => DatabaseManager.Instance.GetItemData<ItemDataImportant>(itemId),
            ItemCategory.Kit        => DatabaseManager.Instance.GetItemData<ItemDataKit>(itemId),
            ItemCategory.Material   => DatabaseManager.Instance.GetItemData<ItemDataMaterial>(itemId),
            ItemCategory.Misc       => DatabaseManager.Instance.GetItemData<ItemDataMisc>(itemId),
            ItemCategory.Move       => DatabaseManager.Instance.GetItemData<ItemDataMove>(itemId),
            ItemCategory.Recovery   => DatabaseManager.Instance.GetItemData<ItemDataRecovery>(itemId),
            _ => null
        };

        return Create(auxItemData);
    }
}
