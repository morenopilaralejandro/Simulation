using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Aremoreno.Enums.Battle;

/// <summary>
/// Manages all user-created team loadouts and tracks which one is active.
/// </summary>
public class TeamManagerLoadout
{
    #region Fields

    private LocalizedString defaultName;

    private Dictionary<string, Team> loadouts = new();
    private string activeLoadoutGuid;
    private CharacterManager characterManager;

    public IReadOnlyDictionary<string, Team> Loadouts => loadouts;
    public Team ActiveLoadout => GetLoadout(activeLoadoutGuid);
    public string ActiveLoadoutGuid => activeLoadoutGuid;

    #endregion

    #region Constants

    public string DEFAULT_NAME = "custom team";
    public const int MAX_LOADOUTS = 10;

    public const string DEFAULT_CREST_ID = "faith_selection";
    public const string TEAM_CREST_ID_COMMON = "generic_common";
    public const string TEAM_CREST_ID_RARE = "generic_rare";
    public const string DEFAULT_KIT_ID = "faith";
    public const string DEFAULT_FULL_BATTLE_FORMATION_ID = "faith";
    public const string DEFAULT_MINI_BATTLE_FORMATION_ID = "offense";

    public const int SIZE_MAX = 16;
    public const int SIZE_FULL = 11;
    public const int SIZE_MINI = 4;

    #endregion

    #region Constructor

    public TeamManagerLoadout(LocalizedString defaultName)
    {
        characterManager = CharacterManager.Instance;
        this.defaultName = defaultName;

        DEFAULT_NAME = defaultName.GetLocalizedString();
    }

    #endregion

    #region CRUD

    public Team CreateLoadout()
    {
        if (loadouts.Count >= MAX_LOADOUTS)
        {
            LogManager.Warning($"[TeamLoadoutManager] Max loadouts ({MAX_LOADOUTS}) reached.");
            return null;
        }

        Team loadout = TeamFactory.Create();
        loadouts[loadout.TeamGuid] = loadout;
        InitializeLoadoutCharacters(loadout);

        // Auto-set as active if it's the first loadout
        if (loadouts.Count == 1)
            SetActiveLoadout(loadout.TeamGuid);

        TeamEvents.RaiseLoadoutCreated(loadout);
        LogManager.Info($"[TeamLoadoutManager] Created loadout: {loadout.TeamName} ({loadout.TeamGuid})");
        return loadout;
    }

    public bool DeleteLoadout(string teamGuid)
    {
        if (!loadouts.TryGetValue(teamGuid, out Team loadout))
        {
            LogManager.Warning($"[TeamLoadoutManager] Loadout {loadout.TeamName} not found for deletion.");
            return false;
        }

        if (loadouts.Count == 1)
        {
            LogManager.Warning($"[TeamLoadoutManager] Can't delete when there is only one loadout");
            return false;
        }

        loadouts.Remove(teamGuid);
        TeamEvents.RaiseLoadoutDeleted(loadout);

        // If we deleted the active loadout, pick another one
        if (activeLoadoutGuid == teamGuid)
        {
            activeLoadoutGuid = loadouts.Count > 0 ? loadouts.Keys.First() : null;
            if (activeLoadoutGuid != null)
                TeamEvents.RaiseActiveLoadoutChanged(loadouts[activeLoadoutGuid]);
        }

        LogManager.Info($"[TeamLoadoutManager] Deleted loadout: {loadout.TeamName} ({loadout.TeamGuid})");
        return true;
    }

    public Team GetLoadout(string teamGuid)
    {
        if (string.IsNullOrEmpty(teamGuid)) return null;
        loadouts.TryGetValue(teamGuid, out Team loadout);
        return loadout;
    }

    public List<Team> GetAllLoadouts()
    {
        return loadouts.Values.ToList();
    }

    #endregion

    #region Active Loadout

    public void SetActiveLoadout(string teamGuid)
    {
        if (!loadouts.ContainsKey(teamGuid))
        {
            LogManager.Warning($"[TeamLoadoutManager] Cannot set active loadout. ID {teamGuid} not found.");
            return;
        }

        activeLoadoutGuid = teamGuid;
        TeamEvents.RaiseActiveLoadoutChanged(loadouts[teamGuid]);
        LogManager.Info($"[TeamLoadoutManager] Active loadout set to: {loadouts[teamGuid].TeamName} ({teamGuid})");
    }

    public bool HasActiveLoadout()
    {
        return activeLoadoutGuid != null && loadouts.ContainsKey(activeLoadoutGuid);
    }

    #endregion

