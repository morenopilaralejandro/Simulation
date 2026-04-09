using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class SerializableKeyValue<TKey, TValue>
{
    public TKey Key;
    public TValue Value;
}
