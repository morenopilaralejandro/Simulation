using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;
using Simulation.Enums.Duel;
using Simulation.Enums.Battle;

public class DuelManager : MonoBehaviour
{
    public static DuelManager Instance { get; private set; }

    private Duel duel;
    private IDuelHandler duelHandler;
    private List<DuelParticipantData> stagedParticipants = new List<DuelParticipantData>();

    private int maxSupporters = 2;
    private float supporterRadius = 1f;

    private int hpWinner = -20;
    private int hpLoser = -5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        duel = new Duel();
    }

    public void AddParticipant(DuelParticipant participant)
    {
        duelHandler?.AddParticipant(participant);
    }

    public void Reset() 
    {
        duel.Reset();
        stagedParticipants.Clear();
        DuelSelectionManager.Instance.ResetSelections();
    }

    public bool IsResolved => duel.IsResolved;
    public bool IsKeeperDuel => duel.IsKeeperDuel;
    public DuelMode DuelMode => duel.DuelMode;
    public int GetParticipantCount() => duel.Participants.Count;
    public int GetStagedParticipantCount() => stagedParticipants.Count;

    public DuelParticipant GetLastOffense() => duel.LastOffense;
    public DuelParticipant GetLastDefense() => duel.LastDefense;

    public DuelAction GetActionByCategory(Category category) =>
    category switch
    {
        Category.Shoot or Category.Dribble => 
            DuelAction.Offense,
        _ => 
            DuelAction.Defense
    };

    public Trait? GetRequiredTraitByCategory(Category category) 
    {
        //this method is for shoot duel only
        if (category == Category.Shoot) 
        {
            if (duel.Participants.Count == 0) 
            {
                //if (isLongShootStart)
                    return Trait.Long;
            } else 
            {
                return Trait.Chain;
            }
        } else if (category == Category.Block) 
        {
            return Trait.Block;
        }

        return null;
    }

    public bool CanSelectMoveCommand(Category category) 
    {
        return !(duel.IsKeeperDuel && category == Category.Dribble);
    }

    public bool CanSelectRegularCommands(Category category) 
    {
        if (category == Category.Shoot) 
        {
            if (duel.Participants.Count == 0) 
            {
                //if (isLongShootStart)
                    return false;
            } else 
            {
                return false;
            }
                 
        }

        if (category == Category.Block &&
            duel.DuelMode == DuelMode.Shoot) 
            return false;
    
        return true;
    }

    #region Field
    public void StartFieldDuel(Character offense, Character defense) 
    {
        LogManager.Info(
            $"[DuelManager] " +  
            $"Starting field duel between " +
            $"{offense.CharacterId} ({offense.TeamSide}) and " +
            $"{defense.CharacterId} ({defense.TeamSide})", this);
           
        StartDuel(DuelMode.Field);
        //PlayDuelStartEffect();
        //AudioManager.Instance.PlaySfx("SfxDuelField");

        duel.IsKeeperDuel = 
            defense.IsKeeper &&
            defense.IsInOwnPenaltyArea();

        //Support
        List<Character> offenseSupports = FindNearbySupporters(offense);
        List<Character> defenseSupports = FindNearbySupporters(defense);
        duel.OffenseSupports.AddRange(offenseSupports);
        duel.DefenseSupports.AddRange(defenseSupports);

        //UI
        BattleUIManager.Instance.SetDuelParticipant(offense, offenseSupports);
        BattleUIManager.Instance.SetDuelParticipant(defense, defenseSupports);
        BattleUIManager.Instance.ShowDuelParticipantsPanel();

        //RegisterTrigger
        DuelManager.Instance.RegisterTrigger(
            offense, 
            false);
        DuelManager.Instance.RegisterTrigger(
            defense, 
            false);

        //SetPreselection
        DuelSelectionManager.Instance.SetPreselection(
            offense.TeamSide, 
            Category.Dribble, 
            0, 
            offense);
        DuelSelectionManager.Instance.SetPreselection(
            defense.TeamSide, 
            Category.Block, 
            1, 
            defense);
        DuelSelectionManager.Instance.StartSelectionPhase();
    }
    #endregion

    #region Shoot
    public void StartShootDuel(Character character, bool isDirect) 
    {
        LogManager.Info($"[DuelManager] " +
            $"Shoot duel started by " +
            $"{character.CharacterId}, " +
            $"teamSide {character.TeamSide}, " +
            $"isDirect {isDirect}", this);       

        character.StartKick();
        DuelManager.Instance.StartDuel(DuelMode.Shoot);
        ShootTriangleManager.Instance.SetTriangleFromCharacter(character);
        BattleEvents.RaiseShootPerformed(character, isDirect);

        //UI
        BattleUIManager.Instance.SetDuelParticipant(character, null);
        BattleUIManager.Instance.SetDuelParticipant(GoalManager.Instance.GetOpponentKeeper(character), null);
        BattleUIManager.Instance.ShowDuelParticipantsPanel();

        //RegisterTrigger
        DuelManager.Instance.RegisterTrigger(character, isDirect);

        //SetPreselection
        DuelSelectionManager.Instance.SetPreselection(
            character.TeamSide, 
            Category.Shoot, 
            0, 
            character);
        DuelSelectionManager.Instance.SetShootDuelSelectionTeamSide(
            character.TeamSide);
        DuelSelectionManager.Instance.StartSelectionPhase();        
    }

    public void StartShootDuelCombo(Character character, Category category) 
    {
        int participantIndex = duel.Participants.Count;
        LogManager.Info(
            $"[DuelManager] " +  
            $"Registering combo trigger for " +
            $"{character.CharacterId} ({character.TeamSide}), " +
            $"participantIndex {participantIndex}, " +
            $"category {category}", this);

        //Travel
        BattleManager.Instance.Ball.PauseTravel();

        //UI
        BattleUIManager.Instance.SetDuelParticipant(character, null);
        //AudioManager.Instance.PlaySfx("SfxDuelShoot");
        

        //RegisterTrigger
        DuelManager.Instance.RegisterTrigger(character, false);
        //SetPreselection
        DuelSelectionManager.Instance.SetPreselection(
            character.TeamSide, 
            category, 
            participantIndex, 
            character);
        DuelSelectionManager.Instance.SetShootDuelSelectionTeamSide(
            character.TeamSide);
        DuelSelectionManager.Instance.StartSelectionPhase();
    }

    public void StartShootDuelReversal() 
    {
        //triangle and travel to the other goal, keep the same duel
    }
    #endregion

    #region Duel Interface
    public void StartDuel(DuelMode duelMode)
    {
        Reset();
        duel.DuelMode = duelMode;
        DuelEvents.RaiseDuelStart(duelMode);
        BattleEffectManager.Instance.PlayDuelStartEffect(BattleManager.Instance.Ball.transform);
        switch (DuelMode)
        {
            case DuelMode.Field:
                duelHandler = new FieldDuelHandler(duel);
                AudioManager.Instance.PlaySfx("sfx-duel_start_field");
                break;
            case DuelMode.Shoot:
                duelHandler = new ShootDuelHandler(duel);
                AudioManager.Instance.PlaySfx("sfx-duel_start_shoot");
                break;
            /*
            case DuelMode.Air:
                duelHandler = new AirDuelHandler(duel);
                break;
            */
        }
    }

    public void CancelDuel()
    {
        LogManager.Info("[DuelManager] Duel cancelled", this);
        DuelEvents.RaiseDuelCancel();
        duel.IsResolved = true;
        BattleUIManager.Instance.HideDuelParticipantsPanel();
        duelHandler.CancelDuel();
    }

    public void EndDuel(DuelParticipant winner, DuelParticipant loser)
    {
        bool isWinnerUser = winner.Character.TeamSide == BattleManager.Instance.GetUserSide();
        LogManager.Info(
            $"[DuelManager] EndDuel " +
            $"Winner {winner.Character?.CharacterId}, " +
            $"TeamSide {winner.Character?.TeamSide}, " +
            $"Action {winner.Action}, " +
            $"Category {winner.Category}", this);
        DuelEvents.RaiseDuelEnd(winner, loser, isWinnerUser);

        winner.Character.ModifyBattleStat(Stat.Hp, hpWinner);
        loser.Character.ModifyBattleStat(Stat.Hp, hpLoser);

        if (isWinnerUser)
            AudioManager.Instance.PlaySfx("sfx-duel_win");
        else
            AudioManager.Instance.PlaySfx("sfx-duel_lose");

        BattleEffectManager.Instance.StopDuelStartEffect();

        BattleUIManager.Instance.HideDuelParticipantsPanel();
        duel.IsResolved = true;
    }
    #endregion

    #region Element
    public void ApplyElementalEffectiveness(DuelParticipant offense, DuelParticipant defense)
    {
        if (DamageCalculator.IsEffective(defense.CurrentElement, offense.CurrentElement))
        {
            defense.Damage *= 2f;
            DuelLogManager.Instance.AddElementDefense(defense.Character);
            LogManager.Info("[DuelManager] Defense element is effective", this);
        }
        else if (DamageCalculator.IsEffective(offense.CurrentElement, defense.CurrentElement))
        {
            if (duel.DuelMode == DuelMode.Shoot)
                duel.OffensePressure -= offense.Damage;
            offense.Damage *= 2;
            if (duel.DuelMode == DuelMode.Shoot)
                duel.OffensePressure += offense.Damage;
            DuelLogManager.Instance.AddElementOffense(offense.Character);
            LogManager.Info("[DuelManager] Offense element is effective", this);
        }
    }
    #endregion

    #region Support
    public List<Character> FindNearbySupporters(Character character)
    {
        List<Character> supporters = new List<Character>();
        HashSet<Character> uniqueCharacters = new HashSet<Character>();


        Collider[] nearbyColliders = Physics.OverlapSphere(
            character.transform.position,
            supporterRadius
        );

        foreach (var collider in nearbyColliders)
        {
            Character nearbyCharacter = collider.GetComponentInParent<Character>();

            if (supporters.Count >= maxSupporters)
                break;

            if (nearbyCharacter == null ||
                !uniqueCharacters.Add(nearbyCharacter) ||
                nearbyCharacter == character ||
                !nearbyCharacter.IsSameTeam(character) ||
                nearbyCharacter.IsKeeper ||
                !nearbyCharacter.CanDuel())
                continue;
            
            LogManager.Trace("[DuelManager] Added support " + 
                $"({character.TeamSide}) " +
                $"{nearbyCharacter.CharacterId}", this);
            supporters.Add(nearbyCharacter);
        }

        return supporters;
    }
    #endregion

    #region Participant Registration
    public void RegisterTrigger(
        Character character, 
        bool isDirect)
    {
        var pd = new DuelParticipantData { 
            Character = character, 
            IsDirect = isDirect };
        stagedParticipants.Add(pd);
        TryFinalizeParticipant(pd);
    }

    public void RegisterSelection(int index, Category category, DuelCommand command, Move move)
    {
        /*
        GameLogger.Info(
            $"[DuelManager] RegisterSelection participantIndex={index}, category={category}, command={command}, secret={(secret != null ? secret.SecretName : "None")}, stagedCount={stagedParticipants.Count}",
            this);
        */
        if (index < 0 || index >= stagedParticipants.Count)
        {
            //GameLogger.Error("[DuelManager] Invalid participant index", this);
            return;
        }
        var pd = stagedParticipants[index];
        pd.Category = category;
        pd.Action = GetActionByCategory(category);
        pd.Command = command;
        pd.Move = move;
        TryFinalizeParticipant(pd);
    }

    private void TryFinalizeParticipant(
        DuelParticipantData pd)
    {
        if (!pd.IsComplete) return;

        var participant = new DuelParticipant(
            pd.Character,
            pd.Category.Value,
            pd.Action.Value,
            pd.Command.Value,
            pd.Move,
            pd.IsDirect,
            duel.IsKeeperDuel
        );

        /*
        LogManager.Trace($"[DuelManager] Created participant: " +  
            $"{participant.Character.CharacterId}, " + 
            $"{participant.Character.TeamSide}", this);
        */
        AddParticipant(participant);
    }
    #endregion
}
