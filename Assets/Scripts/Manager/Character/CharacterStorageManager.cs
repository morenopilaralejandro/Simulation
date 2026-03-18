using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

/// <summary>
/// MonoBehaviour singleton that owns the CharacterStorage instance
/// and provides a global access point.
/// </summary>
public class CharacterStorageManager : MonoBehaviour
{
    public static CharacterStorageManager Instance { get; private set; }

    private CharacterStorage storage;

    public CharacterStorage Storage => storage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        storage = new CharacterStorage();
    }

    #region Convenience API

    /// <summary>
    /// Entry point for the scout system to add a new character.
    /// </summary>
    public Character AddCharacter(Character character) => storage.AddCharacter(character);
    public Character AddCharacterFromScout(CharacterData characterData, int level) => storage.AddCharacterFromScout(characterData, level);
    public Character GetCharacter(string characterGuid) => storage.GetCharacter(characterGuid);
    public bool HasCharacter(string characterGuid) => storage.HasCharacter(characterGuid);
    public bool RemoveCharacter(string characterGuid) => storage.RemoveCharacter(characterGuid);

    public void FirstTimeInitialize() => storage.FirstTimeInitialize();
    #endregion

    #region Persistence

    public CharacterStorageSaveData Export() => storage.Export();
    public void Import(CharacterStorageSaveData saveData) => storage.Import(saveData);

    #endregion

    #region Event

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
