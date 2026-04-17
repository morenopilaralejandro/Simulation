using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using Aremoreno.Enums.Battle;

public class TeamManager : MonoBehaviour
{
    #region Fields

    public static TeamManager Instance { get; private set; }

    [SerializeField] private LocalizedString defaultName;

    private TeamManagerLoadout loadoutSystem;
    private TeamManagerPersistance persistanceSystem;

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
        loadoutSystem = new TeamManagerLoadout(defaultName);
        persistanceSystem = new TeamManagerPersistance();
        //encounterSystem.Subscribe();
    }

    #endregion

    #region API

    // loadoutSystem
    public IReadOnlyDictionary<string, Team> Loadouts => loadoutSystem.Loadouts;
    public Team ActiveLoadout => loadoutSystem.ActiveLoadout;
    public string ActiveLoadoutGuid => loadoutSystem.ActiveLoadoutGuid;
    public string DEFAULT_NAME => loadoutSystem.DEFAULT_NAME;
    public const int MAX_LOADOUTS = TeamManagerLoadout.MAX_LOADOUTS;
    public const string DEFAULT_CREST_ID = TeamManagerLoadout.DEFAULT_CREST_ID;
    public const string TEAM_CREST_ID_COMMON = TeamManagerLoadout.TEAM_CREST_ID_COMMON;
    public const string TEAM_CREST_ID_RARE = TeamManagerLoadout.TEAM_CREST_ID_RARE;
    public const string DEFAULT_KIT_ID = TeamManagerLoadout.DEFAULT_KIT_ID;
    public const string DEFAULT_FULL_BATTLE_FORMATION_ID = TeamManagerLoadout.DEFAULT_FULL_BATTLE_FORMATION_ID;
    public const string DEFAULT_MINI_BATTLE_FORMATION_ID = TeamManagerLoadout.DEFAULT_MINI_BATTLE_FORMATION_ID;
    public const int SIZE_MAX = TeamManagerLoadout.SIZE_MAX;
    public const int SIZE_FULL = TeamManagerLoadout.SIZE_FULL;
    public const int SIZE_MINI = TeamManagerLoadout.SIZE_MINI;
    public Team CreateLoadout() => loadoutSystem.CreateLoadout();
    public bool DeleteLoadout(string teamGuid) => loadoutSystem.DeleteLoadout(teamGuid);
    public Team GetLoadout(string teamGuid) => loadoutSystem.GetLoadout(teamGuid);
    public List<Team> GetAllLoadouts() => loadoutSystem.GetAllLoadouts();
    public void SetActiveLoadout(string teamGuid) => loadoutSystem.SetActiveLoadout(teamGuid);
    public bool HasActiveLoadout() => loadoutSystem.HasActiveLoadout();
    public void SetCharacterInLoadout(string teamGuid, BattleType battleType, int slotIndex, string characterGuid) => loadoutSystem.SetCharacterInLoadout(teamGuid, battleType, slotIndex, characterGuid);
    public void RemoveCharacterFromLoadout(string teamGuid, BattleType battleType, string characterGuid) => loadoutSystem.RemoveCharacterFromLoadout(teamGuid, battleType, characterGuid);
    public List<Character> ResolveCharacters(Team loadout, BattleType battleType) => loadoutSystem.ResolveCharacters(loadout, battleType);
    public Team InitializeFirstLoadout() => loadoutSystem.InitializeFirstLoadout();
    public SaveDataLoadoutSystem ExportLoadoutSystem() => loadoutSystem.Export();
    public void ImportLoadoutSystem(SaveDataLoadoutSystem saveData) => loadoutSystem.Import(saveData);

    // persistanceSystem
    public SaveDataTeamSystem Export() => persistanceSystem.Export();
    public void Import(SaveDataTeamSystem saveData) => persistanceSystem.Import(saveData);

    #endregion

}
