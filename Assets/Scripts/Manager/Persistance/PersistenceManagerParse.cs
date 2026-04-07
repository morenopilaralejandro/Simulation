using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PersistenceManagerParse
{
    #region Fields

    private PersistenceManager persistenceManager;

    #endregion

    #region Constructor

    public PersistenceManagerParse() 
    {
        persistenceManager = PersistenceManager.Instance;
    }

    #endregion

    #region Logic

    public List<SerializableKeyValue<TKey, TValue>> ParseDict<TKey, TValue>(
        Dictionary<TKey, TValue> dict)
    {
        return ParseKeyValuePairs(dict, dict.Count);
    }

    public List<SerializableKeyValue<TKey, TValue>> ParseIReadOnlyDict<TKey, TValue>(
        IReadOnlyDictionary<TKey, TValue> dict)
    {
        return ParseKeyValuePairs(dict, dict.Count);
    }

    public List<T> ParseHashSet<T>(HashSet<T> hashSet)
    {
        var list = new List<T>(hashSet.Count); // pre-allocate

        foreach (var item in hashSet)
        {
            list.Add(item);
        }

        return list;
    }

    #endregion

    #region Helpers

    private List<SerializableKeyValue<TKey, TValue>> ParseKeyValuePairs<TKey, TValue>(
        IEnumerable<KeyValuePair<TKey, TValue>> source,
        int count)
    {
        var list = new List<SerializableKeyValue<TKey, TValue>>(count);

        foreach (var kvp in source)
        {
            list.Add(new SerializableKeyValue<TKey, TValue>
            {
                Key   = kvp.Key,
                Value = kvp.Value
            });
        }

        return list;
    }


    #endregion

    #region Events
    /*    
    public void Subscribe() { }
    public void Unsubscribe() { }
    */
    #endregion

}
