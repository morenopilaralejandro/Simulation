using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;

public class CharacterComponentStatusEffects : MonoBehaviour
{
    private Character character;
    private CharacterEntityBattle characterEntityBattle;

    private readonly HashSet<StatusEffect> activeStatusEffects = new HashSet<StatusEffect>();
    private float statusSpeedMultiplier = 1.0f;
    private float normalSpeedMultiplier = 1.0f;
    private float trippingSpeedMultiplier = 0.6f;
    private Coroutine stunCoroutine;
    private Coroutine blinkCoroutine;
    private Coroutine tripCoroutine;

    public HashSet<StatusEffect> ActiveStatusEffects => activeStatusEffects;
    public float StatusSpeedMultiplier => statusSpeedMultiplier;

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
        this.character = characterEntityBattle.Character;
    }

    public void ApplyStatus(StatusEffect effect)
    {
        if (activeStatusEffects.Contains(effect)) 
            ClearStatus(effect);

        activeStatusEffects.Add(effect);

        switch (effect)
        {
            case StatusEffect.Stunned:
                if (characterEntityBattle.HasBall())
                    PossessionManager.Instance.Release();
                stunCoroutine = StartCoroutine(HandleStun(3f));
                break;
            case StatusEffect.Tripping:
                tripCoroutine = StartCoroutine(HandleTrip(1f));
                break;
        }
        characterEntityBattle.UpdateStatusIndicator(effect);
    }

    public void ClearStatus(StatusEffect effect)
    {
        if (!activeStatusEffects.Contains(effect))
            return;

        switch (effect)
        {
            case StatusEffect.Stunned:
                ClearStun();
                break;

            case StatusEffect.Tripping:
                ClearTrip();
                break;
        }

        activeStatusEffects.Remove(effect);
        characterEntityBattle.UpdateStatusIndicator(null);
        characterEntityBattle.UpdateFatigue();
    }

    public void ClearAllStatus()
    {
        foreach (var effect in new List<StatusEffect>(activeStatusEffects))
        {
            ClearStatus(effect);
        }
        activeStatusEffects.Clear();
    }

    private IEnumerator HandleStun(float duration)
    {
        blinkCoroutine = StartCoroutine(BlinkEffect(duration));

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!BattleManager.Instance.IsTimeFrozen)
                elapsed += Time.deltaTime;
            yield return null;
        }

        ClearStatus(StatusEffect.Stunned);
    }

    private void ClearStun()
    {
        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        stunCoroutine = null;
        blinkCoroutine = null;
        characterEntityBattle.SetCharacterVisible(true);
    }

    private IEnumerator BlinkEffect(float duration)
    {
        float elapsed = 0f;
        float blinkInterval = 0.5f;
        bool visible = true;
        float blinkElapsed = 0f;

        while (elapsed < duration)
        {
            if (!BattleManager.Instance.IsTimeFrozen)
            {
                elapsed += Time.deltaTime;
                blinkElapsed += Time.deltaTime;

                if (blinkElapsed >= blinkInterval)
                {
                    characterEntityBattle.SetCharacterVisible(visible);
                    visible = !visible;
                    blinkElapsed = 0f;
                }
            }
            else
            {
                characterEntityBattle.SetCharacterVisible(true);
            }
            yield return null;
        }

        characterEntityBattle.SetCharacterVisible(true);
    }

    private IEnumerator HandleTrip(float duration)
    {
        statusSpeedMultiplier = trippingSpeedMultiplier;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            if (!BattleManager.Instance.IsTimeFrozen)
                elapsed += Time.deltaTime;
            yield return null;
        }

        ClearStatus(StatusEffect.Tripping);
    }

    private void ClearTrip()
    {
        if (tripCoroutine != null)
            StopCoroutine(tripCoroutine);

        statusSpeedMultiplier = normalSpeedMultiplier;
    }
}
