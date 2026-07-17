using System;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Item;
using Aremoreno.Enums.Character;

public class ItemComponentCharacter
{
    private Item item;
    public string CharacterId { get; private set; }

    public ItemComponentCharacter(ItemDataCharacter itemDataCharacter, Item item, ItemSaveData itemSaveData = null)
    {
        this.item = item;
        CharacterId = itemDataCharacter.CharacterId;
    }
}
