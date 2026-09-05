using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterComponentAwaken
{
    #region Fields

    private Character character;

    public bool HasAwaken { get; private set; }

    #endregion

    #region LifeCycle

    public CharacterComponentAwaken(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;
        HasAwaken = false;

        if (characterSaveData != null) 
        {
            HasAwaken = characterSaveData.HasAwaken;
        }
    }

    #endregion

    #region Logic

    public bool CanAwaken => character.Level >= 50;
    public void Awaken() 
    {
        HasAwaken = true;
    }

    #endregion
}
