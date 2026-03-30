using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Simulation.Enums.Battle;

/// <summary>
/// Manages all player-created team loadouts and tracks which one is active.
/// </summary>
public class TeamLoadoutManager : MonoBehaviour
{
    #region Fields

    public static TeamLoadoutManager Instance { get; private set; }

    [SerializeField] private LocalizedString defaultName;

    private Dictionary<string, Team> loadouts = new();
    private string activeLoadoutGuid;
    private CharacterManager characterManager;

    public IReadOnlyDictionary<string, Team> Loadouts => loadouts;
    public Team ActiveLoadout => GetLoadout(activeLoadoutGuid);
    public string ActiveLoadoutGuid => activeLoadoutGuid;

    #endregion

    public const int MAX_LOADOUTS = 10;

    public string DEFAULT_NAME = "custom team";
    public const string DEFAULT_CREST_ID = "faith_selection";
    public const string TEAM_CREST_ID_COMMON = "generic_common";
    public const string TEAM_CREST_ID_RARE = "generic_rare";
    public const string DEFAULT_KIT_ID = "faith";
    public const string DEFAULT_FULL_BATTLE_FORMATION_ID = "faith";
    public const string DEFAULT_MINI_BATTLE_FORMATION_ID = "offense";

    #region Constants

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

        DEFAULT_NAME = defaultName.GetLocalizedString();
    }

    private void Start() 
    {
        characterManager = CharacterManager.Instance;
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

    public void SetCharacterInLoadout(string teamGuid, BattleType battleType, int slotIndex, string characterGuid)
    {
        Team loadout = GetLoadout(teamGuid);
        if (loadout == null)
        {
            LogManager.Warning($"[TeamLoadoutManager] Loadout {teamGuid} not found.");
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

    public void RemoveCharacterFromLoadout(string teamGuid, BattleType battleType, string characterGuid)
    {
        Team loadout = GetLoadout(teamGuid);
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
    public List<Character> ResolveCharacters(Team loadout, BattleType battleType)
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
        if (loadout == null)
        {
            LogManager.Warning("[TeamLoadoutManager] Failed to create initial loadout.");
            return null;
        }

        List<Character> allCharacters = characterManager.GetAllCharacters();
        if (allCharacters == null || allCharacters.Count == 0)
        {
            LogManager.Warning("[TeamLoadoutManager] No characters in storage. Loadout created but left empty.");
            return loadout;
        }

        int sizeFull = TeamManager.Instance.SizeFull;
        int sizeMini = TeamManager.Instance.SizeMini;

        // Populate Full Battle slots
        int battleCount = Mathf.Min(sizeFull, allCharacters.Count);
        for (int i = 0; i < battleCount; i++)
        {
            SetCharacterInLoadout(loadout.TeamGuid, BattleType.Full, i, allCharacters[i].CharacterGuid);
        }

        // Populate Mini Battle slots
        int miniCount = Mathf.Min(sizeMini, allCharacters.Count);
        for (int i = 0; i < miniCount; i++)
        {
            SetCharacterInLoadout(loadout.TeamGuid, BattleType.Mini, i, allCharacters[i].CharacterGuid);
        }

        LogManager.Info($"[TeamLoadoutManager] First-time loadout initialized: " +
                        $"{battleCount} full-battle and {miniCount} mini-battle characters assigned.");
    
        SetActiveLoadout(loadout.TeamGuid);
        return loadout;
    }

    #endregion
   
    #region Persistence
    /*
    public TeamLoadoutManagerSaveData Export()
    {
        TeamLoadoutManagerSaveData saveData = new TeamLoadoutManagerSaveData();
        saveData.ActiveLoadoutId = activeLoadoutId;
        saveData.LoadoutSaveDataList = new List<TeamLoadoutSaveData>();

        foreach (TeamLoadout loadout in loadouts.Values)
        {
            saveData.LoadoutSaveDataList.Add(loadout.Export());
        }

        return saveData;
    }

    public void Import(TeamLoadoutManagerSaveData saveData)
    {
        loadouts.Clear();
        activeLoadoutId = null;

        if (saveData?.LoadoutSaveDataList == null) return;

        foreach (TeamLoadoutSaveData loadoutSaveData in saveData.LoadoutSaveDataList)
        {
            TeamLoadout loadout = new TeamLoadout(loadoutSaveData.LoadoutId);
            loadout.Import(loadoutSaveData);
            loadouts[loadout.LoadoutId] = loadout;
        }

        if (!string.IsNullOrEmpty(saveData.ActiveLoadoutId) && loadouts.ContainsKey(saveData.ActiveLoadoutId))
        {
            activeLoadoutId = saveData.ActiveLoadoutId;
        }
        else if (loadouts.Count > 0)
        {
            activeLoadoutId = loadouts.Keys.First();
        }
    }
    */
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
