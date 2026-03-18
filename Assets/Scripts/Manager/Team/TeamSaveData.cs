using System.Collections.Generic;
using Simulation.Enums.Battle;

[System.Serializable]
public class TeamSaveData
{
    public string TeamGuid;
    public bool IsCustomLoadout;
    public string CustomName;
    public string CustomCrestId;
    public string CustomKitId;
    public string CustomFullBattleFormationId;
    public List<string> CustomFullBattleCharacterGuids;
    public string CustomMiniBattleFormationId;
    public List<string> CustomMiniBattleCharacterGuids;
}
