using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

[System.Serializable]
public class SaveDataLoadoutSystem
{
    public string ActiveLoadoutGuid;
    public BattleType DefaultBattleType;
    public List<TeamSaveData> TeamSaveDataList;
}
