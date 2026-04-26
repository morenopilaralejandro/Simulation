using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;

public class SubstitutionData
{
    #region Fields

    public Character CharacterIn { get; private set; }
    public Character CharacterOut { get; private set; }
    public TeamSide TeamSide { get; private set; }

    #endregion

    #region Lifecycle

    public SubstitutionData(Character characterIn, Character characterOut, TeamSide teamSide)
    {
        CharacterIn = characterIn;
        CharacterOut = characterOut;
        TeamSide = teamSide;
    }

    #endregion

    #region Logic

    #endregion

}
