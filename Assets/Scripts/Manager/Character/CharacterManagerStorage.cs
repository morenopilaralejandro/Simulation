using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.Move;
using Aremoreno.Enums.Localization;

/// <summary>
/// Persistent storage for all characters the player owns.
/// Characters are keyed by their unique CharacterGuid.
/// </summary>
public class CharacterManagerStorage
{
    private Dictionary<string, Character> characters = new();

    public IReadOnlyDictionary<string, Character> Characters => characters;
    public int Count => characters.Count;

    public CharacterManagerStorage() { }

    #region First Time Initialize

    public void FirstTimeInitialize()
    {
        Clear();

        int startLv = 50; //default is 5

        AddCharacterFromScout("chara-00070-teruel", startLv);

        AddCharacterFromScout("chara-00019-wang", startLv);
        AddCharacterFromScout("chara-00075-alexander", startLv);
        AddCharacterFromScout("chara-00017-ainara", startLv);
        AddCharacterFromScout("chara-00074-paula", startLv);

        AddCharacterFromScout("chara-00067-satu", startLv);
        AddCharacterFromScout("chara-00047-carlos", startLv);
        AddCharacterFromScout("chara-00107-diego", startLv);
        AddCharacterFromScout("chara-00013-fran", startLv);

        AddCharacterFromScout("chara-00154-simon", startLv);
        AddCharacterFromScout("chara-00088-sofireca", startLv);

        AddCharacterFromScout("chara-00089-roble", startLv);
        AddCharacterFromScout("chara-00041-kike", startLv);
        AddCharacterFromScout("chara-00112-fernando", startLv);
        AddCharacterFromScout("chara-00039-arga", startLv);
        AddCharacterFromScout("chara-00143-esteban", startLv);

        TeamManager.Instance.InitializeFirstLoadout();
    }

    public void AddAllFromDatabase()
    {
        foreach (CharacterData characterData in DatabaseManager.Instance.DatabaseRegistry.CharacterData.Data.Values)
        {
            AddCharacterFromScout(characterData.CharacterId, 99);
        }
    }

    #endregion

    #region Add / Remove
    public Character AddCharacter(Character character)
    {
        if (characters.ContainsKey(character.CharacterGuid))
        {
            LogManager.Warning($"[CharacterStorage] Character with GUID {character.CharacterGuid} already exists. Skipping.");
            return characters[character.CharacterGuid];
        }

        characters[character.CharacterGuid] = character;
        CharacterEvents.RaiseCharacterAdded(character);

        LogManager.Info($"[CharacterStorage] Added character: {character.CharacterName} ({character.CharacterGuid})");
        return character;
    }

    /// <summary>
    /// Entry point for the scout system or any other acquisition method.
    /// Creates a Character from data and optional save data, stores it, and returns it.
    /// </summary>
    public Character AddCharacterFromScout(string characterId, int level)
    {
        Character character = new Character(DatabaseManager.Instance.GetCharacterData(characterId));
        character.SetLevel(level);
        return AddCharacter(character);
    }

    public bool RemoveCharacter(string characterGuid)
    {
        if (characters.TryGetValue(characterGuid, out Character character))
        {
            characters.Remove(characterGuid);
            CharacterEvents.RaiseCharacterRemoved(character);
            LogManager.Info($"[CharacterStorage] Removed character: {character.CharacterName} ({characterGuid})");
            return true;
        }

        LogManager.Warning($"[CharacterStorage] Character with GUID {characterGuid} not found for removal.");
        return false;
    }

    #endregion

    #region Query

    public Character GetCharacter(string characterGuid)
    {
        characters.TryGetValue(characterGuid, out Character character);
        return character;
    }

    public bool HasCharacter(string characterGuid)
    {
        return characters.ContainsKey(characterGuid);
    }

    public bool HasCharacterById(string characterId)
    {
        return characters.Values.Any(c => c.CharacterId == characterId);
    }

    public List<Character> GetAllCharacters()
    {
        return characters.Values.ToList();
    }

    public List<Character> GetCharactersByElement(Element element)
    {
        return characters.Values.Where(c => c.Element == element).ToList();
    }

    public List<Character> GetCharactersByPosition(Position position)
    {
        return characters.Values.Where(c => c.Position == position).ToList();
    }

    public List<Character> GetCharactersByGuids(List<string> characterGuids)
    {
        List<Character> result = new();
        foreach (string guid in characterGuids)
        {
            if (characters.TryGetValue(guid, out Character character))
                result.Add(character);
            else
                LogManager.Warning($"[CharacterStorage] Character GUID {guid} not found in storage.");
        }
        return result;
    }

    #endregion

    #region Full Heal

    public void FullHealAll(IEnumerable<Character> characters)
    {
        foreach (Character character in characters)
        {
            character.ClearStatusPermanent();
            character.RestoreHpSp();
        }
    }

    public void FullHealAll() 
    {
        foreach (Character character in characters.Values)
        {
            character.ClearStatusPermanent();
            character.RestoreHpSp();
        }
    }

    #endregion

    #region Persistence
    
    public CharacterStorageSaveData Export()
    {
        CharacterStorageSaveData saveData = new CharacterStorageSaveData();
        saveData.CharacterSaveDataList = new List<CharacterSaveData>();

        foreach (Character character in characters.Values)
            saveData.CharacterSaveDataList.Add(character.Export());

        return saveData;
    }

    public void Import(CharacterStorageSaveData saveData)
    {
        Clear();

        if (saveData?.CharacterSaveDataList == null) return;

        foreach (CharacterSaveData characterSaveData in saveData.CharacterSaveDataList)
        {
            AddCharacter(CharacterFactory.CreateFromSaveData(characterSaveData));
        }
    }

    public void Clear()
    {
        characters.Clear();
    }

    #endregion

}
