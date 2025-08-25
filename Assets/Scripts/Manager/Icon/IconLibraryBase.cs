using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class IconLibraryBase<TEnum> : ScriptableObject where TEnum : Enum
{
    [Serializable]
    public struct IconEntry
    {
        public TEnum key;
        public Sprite icon;
    }

    [SerializeField] private IconEntry[] entries;

    private Dictionary<TEnum, Sprite> _iconLookup;

    private void OnEnable()
    {
        _iconLookup = new Dictionary<TEnum, Sprite>();
        foreach (var entry in entries)
        {
            if (!_iconLookup.ContainsKey(entry.key))
                _iconLookup.Add(entry.key, entry.icon);
        }
    }

    public Sprite GetIcon(TEnum key)
    {
        if (_iconLookup != null && _iconLookup.TryGetValue(key, out var icon))
            return icon;

        return null; // or fallback
    }
}
