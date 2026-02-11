using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentSpeed : MonoBehaviour
{
    private Character character;

    private float defaultSpeedMultiplier = 0.025f;
    private float minSpeed = 1f;
    private float maxSpeed = 100f;
    private float cachedMovementSpeed;

    public float MovementSpeed => cachedMovementSpeed;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    public void CalculateSpeed()
    {
        float baseSpeed = 
            character.GetBattleStat(Stat.Speed) * 
            defaultSpeedMultiplier;

        float modifiedSpeed =
            baseSpeed *
            character.FatigueSpeedMultiplier *
            character.StatusSpeedMultiplier;

        cachedMovementSpeed = Mathf.Clamp(modifiedSpeed, minSpeed, maxSpeed);
    }

}
