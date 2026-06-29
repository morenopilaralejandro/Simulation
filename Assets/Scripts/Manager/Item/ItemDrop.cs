using UnityEngine;
using Aremoreno.Enums.Quest;
using Aremoreno.Enums.World;

[System.Serializable]
public class ItemDrop
{
    public string ItemId;
    public int QuantityMin = 1;
    public int QuantityMax = 1;
    [Range(0f, 1f)]
    public float DropChance = 1f;
}

/*
private void GenerateDrops(List<DropData> drops)
{
    foreach (var drop in drops)
    {
        if (Random.value <= drop.dropChance)
        {
            int quantity = Random.Range(drop.minQuantity, drop.maxQuantity + 1);
            // Award item to player
            InventoryManager.Instance.AddItem(drop.itemId, quantity);
        }
    }
}
*/
