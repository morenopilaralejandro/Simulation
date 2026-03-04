using UnityEngine;

[System.Serializable]
public class EncounterData
{
    public string teamId;
    public int level;
    public string bgmId;

    [Range(0f, 1f)]
    public float encounterRate = 0.5f;
}
