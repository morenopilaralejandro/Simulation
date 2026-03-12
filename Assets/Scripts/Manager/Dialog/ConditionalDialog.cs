using UnityEngine;
using System;

[Serializable]
public class ConditionalDialog
{
    [Tooltip("Game flag to check (from IGameDataProvider)")]
    public string flagName;
    public bool flagValue = true;

    [Tooltip("Knot to use if flag matches")]
    public string knotName;
}
