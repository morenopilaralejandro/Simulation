using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterSaveData
{
    //attributes
    public string CharacterId;
    public string CharacterGuid;

    //level
    public int Level;
    public int CurrentExp;
    public int ExpToNextLevel;

    //stats
    public int CurrentHp; //GetBattleStat
    public int CurrentSp; //GetBattleStat
    public List<CharacterStatSaveData> TrainedStats;

    //training
    public int CurrentFreedom;
    public int TrainingResetCount;

    //moves
    public List<MoveSaveData> learnedMoveIds;
    public List<string> equippedMoveIds;

    //TODO scouting with time stamp
}
