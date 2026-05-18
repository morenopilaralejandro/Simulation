using UnityEngine;
using Aremoreno.Enums.Character;

[RequireComponent(typeof(Animator))]
public class CharacterComponentAnimationController : MonoBehaviour
{
    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    [SerializeField] private Animator animator;
    [SerializeField] private SpriteResolverFrameDriver bodyDriver;
    [SerializeField] private SpriteResolverFrameDriver kitDriver;
    [SerializeField] private SpriteResolverFrameDriver wingsDriver;

    private void Reset()
    {
        animator = GetComponent<Animator>();

        bodyDriver = transform.Find("Body")?.GetComponent<SpriteResolverFrameDriver>();
        kitDriver = transform.Find("Kit")?.GetComponent<SpriteResolverFrameDriver>();
        wingsDriver = transform.Find("Wings")?.GetComponent<SpriteResolverFrameDriver>();
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
        string animName = CharacterAnimationState.Hurt.ToString().ToLowerInvariant();
        string dirName = "down";

        ConfigureResolvers(animName, dirName);

        animator.SetInteger(StateHash, (int)CharacterAnimationState.Hurt);
        animator.SetInteger(DirectionHash, (int)CharacterDirection.Down);
    }

    private void PlayDirectional(CharacterAnimationState state, Vector2 direction)
    {
        CharacterDirection resolvedDirection = ResolveDirection(direction);

        string animName = state.ToString().ToLowerInvariant();
        if (state == CharacterAnimationState.Backslash1H) animName = "1h_backslash";
        string dirName = DirectionToString(resolvedDirection);

        ConfigureResolvers(animName, dirName);

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

    private void ConfigureResolvers(string animName, string dir)
    {
        bodyDriver?.Configure(animName, dir);
        kitDriver?.Configure(animName, dir);
        wingsDriver?.Configure(animName, dir);
    }

    private string DirectionToString(CharacterDirection direction)
    {
        switch (direction)
        {
            case CharacterDirection.Down:
                return "down";
            case CharacterDirection.Up:
                return "up";
            case CharacterDirection.Left:
                return "left";
            case CharacterDirection.Right:
                return "right";
            default:
                return "down";
        }
    }
}
