using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Item;

[System.Serializable]
public class SaveDataCurrencySystem
{
    public List<SerializableKeyValue<CurrencyType, int>> CurrencyList;
}
