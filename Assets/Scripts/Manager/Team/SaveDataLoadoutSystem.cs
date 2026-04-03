using System.Collections.Generic;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;

[System.Serializable]
public class SaveDataLoadoutSystem
{
    public string ActiveLoadoutGuid;
    public List<TeamSaveData> TeamSaveDataList;
}
