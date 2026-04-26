using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterComponentSpeed : MonoBehaviour
{
    private CharacterEntityBattle characterEntityBattle;

    private float defaultSpeedMultiplier = 0.025f;
    private float minSpeed = 1f;
    private float maxSpeed = 100f;
    private float cachedMovementSpeed;

    public float MovementSpeed => cachedMovementSpeed;

    public void Initialize(CharacterEntityBattle characterEntityBattle) 
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    public void CalculateSpeed()
    {
        float baseSpeed = 
            characterEntityBattle.GetBattleStat(Stat.Speed) * 
            defaultSpeedMultiplier;

        float modifiedSpeed =
            baseSpeed *
            characterEntityBattle.FatigueSpeedMultiplier *
            characterEntityBattle.StatusSpeedMultiplier;

        cachedMovementSpeed = Mathf.Clamp(modifiedSpeed, minSpeed, maxSpeed);
    }

}
