using UnityEngine;
using Aremoreno.Enums.Animation;

[RequireComponent(typeof(Animator))]
public class CharacterComponentAnimationController : MonoBehaviour
{
    #region Animator Hashes

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    #endregion

    #region Cached Names

    private static readonly string[] StateNames =
    {
        "idle",
        "walk",
        "run",
        "jump",
        "combat",
        "emote",
        "slash",
        "1h_backslash",
        "spellcast",
        "hurt"
    };

    private static readonly string[] DirectionNames =
    {
        "down",
        "up",
        "left",
        "right"
    };

    #endregion

    #region Serialized

    [SerializeField] private Animator animator;

    [SerializeField] private SpriteResolverFrameDriver bodyDriver;
    [SerializeField] private SpriteResolverFrameDriver kitDriver;
    [SerializeField] private SpriteResolverFrameDriver wingsDriver;

    #endregion

    #region Runtime

    private CharacterAnimationState currentState = CharacterAnimationState.Idle;
    private CharacterDirection currentDirection = CharacterDirection.Down;

    #endregion

    #region Initialization

    private void Reset()
    {
        animator = GetComponent<Animator>();
        bodyDriver = transform.Find("Body")?.GetComponent<SpriteResolverFrameDriver>();
        kitDriver = transform.Find("Kit")?.GetComponent<SpriteResolverFrameDriver>();
        wingsDriver = transform.Find("Wings")?.GetComponent<SpriteResolverFrameDriver>();
    }

    #endregion

    #region Public

    public void Play(
        CharacterAnimationState state,
        CharacterDirection direction)
    {
        if (state == currentState && direction == currentDirection) return;

        currentState = state;
        currentDirection = direction;

        ConfigureResolvers(state, direction);

        animator.SetInteger(StateHash, (int)state);
        animator.SetInteger(DirectionHash, (int)direction);
    }

    #endregion

    #region Resolver

    private void ConfigureResolvers(
        CharacterAnimationState state,
        CharacterDirection direction)
    {
        string animName = StateNames[(int)state];
        string dirName = DirectionNames[(int)direction];

        bodyDriver.Configure(animName, dirName);
        kitDriver.Configure(animName, dirName);
        wingsDriver.Configure(animName, dirName);
    }

    #endregion
}
