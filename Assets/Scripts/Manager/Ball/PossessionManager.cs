using System;
using UnityEngine;

public class PossessionManager : MonoBehaviour
{
    public static PossessionManager Instance { get; private set; }

    [SerializeField] private Character currentCharacter;
    [SerializeField] private Character lastCharacter;
    private float cooldown = 0.2f;
    private float lastKickTime = -Mathf.Infinity;

    public Character CurrentCharacter => currentCharacter;
    public Character LastCharacter => lastCharacter;
    public float LastKickTime => LastKickTime;

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

    public bool IsOnCooldown(Character character) => character == lastCharacter && (Time.time - lastKickTime) <= cooldown;

    public void Gain(Character character)
    {
        if (character == null || character == currentCharacter || IsOnCooldown(character)) return;

        Release();
        currentCharacter = character;
        OffsideManager.Instance.OnBallTouched(character);
        LogManager.Info($"[PossessionManager] Possession gained by {character.CharacterId}", this);
        BallEvents.RaiseGained(character);
    }

    public void Release()
    {
        if (CurrentCharacter == null) return;

        lastCharacter = currentCharacter;
        lastKickTime = Time.time;
        LogManager.Info($"[PossessionManager] Possession released by {lastCharacter.CharacterId}", this);
        BallEvents.RaiseReleased(lastCharacter);
        currentCharacter = null;
    }

    public void SetLastCharacter(Character character) => lastCharacter = character;

    public void Reset()
    {
        currentCharacter = null;
        lastCharacter = null;
        lastKickTime = -Mathf.Infinity;
    }

    public void GiveBallToCharacter(Character character) 
    {
        BattleManager.Instance.Ball.transform.position = character.transform.position;
    }
}