    #region Loadout Character Management

    public void SetCharacterInLoadout(Team loadout, BattleType battleType, int slotIndex, string characterGuid)
    {
        if (loadout == null)
        {
            LogManager.Warning($"[TeamLoadoutManager] Loadout {loadout.TeamGuid} not found.");
            return;
        }

        // Prevent duplicate characters in the same roster
        List<string> guids = loadout.GetCharacterGuids(battleType);
        if (!string.IsNullOrEmpty(characterGuid) && guids.Contains(characterGuid))
        {
            // Swap: find the existing slot and clear it
            int existingIndex = guids.IndexOf(characterGuid);
            if (existingIndex != slotIndex)
            {
                string swapGuid = (slotIndex < guids.Count) ? guids[slotIndex] : null;
                loadout.SetCharacterGuid(battleType, existingIndex, swapGuid);
            }
        }

        loadout.SetCharacterGuid(battleType, slotIndex, characterGuid);
        TeamEvents.RaiseLoadoutUpdated(loadout);
    }

    public void SwapCharactersInBattle(
        Team loadout, BattleType battleType,
        int slotIndexA, FormationCoord coordA, string guidA,
        int slotIndexB, FormationCoord coordB, string guidB)
    {
        List<CharacterEntityBattle> entities = loadout.GetCharacterEntities(battleType);

        if (entities == null)
        {
            Debug.LogError($"SwapCharactersInBattle: entities null for {battleType}");
            return;
        }

        // Resolve BOTH references BEFORE modifying anything
        CharacterEntityBattle entityA = null;
        CharacterEntityBattle entityB = null;

        int count = entities.Count;
        for (int i = 0; i < count; i++)
        {
            if (entities[i] == null)
                continue;

            string guid = entities[i].CharacterGuid;

            if (entityA == null && guid == guidA)
                entityA = entities[i];

            if (entityB == null && guid == guidB)
                entityB = entities[i];

            if (entityA != null && entityB != null)
                break;
        }

        LogManager.Trace($"[TeamManagerLoadout] guidA: {guidA}, entityA found: {entityA != null}, ref: {entityA?.GetHashCode()}");
        LogManager.Trace($"[TeamManagerLoadout] guidB: {guidB}, entityB found: {entityB != null}, ref: {entityB?.GetHashCode()}");
        LogManager.Trace($"[TeamManagerLoadout] coordA: {coordA.Position}, coordB: {coordB.Position}");

        // Now swap — same references, no new lookups
        loadout.SetCharacterEntity(battleType, slotIndexA, entityB);
        loadout.SetCharacterEntity(battleType, slotIndexB, entityA);

        // Raise events with the EXACT same references
        if (entityB != null)
            TeamEvents.RaiseAssignCharacterToTeamBattle(entityB, loadout, coordA);

        if (entityA != null)
            TeamEvents.RaiseAssignCharacterToTeamBattle(entityA, loadout, coordB);
    }

    public void RemoveCharacterFromLoadout(Team loadout, BattleType battleType, string characterGuid)
    {
        if (loadout == null) return;

        loadout.RemoveCharacterGuid(battleType, characterGuid);
        TeamEvents.RaiseLoadoutUpdated(loadout);
    }

    /*

    public void SetFormation(string loadoutId, BattleType battleType, string formationId)
    {
        TeamLoadout loadout = GetLoadout(loadoutId);
        if (loadout == null) return;

        if (battleType == BattleType.Full)
            loadout.FullBattleFormationId = formationId;
        else
            loadout.MiniBattleFormationId = formationId;

        TeamEvents.RaiseLoadoutUpdated(loadout);
    }

    */

    #endregion

    #region Resolve Characters

    /// <summary>
    /// Resolves a loadout's character GUIDs into actual Character instances from storage.
    /// Returns the list in slot order (nulls filtered out).
    /// </summary>
    public List<Character> ResolveCharactersFromStorage(Team loadout, BattleType battleType)
    {
        List<Character> resolved = new();
        List<string> guids = loadout.GetCharacterGuids(battleType);

        foreach (string guid in guids)
        {
            if (string.IsNullOrEmpty(guid)) continue;

            Character character = characterManager.GetCharacter(guid);
            if (character != null)
            {
                resolved.Add(character);
            }
            else
            {
                LogManager.Warning($"[TeamLoadoutManager] Character GUID {guid} not found in storage. Skipping.");
            }
        }

        return resolved;
    }

    public List<Character> ResolveCharactersFromBattle(Team loadout, BattleType battleType)
    {
        return loadout.GetCharacters(battleType);
    }

    #endregion

