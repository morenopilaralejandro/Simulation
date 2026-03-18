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
    public string CustomAvatarId;
    public string CustomName;
    public CharacterSize CustomCharacterSize;
    public Gender CustomGender;
    public Element CustomElement;
    public Position CustomPosition;
    public HairStyle CustomHairStyle;
    public HairColorType CustomHairColorType;
    public EyeColorType CustomEyeColorType;
    public BodyColorType CustomBodyColorType;
    public PortraitSize CustomPortraitSize;

    //TODO scouting with time stamp
}
