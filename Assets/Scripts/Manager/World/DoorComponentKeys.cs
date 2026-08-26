using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.World;

public class DoorComponentKeys
{
    private List<ItemData> _requiredKeyDataList;
    private List<Item> _requiredKeyList;

    public IReadOnlyList<ItemData> RequiredKeyDataList => _requiredKeyDataList;
    public IReadOnlyList<Item> RequiredKeyList => _requiredKeyList;

    public DoorComponentKeys(List<ItemData> requiredKeyDataList)
    {
        Initialize(requiredKeyDataList);
    }

    public void Initialize(List<ItemData> requiredKeyDataList)
    {
        _requiredKeyDataList = new List<ItemData>(requiredKeyDataList);
        _requiredKeyList = new List<Item>();

        foreach (ItemData itemData in _requiredKeyDataList)
        {
            Item item = ItemFactory.CreateById(itemData.ItemId);
            _requiredKeyList.Add(item);
        }
    }

    public bool HasRequiredKeys()
    {
        foreach (Item key in _requiredKeyList)
        {
            if (!ItemManager.Instance.HasItem(key)) return false;
        }

        return true;
    }
}
