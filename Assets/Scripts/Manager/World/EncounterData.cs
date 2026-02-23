// EncounterData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EncounterData", menuName = "RPG/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public string encounterName;
    public Team[] possibleGroups;
    public string bgmId;

    [Range(0f, 1f)]
    public float encounterRate = 0.5f;
}
