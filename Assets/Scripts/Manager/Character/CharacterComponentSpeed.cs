using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentSpeed : MonoBehaviour
{
    private Character character;

    private float defaultSpeedMultiplier = 0.025f;
    private float minSpeed = 0.2f;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    public float GetMovementSpeed()
    {
        return (character.GetBattleStat(Stat.Speed) * defaultSpeedMultiplier + minSpeed) * character.FatigueSpeedMultiplier * character.StatusSpeedMultiplier;
    }

}
