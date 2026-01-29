using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;

public class BattleCharacterManager : MonoBehaviour
{
    public static BattleCharacterManager Instance { get; private set; }

    [Header("Character Settings")]
    [SerializeField] private GameObject characterPrefab;    //inspector
    [SerializeField] private int initialPoolSize = 22;

    private Queue<Character> characterPool = new Queue<Character>();
    private Transform spawnPoint;

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

    }

    public void RegisterSpawnPoint(Transform spawner)
    {
        spawnPoint = spawner;

        if (characterPool.Count == 0)
            PrewarmCharacterPool();
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    private void PrewarmCharacterPool()
    {
        if (characterPrefab == null)
        {
            LogManager.Error("[BattleCharacterManager] Character prefab missing!");
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            var go = Instantiate(characterPrefab, spawnPoint);
            var character = go.GetComponent<Character>();
            go.SetActive(false);
            characterPool.Enqueue(character);
        }

        LogManager.Trace($"[BattleCharacterManager] Prewarmed {initialPoolSize} pooled characters");
    }

    public void GetPooledCharacter(Action<Character> onCharacterReady)
    {
        if (characterPool.Count == 0)
        {
            var go = Instantiate(characterPrefab, spawnPoint);
            var character = go.GetComponent<Character>();
            characterPool.Enqueue(character);
        }

        var pooledCharacter = characterPool.Dequeue();
        pooledCharacter.gameObject.SetActive(true);
        onCharacterReady?.Invoke(pooledCharacter);
    }

    public void ReturnCharacterToPool(Character character)
    {
        if (character == null) return;
        character.gameObject.SetActive(false);
        characterPool.Enqueue(character);
    }

    public void AssignCharacterToTeamBattle(Character character, Team team, int characterIndex)
    {
        FormationCoord formationCoord = team.Formation.FormationCoords[characterIndex];
        TeamEvents.RaiseAssignCharacterToTeamBattle(character, team, formationCoord);
    }

    public void ResetCharacterPosition(Character character)
    {
        character.ResetPhysics();
        character.Teleport(character.FormationCoord.DefaultPosition);

        //rotation
        float yRotation = character.transform.position.z > 0f ? 180f : 0f;
        character.Model.transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

        character.ClearAllStatus();
        character.ReleaseStateLock();
    }

    public void ClearPool()
    {
        foreach (var character in characterPool)
            Destroy(character.gameObject);
        characterPool.Clear();
    }
}
