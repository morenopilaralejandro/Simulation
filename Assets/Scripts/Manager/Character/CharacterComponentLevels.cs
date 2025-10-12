using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterComponentLevels : MonoBehaviour
{
    private Character character;
    public const int MAX_LEVEL = 99;

    [SerializeField] private int level;

    public int Level => level;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
        this.level = 99;
    }

    public void LevelUp()
    {
        if(this.level < MAX_LEVEL)
        {
            this.level++;
            this.character.UpdateStats();
        }
    }

}
