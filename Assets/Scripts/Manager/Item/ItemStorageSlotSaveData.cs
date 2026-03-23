using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Item;

[System.Serializable]
public class ItemStorageSlotSaveData
{
    public string ItemId;
    public ItemCategory Category;
    public int Count;
}
