using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Input;

public class SubstitutionManager : MonoBehaviour
{
    #region Fields

    public static SubstitutionManager Instance { get; private set; }

    private BattleType battleType = BattleType.Full;
    private int maxSubstitutions = 3;
    private Dictionary<TeamSide, int> substitutionsMade = new();
    private List<SubstitutionData> cachedSubstitutions = new();

    private DuelLogManager duelLogManager;

    //[SerializeField] private Dictionary<TeamSide, int> remainingChanges = new ();

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

        substitutionsMade[TeamSide.Home] = 0;
        substitutionsMade[TeamSide.Away] = 0;
    }

    private void Start() 
    {
        duelLogManager = DuelLogManager.Instance;
    }

    public void InitializeForBattle(BattleType battleType)
    {
        this.battleType = battleType;

        substitutionsMade[TeamSide.Home] = 0;
        substitutionsMade[TeamSide.Away] = 0;

        cachedSubstitutions.Clear();
    }

    #endregion

    #region Logic

    public int GetRemainingSubstitutions(TeamSide side)
    {
        if (!substitutionsMade.ContainsKey(side)) return maxSubstitutions;
        return maxSubstitutions - substitutionsMade[side];
    }

    public bool CanSubstitute(TeamSide side)
    {
        return GetRemainingSubstitutions(side) > 0;
    }

    public bool IsSubstitution(FormationCharacterSlotUI slotA, FormationCharacterSlotUI slotB)
    {
        return slotA.IsBench != slotB.IsBench;
    }

    public bool ValidateSwap(
        TeamSide side,
        FormationCharacterSlotUI slotA,
        FormationCharacterSlotUI slotB)
    {
        if (!IsSubstitution(slotA, slotB))
            return true;

        return TryUseSubstitution(side);
    }

    public bool TryUseSubstitution(TeamSide side)
    {
        if (!CanSubstitute(side))
        {
            TeamEvents.RaiseSubstitutionDenied(side);
            return false;
        }

        substitutionsMade[side]++;

        int remaining = GetRemainingSubstitutions(side);
        TeamEvents.RaiseSubstitutionMade(side, remaining);
        UIEvents.RaiseSubstitutionChangesUpdated(remaining,maxSubstitutions);
        // TeamEvents.RaiseSubstitutionResetPositions(team.TeamSide);

        LogManager.Trace($"[SubstitutionManager] {side} used a substitution. " +
                  $"Remaining: {remaining}/{maxSubstitutions}");

        return true;
    }

    #endregion

    #region Display

    public void ShowSubstitutions() 
    {
        if (cachedSubstitutions.Count == 0) return;

        // 1. Group substitutions by TeamSide to handle them independently
        var subsByTeam = new Dictionary<TeamSide, List<SubstitutionData>>();
        foreach (var sub in cachedSubstitutions)
        {
            if (!subsByTeam.ContainsKey(sub.TeamSide))
            {
                subsByTeam[sub.TeamSide] = new List<SubstitutionData>();
            }
            subsByTeam[sub.TeamSide].Add(sub);
        }

        var finalSubstitutions = new List<SubstitutionData>();

        // 2. Process each team's substitution chain
        foreach (var kvp in subsByTeam)
        {
            TeamSide side = kvp.Key;
            List<SubstitutionData> teamSubs = kvp.Value;

            // Track the current state of characters on the field for this team
            // Key: The character currently IN, Value: The original character they replaced (OUT)
            var activeSubs = new Dictionary<Character, Character>();

            foreach (var sub in teamSubs)
            {
                // If the character coming OUT was already swapped IN during this sequence,
                // we collapse/chain the substitution.
                if (activeSubs.ContainsKey(sub.CharacterOut))
                {
                    Character originalOut = activeSubs[sub.CharacterOut];
                    activeSubs.Remove(sub.CharacterOut);

                    // Only keep the chain if it didn't completely revert to the original state
                    if (sub.CharacterIn != originalOut)
                    {
                        activeSubs[sub.CharacterIn] = originalOut;
                    }
                }
                else
                {
                    // New substitution chain link: only add if it's not a dummy self-substitution
                    if (sub.CharacterIn != sub.CharacterOut)
                    {
                        activeSubs[sub.CharacterIn] = sub.CharacterOut;
                    }
                }
            }

            // 3. Reconstruct the valid, non-cancelled substitution data
            foreach (var pair in activeSubs)
            {
                finalSubstitutions.Add(new SubstitutionData(pair.Key, pair.Value, side));
            }
        }

        // 4. Order by TeamSide and display the clean list
        var orderedList = finalSubstitutions.OrderBy(s => s.TeamSide).ToList();
        foreach (SubstitutionData substitution in orderedList)
        {
            duelLogManager.AddActionSubstitution(substitution.CharacterIn, substitution.TeamSide);
        }

        cachedSubstitutions.Clear(); 
    }

    #endregion

    #region EnemyAiSubstitution

    public void TryEnemyAiSubstitution()
    {
        // Mini battles do not support substitutions.
        if (battleType == BattleType.Mini)
            return;

        const TeamSide enemySide = TeamSide.Away;

        // No substitutions remaining.
        if (!CanSubstitute(enemySide))
            return;

        Team enemyTeam = BattleManager.Instance.Teams[enemySide];

        if (enemyTeam == null)
            return;

        List<CharacterEntityBattle> entities =
            enemyTeam.GetCharacterEntities(battleType);

        List<Character> characters =
            enemyTeam.GetCharacters(battleType);

        if (entities == null || characters == null)
            return;

        if (entities.Count == 0 || characters.Count <= 11)
            return;

        // =========================================================
        // ACTIVE PLAYERS
        // =========================================================
        //
        // CharacterEntityBattle only contains the active players:
        // slots 0-10.
        //
        // Slot 0 is the goalkeeper, but there is no special logic
        // required. A goalkeeper is simply another fainted player
        // whose Position is GK.
        //
        for (int fieldIndex = 0;
             fieldIndex <= 10 && fieldIndex < entities.Count;
             fieldIndex++)
        {
            if (!CanSubstitute(enemySide))
                break;

            CharacterEntityBattle fieldEntity = entities[fieldIndex];

            if (fieldEntity == null || fieldEntity.Character == null)
                continue;

            // Only replace fainted characters.
            if (!fieldEntity.IsFainted)
                continue;

            Position requiredPosition = fieldEntity.Position;

            Character replacement = null;
            int replacementIndex = -1;


            // =====================================================
            // FIND REPLACEMENT ON BENCH
            // =====================================================
            //
            // The full character list contains slots 0-15.
            // Bench slots are 11-15.
            //
            for (int benchIndex = 11;
                 benchIndex <= 15 && benchIndex < characters.Count;
                 benchIndex++)
            {
                Character candidate = characters[benchIndex];

                // Bench slot is empty.
                if (candidate == null)
                    continue;

                // Candidate must not be fainted.
                if (candidate.IsFainted)
                    continue;

                // Candidate must have the same position.
                if (candidate.Position != requiredPosition)
                    continue;

                replacement = candidate;
                replacementIndex = benchIndex;

                break;
            }


            // No suitable replacement.
            if (replacement == null)
                continue;


            // =====================================================
            // PERFORM SUBSTITUTION
            // =====================================================

            SwapEnemyCharacters(
                enemyTeam,
                fieldIndex,
                fieldEntity,
                replacementIndex,
                replacement);
        }
    }


    private void SwapEnemyCharacters(
        Team enemyTeam,
        int fieldIndex,
        CharacterEntityBattle fieldEntity,
        int benchIndex,
        Character benchCharacter)
    {
        if (!CanSubstitute(enemyTeam.TeamSide))
            return;

        if (fieldEntity == null || benchCharacter == null)
            return;

        if (fieldEntity.Character == null)
            return;

        string guidA = fieldEntity.CharacterGuid;
        string guidB = benchCharacter.CharacterGuid;

        TeamManager.Instance.SwapCharactersInBattle(
            enemyTeam,
            battleType,
            fieldIndex,
            fieldEntity.FormationCoord,
            guidA,
            benchIndex,
            fieldEntity.FormationCoord, //not used when A is field and b is bench
            guidB);

        TryUseSubstitution(enemyTeam.TeamSide);

        LogManager.Trace(
            $"[SubstitutionManager] Enemy substitution: " +
            $"{fieldEntity.CharacterId} -> " +
            $"{benchCharacter.CharacterId}");
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        BattleEvents.OnBattleStarted += HandleBattleStarted;
        TeamEvents.OnCharacterSubstituted += HandleCharacterSubstituted;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStarted -= HandleBattleStarted;
        TeamEvents.OnCharacterSubstituted -= HandleCharacterSubstituted;
    }

    private void HandleBattleStarted(BattleType battleType) 
    {
        InitializeForBattle(battleType);
    }

    private void HandleCharacterSubstituted(Character characterIn, Character characterOut, TeamSide teamSide)
    {
        cachedSubstitutions.Add(new SubstitutionData(characterIn, characterOut, teamSide));
    }

    #endregion

}
