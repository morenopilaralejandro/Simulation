using UnityEngine;

public class CharacterComponentLevels
{
    private Character character;

    public const int MAX_LEVEL = 99;
    public const int MIN_LEVEL = 1;

    private int level = MIN_LEVEL;
    private int currentExp = 0;
    private int expToNextLevel = 100;

    public int Level => level;
    public int CurrentExp => currentExp;
    public int ExpToNextLevel => expToNextLevel;

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
            currentExp = characterSaveData.CurrentExp;
            expToNextLevel = characterSaveData.ExpToNextLevel;
        } else 
        {
            level = MIN_LEVEL;
            currentExp = 0;
            expToNextLevel = CalculateExpForNextLevel();
        }

    }

    public void AddExp(int amount)
    {
        if (level >= MAX_LEVEL)
            return;

        currentExp += amount;

        while (currentExp >= expToNextLevel && level < MAX_LEVEL)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        expToNextLevel = CalculateExpForNextLevel();
        character.UpdateStats();
        character.CheckLearnMoveOnLevelUp();
    }

    private int CalculateExpForNextLevel()
    {
        // Example scaling formula (tweak as needed)
        return 100 + (level * 25);
    }

    public void SetLevel(int targetLevel)
    {
        targetLevel = Mathf.Clamp(targetLevel, MIN_LEVEL, MAX_LEVEL);
        level = targetLevel;

        currentExp = 0;
        expToNextLevel = CalculateExpForNextLevel();

        character.UpdateStats();
        character.CheckLearnMoveOnLevelUp();
    }
}
