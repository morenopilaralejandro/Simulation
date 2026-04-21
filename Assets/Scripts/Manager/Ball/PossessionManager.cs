using System;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Duel;
using Aremoreno.Enums.Character;

public class PossessionManager : MonoBehaviour
{
    #region Fields
    public static PossessionManager Instance { get; private set; }

    [SerializeField] private CharacterEntityBattle currentCharacter;
    [SerializeField] private CharacterEntityBattle lastCharacter;
    private float cooldown = 0.2f;
    private float cooldownSfxOnDuel = 0.5f;
    private float controlFeedbackKickTime = 0.2f;
    private float controlGoalDistance = 3.5f;
    private float lastKickTime = -999f;
    private float lastDuelTime = -999f;
    private OffsideManager offsideManager;
    private BattleManager battleManager;
    private AudioManager audioManager;

    public CharacterEntityBattle CurrentCharacter => currentCharacter;
    public CharacterEntityBattle LastCharacter => lastCharacter;
    public float LastKickTime => lastKickTime;

    #endregion

    #region LifeCycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() 
    {
        offsideManager = OffsideManager.Instance;
        battleManager = BattleManager.Instance;
        audioManager = AudioManager.Instance;
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        DuelEvents.OnDuelEnd += HandleDuelEnd;
    }

    private void OnDisable()
    {
        DuelEvents.OnDuelEnd -= HandleDuelEnd;
    }

    private void HandleDuelEnd(
        DuelMode duelMode,
        DuelParticipant winner,
        DuelParticipant loser,
        bool isWinnerUser)
    {
        lastDuelTime = Time.time;
    }

    #endregion

    #region Possession Logic
    public bool IsOnCooldown(CharacterEntityBattle character, float now) => character == lastCharacter && (now - lastKickTime) <= cooldown;

    public void Gain(CharacterEntityBattle character)
    {
        float now = Time.time;
        if (character == null || character == currentCharacter || IsOnCooldown(character, now)) return;

        Release();
        currentCharacter = character;
        offsideManager.OnBallTouched(character);
        LogManager.Info($"[PossessionManager] Possession gained by {character.CharacterId}", this);
        BallEvents.RaiseGained(character);

        if(character.IsKeeper ||
            battleManager.CurrentPhase == BattlePhase.DeadBall || 
            battleManager.IsTimeFrozen ||
            IsOnCooldownDuel(now)) return;
        if (IsPassCut())
            audioManager.PlaySfx("sfx-ball_pass_cut");
        else if (ShowControlFeedback(character, now))
            audioManager.PlaySfx("sfx-ball_control");
    }

    public void Release()
    {
        if (CurrentCharacter == null) return;
        float now = Time.time;
        lastCharacter = currentCharacter;
        lastKickTime = now;
        LogManager.Info($"[PossessionManager] Possession released by {lastCharacter.CharacterId}", this);
        BallEvents.RaiseReleased(lastCharacter);
        currentCharacter = null;
    }

    public void SetLastCharacter(CharacterEntityBattle characterEntityBattle) 
    {
        lastCharacter = characterEntityBattle;
        offsideManager.OnBallTouched(characterEntityBattle);
    }

    public void Reset()
    {
        currentCharacter = null;
        lastCharacter = null;
        lastKickTime = -999f;
        lastDuelTime = -999f;
    }

    public void GiveBallToCharacter(CharacterEntityBattle character) 
    {
        battleManager.Ball.transform.position = character.transform.position;
    }

    #endregion

    #region Feedback Logic

    public bool IsOnCooldownDuel(float now) => (now - lastDuelTime) <= cooldownSfxOnDuel;

    public void SetCooldown(CharacterEntityBattle character)
    {
        lastCharacter = character;
        lastKickTime = Time.time;
    }

    private bool IsPassCut() => 
        lastCharacter != null &&
        currentCharacter != null &&
        !currentCharacter.IsSameTeam(lastCharacter);
    private bool IsInControlFeedbackDistance(CharacterEntityBattle character) 
    {
        float z = character.transform.position.z;
        return character.TeamSide == TeamSide.Home
            ? z > controlGoalDistance
            : z < controlGoalDistance;
    }
    private bool ShowControlFeedback(CharacterEntityBattle character, float now) => 
        (now - lastKickTime) > controlFeedbackKickTime && 
        IsInControlFeedbackDistance(character) && 
        !character.IsEnemyAI;

    #endregion

}
