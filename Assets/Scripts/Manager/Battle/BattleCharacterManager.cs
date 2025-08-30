using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class BattleCharacterManager : MonoBehaviour
{

    public static BattleCharacterManager Instance { get; private set; }

    private Transform spawnPoint; 
    private string characterKey = "CharacterPrefab";
    private int charactersSpawned = 0;
    public event Action OnAllCharactersSpawned;

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

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void RegisterSpawnPoint(Transform spawner)
    {
        spawnPoint = spawner.transform;
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    public void SpawnCharacter_Singleplayer(int teamIndex)
    {
        Vector3 spawnPos = spawnPoint.position;
        InstantiateCharacter_Singleplayer(
            characterKey,
            spawnPos,
            Quaternion.identity,
            teamIndex,
            (character) =>
            {
                if (character != null) {
                    BattleManager.Instance.AddCharacterToTeam(character, teamIndex);
                    charactersSpawned++;
                    if (charactersSpawned >= TeamManager.Instance.SizeBattle*2)
                    {
                        charactersSpawned = 0;
                        LogManager.Trace("[BattleCharacterManager] All characters spawned.");
                        OnAllCharactersSpawned?.Invoke();
                    }
                }
            });
    }

    private void InstantiateCharacter_Singleplayer(
        string characterKey,
        Vector3 position,
        Quaternion rotation,
        int teamIndex,
        System.Action<Character> onCharacterSpawned)
    {
        Addressables.InstantiateAsync(characterKey, position, rotation, spawnPoint).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject go = handle.Result;

                Character characterComponent = go.GetComponent<Character>();
                if (characterComponent != null)
                {
                    LogManager.Trace($"[BattleCharacterManager] Successfully spawned character with key: {characterKey}");
                    onCharacterSpawned?.Invoke(characterComponent);
                }
                else
                {
                    LogManager.Error("[BattleCharacterManager] Spawned prefab is missing a Character component!");
                    onCharacterSpawned?.Invoke(null);
                }
            }
            else
            {
                LogManager.Error($"[BattleCharacterManager] Failed to spawn character with key: {characterKey}");
                onCharacterSpawned?.Invoke(null);
            }
        };
    }

    public void ResetCharacterPosition(Character character)
    {
        //character.Unstun();
        character.transform.position = character.GetFormationCoord().DefaultPosition;        
    }
}
