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
        var characterDatabase = DatabaseManager.Instance;

        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00027-almu"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00088-sofireca"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00067-satu"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00001-are"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00013-fran"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00103-rocinante"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00017-ainara"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00070-teruel"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00075-alexander"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00019-wang"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00095-navarro"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00002-mohamed"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00119-meiga"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00117-ruperta"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00040-malaki"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00023-ayud"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00140-isa"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00048-nina"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00042-gambino"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00124-inquina"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00030-apa"), 50);
        AddCharacterFromScout(characterDatabase.GetCharacterData("chara-00010-ali"), 50);

        TeamManager.Instance.InitializeFirstLoadout();
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
    public Character AddCharacterFromScout(CharacterData characterData, int level)
    {
        Character character = new Character(characterData);
        character.SetLevel(99);
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
        characters.Clear();

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
