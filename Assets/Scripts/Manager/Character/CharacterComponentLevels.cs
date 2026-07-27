using UnityEngine;

public class CharacterComponentLevels
{
    private Character character;

    public const int MAX_LEVEL = 99;
    public const int MIN_LEVEL = 1;

    private int level = MIN_LEVEL;
    private int currentXp = 0;
    private int xpToNextLevel = 100;

    public int Level => level;
    public int CurrentXp => currentXp;
    public int XpToNextLevel => xpToNextLevel;

    public CharacterComponentLevels(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null) 
    {
        Initialize(characterData, character, characterSaveData);
    }

    public void Initialize(CharacterData characterData, Character character, CharacterSaveData characterSaveData = null)
    {
        this.character = character;

        if (characterSaveData != null)
        {
            level = characterSaveData.Level;
            currentXp = characterSaveData.CurrentXp;
            xpToNextLevel = characterSaveData.XpToNextLevel;
        } else 
        {
            level = MIN_LEVEL;
            currentXp = 0;
            xpToNextLevel = CalculateXpForNextLevel();
        }

    }

    public void AddXp(int amount)
    {
        if (level >= MAX_LEVEL)
            return;

        currentXp += amount;

        while (currentXp >= xpToNextLevel && level < MAX_LEVEL)
        {
            currentXp -= xpToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel = CalculateXpForNextLevel();
        character.UpdateStats();
        character.CheckLearnMoveOnLevelUp();
    }

    private int CalculateXpForNextLevel()
    {
        // Example scaling formula (tweak as needed)
        return 100 + (level * 25);
    }

    public void SetLevel(int targetLevel)
    {
        targetLevel = Mathf.Clamp(targetLevel, MIN_LEVEL, MAX_LEVEL);
        level = targetLevel;

        currentXp = 0;
        xpToNextLevel = CalculateXpForNextLevel();

        character.UpdateStats();
        character.CheckLearnMoveOnLevelUp();
    }
}
