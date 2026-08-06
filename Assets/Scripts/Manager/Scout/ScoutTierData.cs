using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Scout;

[CreateAssetMenu(fileName = "ScoutTierData", menuName = "ScriptableObject/Scout/ScoutTierData")]
public class ScoutTierData : ScriptableObject
{
    public string ScoutTierId;
    public string UnlockFlag;
    public int CharacterCost;
    public int CharacterLevel;
    public List<string> CharacterIds;
}
