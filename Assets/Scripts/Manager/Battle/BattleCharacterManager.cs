using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Simulation.Enums.Character;

public class BattleCharacterManager : MonoBehaviour
{
    public static BattleCharacterManager Instance { get; private set; }

    private Queue<Character> characterPool = new Queue<Character>();

    private Transform spawnPoint; 
    private string characterKey = "CharacterPrefab";

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

    public void GetPooledCharacter(System.Action<Character> onCharacterReady)
    {
        if (characterPool.Count > 0)
        {
            Character pooledCharacter = characterPool.Dequeue();
            pooledCharacter.gameObject.SetActive(true);
            onCharacterReady?.Invoke(pooledCharacter);
        }
        else
        {
            //prefab key, position, rotation, root
            Addressables.InstantiateAsync(characterKey, spawnPoint.position, Quaternion.Euler(70f, 0f, 0f), spawnPoint).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject go = handle.Result;
                    Character character = go.GetComponent<Character>();

                    if (character != null)
                    {
                        LogManager.Trace($"[BattleCharacterManager] Spawned new character via Addressables with key {characterKey}");
                        onCharacterReady?.Invoke(character);
                    }
                    else
                    {
                        Debug.LogError("[BattleCharacterManager] Prefab is missing Character component!");
                        onCharacterReady?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError($"[BattleCharacterManager] Failed to load character with key {characterKey}");
                    onCharacterReady?.Invoke(null);
                }
            };
        }
    }

    public void AssignCharacterToTeamBattle(Character character, Team team, int characterIndex, int teamIndex) {
        FormationCoord formationCoord = team.Formation.FormationCoords[characterIndex];
        ControlType controlType = (teamIndex == 0) ? ControlType.LocalHuman : ControlType.AI;
        TeamEvents.RaiseAssignCharacterToTeamBattle(character, team, teamIndex, formationCoord, controlType);
    }

    public void ResetCharacterPosition(Character character)
    {
        //character.Unstun();
        character.transform.position = character.GetFormationCoord().DefaultPosition;        
    }

    public void ReturnCharacterToPool(Character character)
    {
        if (character == null) return;

        character.gameObject.SetActive(false);
        characterPool.Enqueue(character);
    }

    public void ClearPool()
    {
        foreach (var character in characterPool)
        {
            Destroy(character.gameObject);
        }
        characterPool.Clear();
    }
}
