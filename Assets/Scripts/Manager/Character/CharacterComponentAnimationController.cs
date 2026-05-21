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
        "hurt",
        "1h_halfslash"
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
    [SerializeField] private SpriteResolverFrameDriver hairFrontDriver;
    [SerializeField] private SpriteResolverFrameDriver hairBackDriver;
    [SerializeField] private SpriteResolverFrameDriver wingsFrontDriver;
    [SerializeField] private SpriteResolverFrameDriver wingsBackDriver;

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

        hairFrontDriver = transform.Find("HairFront")?.GetComponent<SpriteResolverFrameDriver>();
        hairBackDriver = transform.Find("HairBack")?.GetComponent<SpriteResolverFrameDriver>();
        wingsFrontDriver = transform.Find("WingsFront")?.GetComponent<SpriteResolverFrameDriver>();
        wingsBackDriver = transform.Find("WingsBack")?.GetComponent<SpriteResolverFrameDriver>();
    }

    public void OnLateUpdate() 
    {
        bodyDriver.OnLateUpdate();
        kitDriver.OnLateUpdate();
        hairFrontDriver.OnLateUpdate();
        hairBackDriver.OnLateUpdate();
        wingsFrontDriver.OnLateUpdate();
        wingsBackDriver.OnLateUpdate();
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

    public void RefreshAnimation() 
    {
        ConfigureResolvers(currentState, currentDirection);

        animator.SetInteger(StateHash, (int)currentState);
        animator.SetInteger(DirectionHash, (int)currentDirection);
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
        hairFrontDriver.Configure(animName, dirName);
        hairBackDriver.Configure(animName, dirName);
        wingsFrontDriver.Configure(animName, dirName);
        wingsBackDriver.Configure(animName, dirName);
    }

    #endregion
}
