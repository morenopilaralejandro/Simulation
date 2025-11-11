using UnityEngine;
using Simulation.Enums.Character;

public class CharacterComponentFatigue : MonoBehaviour
{
    private Character character;

    [SerializeField] private FatigueState fatigueState;
    private int tiredThreshold = 20;
    private int exhaustedThreshold = 0;
    [SerializeField] private float fatigueSpeedMultiplier = 1.0f;
    private float normalSpeedMultiplier = 1.0f;
    private float tiredSpeedMultiplier = 0.6f;
    private float exhaustedSpeedMultiplier = 0.3f;

    public FatigueState FatigueState => fatigueState;
    public float FatigueSpeedMultiplier => fatigueSpeedMultiplier;

    public void Initialize(CharacterData characterData, Character character) 
    {
        this.character = character;
    }

    public void UpdateFatigue()
    {
        int hp = this.character.GetBattleStat(Stat.Hp);
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
        if (!this.character.HasStatusEffect()) 
            this.character.UpdateStatusIndicator(null);
    }

}
