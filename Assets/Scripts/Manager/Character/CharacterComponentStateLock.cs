using UnityEngine;
using System.Collections;
using Aremoreno.Enums.Character;

public class CharacterComponentStateLock : MonoBehaviour
{
    private Character character;
    private CharacterEntityBattle characterEntityBattle;

    [SerializeField] private bool isStateLocked;
    private float controlMultiplier = 0.005f;
    private float minLockDuration = 0.1f;
    private float baseDurationKick = 0.4f;
    private float baseDurationControl = 0.4f;
    private float baseDurationDefault = 0.4f;
    private Coroutine stateLockCoroutine;

    public bool IsStateLocked => isStateLocked;

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
        this.character = characterEntityBattle.Character;
    }

    private float GetStateDuration(CharacterState state)
    {
        // Base durations (in seconds)
        float baseDuration = state switch
        {
            CharacterState.Kick => baseDurationKick,
            CharacterState.Control => baseDurationControl,
            _ => baseDurationDefault
        };

        // Control stat adjustment
        float control = characterEntityBattle.GetBattleStat(Stat.Control); // e.g., range: 0–100
        float adjustedDuration = baseDuration - (control * controlMultiplier);
        return Mathf.Max(adjustedDuration, minLockDuration);
    }

    public void StartStateLock(CharacterState state)
    {
        if (isStateLocked) return;

        characterEntityBattle.StopVelocity();

        isStateLocked = true;
        float duration = GetStateDuration(state);

        if (stateLockCoroutine != null)
            StopCoroutine(stateLockCoroutine);
        stateLockCoroutine = StartCoroutine(FailSafeUnlock(duration));
    }

    public void ReleaseStateLock()
    {
        isStateLocked = false;
        if (stateLockCoroutine == null) return;
        
        StopCoroutine(stateLockCoroutine);
        stateLockCoroutine = null;
        characterEntityBattle.SetCharacterState(CharacterState.Idle);        
    }

    private IEnumerator FailSafeUnlock(float duration)
    {
        yield return new WaitForSeconds(duration);
        ReleaseStateLock();
    }
}
