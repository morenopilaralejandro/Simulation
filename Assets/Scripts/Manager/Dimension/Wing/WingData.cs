using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

[CreateAssetMenu(fileName = "WingData", menuName = "ScriptableObject/Dimension/Wing/WingData")]
public class WingData : ScriptableObject
{
    public string WingId;
    public Element Element;
    public WingGrowthType WingGrowthType;
    public WingGrowthRate WingGrowthRate;

    /*
    public int Cost;
    public int BasePower;
    public int StunDamage;
    public int AuraDamage;
    public int Difficulty;
    public int FaultRate;
    */
}
