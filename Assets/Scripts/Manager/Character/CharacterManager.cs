using UnityEngine;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;

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
    public IReadOnlyDictionary<string, Character> Characters => storageSystem.Characters;
    public Character AddCharacter(Character character) => storageSystem.AddCharacter(character);
    public Character AddCharacterFromScout(string characterId, int level) => storageSystem.AddCharacterFromScout(characterId, level);
    public Character GetCharacter(string characterGuid) => storageSystem.GetCharacter(characterGuid);
    public List<Character> GetAllCharacters() => storageSystem.GetAllCharacters();
    public bool HasCharacter(string characterGuid) => storageSystem.HasCharacter(characterGuid);
    public bool HasCharacterById(string characterId) => storageSystem.HasCharacterById(characterId);
    public bool RemoveCharacter(string characterGuid) => storageSystem.RemoveCharacter(characterGuid);
    public void FirstTimeInitialize() => storageSystem.FirstTimeInitialize();
    public CharacterStorageSaveData ExportStorageSystem() => storageSystem.Export();
    public void ImportStorageSystem(CharacterStorageSaveData saveData) => storageSystem.Import(saveData);
    public void FullHealAll(IEnumerable<Character> characters) => storageSystem.FullHealAll(characters);
    public void FullHealAll() => storageSystem.FullHealAll();

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
        LogManager.Trace($"[CharacterStorage] HandleMoveUsed {character.Character.CharacterGuid}");
        if(!HasCharacter(character.Character.CharacterGuid)) return;

        LogManager.Trace($"[CharacterStorage] {character.Character.CharacterName} used {move.MoveName}");

        bool hasEvolved = move.ProgressEvolution();
        if (hasEvolved)
            MoveEvents.RaiseMoveEvolved(move, character);
    }

    #endregion
}
