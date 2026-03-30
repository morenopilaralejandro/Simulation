using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

/// <summary>
/// MonoBehaviour singleton that owns the CharacterStorage instance
/// and provides a global access point.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    #region Fields

    private CharacterManagerStorage storageSystem;
    private CharacterManagerPersistance persistanceSystem;

    #endregion

    #region Lifecycle

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

    private void OnDestroy() 
    {
        //encounterSystem.Unsubscribe();
    }


    private void Start()
    {
        storageSystem = new CharacterManagerStorage();
        persistanceSystem = new CharacterManagerPersistance();
        
        //encounterSystem.Subscribe();
        //InitializeAsync();
    }

    //private async void InitializeAsync() { }

    #endregion

    #region API

    // storageSystem
    public Character AddCharacter(Character character) => storageSystem.AddCharacter(character);
    public Character AddCharacterFromScout(CharacterData characterData, int level) => storageSystem.AddCharacterFromScout(characterData, level);
    public Character GetCharacter(string characterGuid) => storageSystem.GetCharacter(characterGuid);
    public List<Character> GetAllCharacters() => storageSystem.GetAllCharacters();
    public bool HasCharacter(string characterGuid) => storageSystem.HasCharacter(characterGuid);
    public bool RemoveCharacter(string characterGuid) => storageSystem.RemoveCharacter(characterGuid);
    public void FirstTimeInitialize() => storageSystem.FirstTimeInitialize();
    public CharacterStorageSaveData ExportStorageSystem() => storageSystem.Export();
    public void ImportStorageSystem(CharacterStorageSaveData saveData) => storageSystem.Import(saveData);

    // persistanceSystem
    public CharacterSystemSaveData Export() => persistanceSystem.Export();
    public void Import(CharacterSystemSaveData saveData) => persistanceSystem.Import(saveData);

    #endregion

    #region Event

    //TODO placeholder

    private void OnEnable() 
    {
        MoveEvents.OnMoveUsed += HandleMoveUsed;
    }

    private void OnDisable() 
    {
        MoveEvents.OnMoveUsed -= HandleMoveUsed;
    }

    private void HandleMoveUsed(Move move, CharacterEntityBattle character)
    {
        if(!HasCharacter(character.Character.CharacterGuid)) return;

        move.ProgressEvolution();
        LogManager.Trace($"[CharacterStorage] {character.Character.CharacterName} used {move.MoveName}");
    }

    #endregion
}
