using System;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Item;

public class ItemManagerCurrency
{
    #region Fields

    private Dictionary<CurrencyType, int> currencyDict = new();
    public IReadOnlyDictionary<CurrencyType, int> CurrencyDict => currencyDict;

    private readonly Dictionary<CurrencyType, int> maxCapDict = new()
    {
        { CurrencyType.Gold, 9999999 },
        { CurrencyType.Token, 9999999 },
    };

    #endregion

    #region Constructor

    public ItemManagerCurrency()
    {

    }

    private void InitializeDefault() 
    {
        foreach (CurrencyType type in System.Enum.GetValues(typeof(CurrencyType)))
            currencyDict[type] = 0;
    }

    #endregion

    #region Logic

    public int GetGold()
    {
        return GetAmount(CurrencyType.Gold);
    }

    public int GetAmount(CurrencyType type)
    {
        return currencyDict.GetValueOrDefault(type, 0);
    }

    public void Add(CurrencyType type, int amount)
    {
        if (amount <= 0) return;

        int max = maxCapDict.GetValueOrDefault(type, int.MaxValue);
        currencyDict[type] = Mathf.Min(currencyDict[type] + amount, max);
        ItemEvents.RaiseCurrencyUpdated(type, currencyDict[type]);
    }

    public bool Spend(CurrencyType type, int amount)
    {
        if (amount <= 0) return false;
        if (currencyDict[type] < amount) return false;

        currencyDict[type] -= amount;
        ItemEvents.RaiseCurrencyUpdated(type, currencyDict[type]);
        return true;
    }

    public void Set(CurrencyType type, int amount)
    {
        int max = maxCapDict.GetValueOrDefault(type, int.MaxValue);
        currencyDict[type] = Mathf.Clamp(amount, 0, max);
        ItemEvents.RaiseCurrencyUpdated(type, currencyDict[type]);
    }

    public bool CanAfford(CurrencyType type, int amount)
    {
        return currencyDict[type] >= amount;
    }

    #endregion

    #region Persistance

    public SaveDataCurrencySystem Export()
    {
        SaveDataCurrencySystem saveData = new SaveDataCurrencySystem();

        saveData.CurrencyDict = currencyDict;

        return saveData;
    }

    public void Import(SaveDataCurrencySystem saveData)
    {
        currencyDict = saveData.CurrencyDict;
    }

    #endregion

    #region Events

    //public void Subscribe() { }
    //public void Unsubscribe() { }

    #endregion

}
