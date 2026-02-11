using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

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
    public List<MoveSaveData> LearnedMoves;
    public List<string> EquippedMovesIds;

    //avatar
    public bool IsCustomAvatar;
    public string CustomName;
    public CharacterSize CustomCharacterSize;
    public PortraitSize CustomPortraitSize;
    public Gender CustomGender;
    public Element CustomElement;
    public Position CustomPosition;
    public List<CharacterStatSaveData> CustomBaseStats;

    //TODO scouting with time stamp
}
