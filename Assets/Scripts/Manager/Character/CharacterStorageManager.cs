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
    public Character AddCharacter(CharacterData characterData, int level) => storage.AddCharacter(characterData, level);
    public Character GetCharacter(string characterGuid) => storage.GetCharacter(characterGuid);
    public bool HasCharacter(string characterGuid) => storage.HasCharacter(characterGuid);
    public bool RemoveCharacter(string characterGuid) => storage.RemoveCharacter(characterGuid);

    public void FirstTimeInitialize() => storage.FirstTimeInitialize();
    #endregion

    #region Persistence

    /*

    public void Save()
    {
        CharacterStorageSaveData saveData = storage.Export();
        // Integrate with your existing save system
        // e.g., SaveManager.Instance.SetCharacterStorageData(saveData);
    }

    public void Load(CharacterStorageSaveData saveData)
    {
        storage.Import(saveData);
    }

    */

    #endregion
}
