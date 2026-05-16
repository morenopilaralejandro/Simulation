using UnityEngine;
using Aremoreno.Enums.Character;

[RequireComponent(typeof(Animator))]
public class CharacterAnimationController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    [SerializeField]
    private Animator animator;

    private void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdle(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Idle, direction);
    }

    public void PlayWalk(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Walk, direction);
    }

    public void PlayRun(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Run, direction);
    }

    public void PlayJump(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Jump, direction);
    }

    public void PlayCombat(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Combat, direction);
    }

    public void PlayEmote(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Emote, direction);
    }

    public void PlaySlash(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Slash, direction);
    }

    public void PlayBackslash(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Backslash1H, direction);
    }

    public void PlaySpellcast(Vector2 direction)
    {
        PlayDirectional(CharacterAnimationState.Spellcast, direction);
    }

    public void PlayHurt()
    {
        animator.SetInteger(StateHash, (int)CharacterAnimationState.Hurt);
        animator.SetInteger(DirectionHash, (int)CharacterDirection.Down);
    }

    private void PlayDirectional(CharacterAnimationState state, Vector2 direction)
    {
        CharacterDirection resolvedDirection = ResolveDirection(direction);

        animator.SetInteger(StateHash, (int)state);
        animator.SetInteger(DirectionHash, (int)resolvedDirection);
    }

    private CharacterDirection ResolveDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0
                ? CharacterDirection.Right
                : CharacterDirection.Left;
        }

        return direction.y > 0
            ? CharacterDirection.Up
            : CharacterDirection.Down;
    }
}