    #region First Time Initialize

    /// <summary>
    /// Creates the initial default loadout and populates it with the first SizeFull
    /// and first SizeMini characters found in character storage.
    /// Should only be called once when the player has no existing loadouts.
    /// </summary>
    public Team InitializeFirstLoadout()
    {
        if (loadouts.Count > 0)
        {
            LogManager.Warning("[TeamLoadoutManager] Loadouts already exist. Skipping first-time initialization.");
            return null;
        }

        Team loadout = CreateLoadout();
        SetActiveLoadout(loadout.TeamGuid);

        return loadout;
    }

    public void InitializeLoadoutCharacters(Team loadout)
    {
        if (loadout == null)
        {
            LogManager.Warning("[TeamLoadoutManager] Failed to create loadout.");
            return;
        }

        List<Character> allCharacters = characterManager.GetAllCharacters();
        if (allCharacters == null || allCharacters.Count == 0)
        {
            LogManager.Warning("[TeamLoadoutManager] No characters in storage. Loadout created but left empty.");
            return;
        }

        int sizeFull = TeamManager.SIZE_MAX;
        int sizeMini = TeamManager.SIZE_MINI;

        // Populate Full Battle slots
        int battleCount = Mathf.Min(sizeFull, allCharacters.Count);
        for (int i = 0; i < battleCount; i++)
        {
            SetCharacterInLoadout(loadout, BattleType.Full, i, allCharacters[i].CharacterGuid);
        }

        // Populate Mini Battle slots
        int miniCount = Mathf.Min(sizeMini, allCharacters.Count);
        for (int i = 0; i < miniCount; i++)
        {
            SetCharacterInLoadout(loadout, BattleType.Mini, i, allCharacters[i].CharacterGuid);
        }

        LogManager.Info($"[TeamLoadoutManager] Loadout initialized: " +
                        $"{battleCount} full-battle and {miniCount} mini-battle characters assigned.");

    }

    #endregion
   
    #region Persistence

    public SaveDataLoadoutSystem Export()
    {
        SaveDataLoadoutSystem saveData = new SaveDataLoadoutSystem();
        saveData.ActiveLoadoutGuid = activeLoadoutGuid;
        saveData.TeamSaveDataList = new List<TeamSaveData>();

        foreach (Team loadout in loadouts.Values)
        {
            saveData.TeamSaveDataList.Add(loadout.Export());
        }

        return saveData;
    }

    public void Import(SaveDataLoadoutSystem saveData)
    {
        loadouts.Clear();

        activeLoadoutGuid = null;

        if (saveData?.TeamSaveDataList == null) return;

        foreach (TeamSaveData teamSaveData in saveData.TeamSaveDataList)
        {
            Team loadout = TeamFactory.Create(teamSaveData);

            LogManager.Trace($"[TeamLoadoutManager] Imported loadout '{loadout.TeamName}' " +
            $"({loadout.TeamGuid}) with " +
            $"{loadout.GetCharacterGuids(BattleType.Full).Count(g => !string.IsNullOrEmpty(g))} full / " +
            $"{loadout.GetCharacterGuids(BattleType.Mini).Count(g => !string.IsNullOrEmpty(g))} mini characters.");

            loadouts[loadout.TeamGuid] = loadout;
        }

        string targetGuid = null;

        if (!string.IsNullOrEmpty(saveData.ActiveLoadoutGuid) && loadouts.ContainsKey(saveData.ActiveLoadoutGuid))
        {
            targetGuid = saveData.ActiveLoadoutGuid;
        }
        else if (loadouts.Count > 0)
        {
            targetGuid = loadouts.Keys.First();
        }

        if (targetGuid != null)
        {
            SetActiveLoadout(targetGuid);
        }
    }

    #endregion
}

/*

// Create a loadout
TeamLoadout loadout = TeamLoadoutManager.Instance.CreateLoadout("My Dream Team");

// Add characters from storage to the mini battle roster
List<Character> allCharacters = CharacterStorageManager.Instance.Storage.GetAllCharacters();
for (int i = 0; i < Mathf.Min(5, allCharacters.Count); i++)
{
    TeamLoadoutManager.Instance.SetCharacterInLoadout(
        loadout.LoadoutId, 
        BattleType.Mini, 
        i, 
        allCharacters[i].CharacterGuid);
}

// Set formation
TeamLoadoutManager.Instance.SetFormation(loadout.LoadoutId, BattleType.Mini, "4-4-2");

// Set as active
TeamLoadoutManager.Instance.SetActiveLoadout(loadout.LoadoutId);

*/
