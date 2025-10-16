using System;
using UnityEngine;

public class PossessionManager : MonoBehaviour
{
    public static PossessionManager Instance { get; private set; }

    public event Action<Character> OnGained;
    public event Action<Character> OnReleased;

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

    public void Subscribe(Action<Character> onGained, Action<Character> onReleased)
    {
        OnGained += onGained;
        OnReleased += onReleased;
    }

    public void Unsubscribe(Action<Character> onGained, Action<Character> onReleased)
    {
        OnGained -= onGained;
        OnReleased -= onReleased;
    }

    public bool IsOnCooldown(Character character) => character == lastCharacter && (Time.time - lastKickTime) <= cooldown;

    private void Gain(Character character)
    {
        if (character == null || character == currentCharacter || IsOnCooldown(character)) return;

        Release();
        currentCharacter = character;
        LogManager.Trace($"[PossessionManager] Possession gained by {character.CharacterId}", this);
        OnGained?.Invoke(character);
    }

    private void Release()
    {
        if (CurrentCharacter == null) return;

        lastCharacter = currentCharacter;
        lastKickTime = Time.time;
        LogManager.Trace($"[PossessionManager] Possession released by {lastCharacter.CharacterId}", this);
        OnReleased?.Invoke(currentCharacter);
        currentCharacter = null;
    }

    public void SetLastCharacter(Character character) => lastCharacter = character;

    public void Reset()
    {
        currentCharacter = null;
        lastCharacter = null;
        lastKickTime = -Mathf.Infinity;
    }

}
