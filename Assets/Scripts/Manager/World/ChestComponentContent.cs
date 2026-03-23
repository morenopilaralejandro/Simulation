using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.World;

public class ChestComponentContent
{
    public ItemData ItemData { get; private set; }
    public string ItemId { get; private set; }

    public ChestComponentContent(ItemData itemData)
    {
        Initialize(itemData);
    }

    public void Initialize(ItemData itemData)
    {
        ItemData = itemData;
        ItemId = itemData.ItemId;
        //Item = ItemFactory.Create(itemData);
    }
}
