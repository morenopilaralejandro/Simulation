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

    public void InitializeForBattle()
    {
        battleType = BattleManager.Instance.CurrentType;

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
        var orderedList = cachedSubstitutions.OrderBy(s => s.TeamSide).ToList();
        foreach (SubstitutionData substitution in orderedList)
            duelLogManager.AddActionSubstitution(substitution.CharacterIn, substitution.TeamSide);
    }

    #endregion

    #region Events

    private void OnEnable()
    {
        BattleEvents.OnBattleStart += HandleBattleStart;
        TeamEvents.OnCharacterSubstituted += HandleCharacterSubstituted;
    }

    private void OnDisable()
    {
        BattleEvents.OnBattleStart -= HandleBattleStart;
        TeamEvents.OnCharacterSubstituted -= HandleCharacterSubstituted;
    }

    private void HandleBattleStart() 
    {
        InitializeForBattle();
    }

    private void HandleCharacterSubstituted(Character characterIn, Character characterOut, TeamSide teamSide)
    {
        cachedSubstitutions.Add(new SubstitutionData(characterIn, characterOut, teamSide));
    }

    #endregion

}
