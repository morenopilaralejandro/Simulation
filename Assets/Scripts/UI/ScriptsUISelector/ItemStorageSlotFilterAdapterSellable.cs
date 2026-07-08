using UnityEngine;

public class ItemStorageSlotFilterAdapterSellable : ISelectorFilter<ItemStorageSlot>
{
    //private readonly CharacterFilterData data;
    //public CharacterFilterAdapter(CharacterFilterData data) => this.data = data;
    public bool Matches(ItemStorageSlot obj) => obj.Item.IsSellable;
}
