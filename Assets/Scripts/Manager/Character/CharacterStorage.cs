using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;
using Simulation.Enums.Move;
using Simulation.Enums.Localization;

/// <summary>
/// Persistent storage for all characters the player owns.
/// Characters are keyed by their unique CharacterGuid.
/// </summary>
public class CharacterStorage
{
    private Dictionary<string, Character> characters = new();

    public IReadOnlyDictionary<string, Character> Characters => characters;
    public int Count => characters.Count;

    public CharacterStorage() 
    {

    }

    #region First Time Initialize

    public void FirstTimeInitialize()
    {
        CharacterManager characterManager = CharacterManager.Instance;

        AddCharacter(characterManager.GetCharacterData("almu"), 50);
        AddCharacter(characterManager.GetCharacterData("sofireca"), 50);
        AddCharacter(characterManager.GetCharacterData("satu"), 50);
        AddCharacter(characterManager.GetCharacterData("are"), 50);
        AddCharacter(characterManager.GetCharacterData("fran"), 50);
        AddCharacter(characterManager.GetCharacterData("rocinante"), 50);
        AddCharacter(characterManager.GetCharacterData("ainara"), 50);
        AddCharacter(characterManager.GetCharacterData("teruel"), 50);
        AddCharacter(characterManager.GetCharacterData("alexander"), 50);
        AddCharacter(characterManager.GetCharacterData("wang"), 50);
        AddCharacter(characterManager.GetCharacterData("navarro"), 50);
        AddCharacter(characterManager.GetCharacterData("mohamed"), 50);
        AddCharacter(characterManager.GetCharacterData("meiga"), 50);
        AddCharacter(characterManager.GetCharacterData("ruperta"), 50);
        AddCharacter(characterManager.GetCharacterData("malaki"), 50);
        AddCharacter(characterManager.GetCharacterData("ayud"), 50);
        AddCharacter(characterManager.GetCharacterData("isa"), 50);
        AddCharacter(characterManager.GetCharacterData("nina"), 50);
        AddCharacter(characterManager.GetCharacterData("gambino"), 50);
        AddCharacter(characterManager.GetCharacterData("inquina"), 50);
        AddCharacter(characterManager.GetCharacterData("apa"), 50);
        AddCharacter(characterManager.GetCharacterData("ali"), 50);

        TeamLoadoutManager.Instance.InitializeFirstLoadout();
    }

    #endregion

    #region Add / Remove

    /// <summary>
    /// Entry point for the scout system or any other acquisition method.
    /// Creates a Character from data and optional save data, stores it, and returns it.
    /// </summary>
    public Character AddCharacter(CharacterData characterData, int level)
    {
        Character character = new Character(characterData);
        character.SetLevel(99);

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
    
    /*

    public CharacterStorageSaveData Export()
    {
        CharacterStorageSaveData saveData = new CharacterStorageSaveData();
        saveData.CharacterSaveDataList = new List<CharacterSaveData>();

        foreach (Character character in characters.Values)
        {
            saveData.CharacterSaveDataList.Add(character.Export());
        }

        return saveData;
    }

    public void Import(CharacterStorageSaveData saveData)
    {
        characters.Clear();

        if (saveData?.CharacterSaveDataList == null) return;

        foreach (CharacterSaveData characterSaveData in saveData.CharacterSaveDataList)
        {
            CharacterData characterData = CharacterManager.Instance.GetCharacterData(characterSaveData.CharacterId);
            if (characterData != null)
            {
                AddCharacter(characterData, characterSaveData);
            }
            else
            {
                LogManager.Warning($"[CharacterStorage] CharacterData not found for ID: {characterSaveData.CharacterId}");
            }
        }
    }

    public void Clear()
    {
        characters.Clear();
    }

    */

    #endregion
}
