using UnityEngine;
using Aremoreno.Enums.Character;

public class CharacterComponentFatigue : MonoBehaviour
{
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private FatigueState fatigueState;
    private int tiredThreshold = 20;
    private int exhaustedThreshold = 0;
    [SerializeField] private float fatigueSpeedMultiplier = 1.0f;
    private float normalSpeedMultiplier = 1.0f;
    private float tiredSpeedMultiplier = 0.6f;
    private float exhaustedSpeedMultiplier = 0.3f;

    public FatigueState FatigueState => fatigueState;
    public float FatigueSpeedMultiplier => fatigueSpeedMultiplier;

    public void Initialize(CharacterEntityBattle characterEntityBattle) 
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    public void UpdateFatigue()
    {
        int hp = characterEntityBattle.GetBattleStat(Stat.Hp);
        if (hp <= exhaustedThreshold)
        {
            fatigueState = FatigueState.Exhausted;
            fatigueSpeedMultiplier = exhaustedSpeedMultiplier;
        }
        else if (hp < tiredThreshold)
        {
            fatigueState = FatigueState.Tired;
            fatigueSpeedMultiplier = tiredSpeedMultiplier;
        }
        else
        {
            fatigueState = FatigueState.Normal;
            fatigueSpeedMultiplier = normalSpeedMultiplier;
        }
        if (!characterEntityBattle.HasStatusEffect()) 
            characterEntityBattle.UpdateStatusIndicator(null);

        characterEntityBattle.CalculateSpeed();
    }

}
