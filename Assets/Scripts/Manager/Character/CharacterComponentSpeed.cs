using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterComponentSpeed : MonoBehaviour
{
    private CharacterEntityBattle characterEntityBattle;

    private float defaultSpeedMultiplier = 0.02f;
    private float minSpeed = 2.0f;
    private float maxSpeed = 100f;
    private float cachedMovementSpeed;

    public float MovementSpeed => cachedMovementSpeed;

    public void Initialize(CharacterEntityBattle characterEntityBattle) 
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    public void CalculateSpeed()
    {
        /*
        2.0f average
        2.5f fast
        */

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
