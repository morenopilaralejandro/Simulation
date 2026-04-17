using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using Aremoreno.Enums.Localization;

[System.Serializable]
public struct TableMapping
{
    public LocalizationEntity Entity;
    public LocalizationField Field;
    public LocalizationStyle Style;
    public TableReference Table;
}
