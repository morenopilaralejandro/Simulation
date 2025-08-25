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
    /*
    public void OfflineSpawn() {
        SpawnCharacters_Singleplayer();
    }

    private void SpawnCharacter_Singleplayer(int teamIndex, ControlType controlType, FormationCoord formationCoord)
    {
        Vector3 spawnPos = formationCoord.DefaultPosition;
        InstantiateCharacter_Singleplayer(
            characterKey,
            spawnPos,
            Quaternion.identity,
            teamIndex,
            controlType,
            formationCoord,
            (character) =>
            {
                if (character != null)
                    BattleBallManager.Instance.AddCharacterToTeam(character, teamIndex);
            });
    }

    private void InstantiateCharacter_Singleplayer(
        string characterKey,
        Vector3 position,
        Quaternion rotation,
        int teamIndex,
        ControlType controlType,
        FormationCoord formationCoord,
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
                    characterComponent.TeamIndex = teamIndex;
                    characterComponent.ControlType = controlType;
                    characterComponent.Coord = coord;
                    characterComponent.DefaultPosition = position;

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

    public void InitializeCharacter(Character character, CharacterData characterData, int teamIndex, Team team, bool isKeeper)
    {
                character.Initialize(characterData);
                character.IsKeeper = isKeeper;
                character.UpdateKeeperColliderState();
                character.Lv = team.Lv;
                character.TeamIndex = i;
                character.SetWear(team.WearId, WearManager.Instance.IsHome(teams, character.TeamIndex));
            
    }

    public void ResetCharacterPosition(Character character)
    {
        character.Unstun();
        character.transform.position = character.DefaultPosition;        
    }
*/
}
