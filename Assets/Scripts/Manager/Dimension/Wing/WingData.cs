using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

[CreateAssetMenu(fileName = "WingData", menuName = "ScriptableObject/Dimension/Wing/WingData")]
public class WingData : ScriptableObject
{
    public string WingId;
    public WingType WingType;
    public WingColorType WingColorType;
    public Element[] Elements;
    public WingGrowthType WingGrowthType;
    public WingGrowthRate WingGrowthRate;

    public int KickBase;
    public int BodyBase;
    public int ControlBase;
    public int GuardBase;
    public int SpeedBase;
    public int StaminaBase;
    public int TechniqueBase;
    public int LuckBase;
    public int CourageBase;

    public int KickIndividual;
    public int BodyIndividual;
    public int ControlIndividual;
    public int GuardIndividual;
    public int SpeedIndividual;
    public int StaminaIndividual;
    public int TechniqueIndividual;
    public int LuckIndividual;
    public int CourageIndividual;
}
