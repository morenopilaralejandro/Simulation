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
    private TeamSide shootTeamSide;
    private TeamSide ShootTeamSide => shootTeamSide;

    private int maxSupporters = 2;
    private float supporterRadius = 1.5f;

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

    public void StartDuel(DuelMode DuelMode)
    {
        Reset();
        //PlayDuelStartEffect();
        //AudioManager.Instance.PlaySfx("SfxDuelField");
        BattleUIManager.Instance.ShowDuelParticipantsPanel();
        switch (DuelMode)
        {
            case DuelMode.Field:
                duelHandler = new FieldDuelHandler(duel);
                break;
            /*
            case DuelMode.Shoot:
                duelHandler = new ShootDuelHandler(duel);
                break;
            case DuelMode.Air:
                duelHandler = new AirDuelHandler(duel);
                break;
            */
        }
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

    public DuelMode DuelMode => duel.DuelMode;

    public DuelAction GetActionByCategory(Category category) 
    {
        if (category == Category.Shoot ||
            category == Category.Dribble)
            return DuelAction.Offense;
        return DuelAction.Defense;
    }

    private void CancelDuel()
    {
        //GameLogger.Warning("[DuelManager] Duel cancelled", this);
        duel.IsResolved = true;
        //ShootTriangle.Instance.SetTriangleVisible(false);
        //BallTrail.Instance.SetTrailVisible(false);
        duelHandler.CancelDuel();
    }

    public void EndDuel(DuelParticipant winner, DuelParticipant loser)
    {
        LogManager.Info(
            $"[DuelManager] EndDuel " +
            $"Winner {winner.Character?.CharacterId}, " +
            $"TeamSide {winner.Character?.TeamSide}, " +
            $"Action {winner.Action}, " +
            $"Category {winner.Category}", this);

        if (winner.Character.TeamSide == BattleManager.Instance.GetUserSide())
        {
            //DuelLogManager.Instance.AddDuelWin(winningParticipant.Player.TeamIndex);
            //AudioManager.Instance.PlaySfx("SfxDuelWin");
        }
        else
        {
            //DuelLogManager.Instance.AddDuelLose(winningParticipant.Player.TeamIndex);
            //AudioManager.Instance.PlaySfx("SfxDuelLose");
        }


        /*
        if (winner.Action == DuelAction.Defense)
        {
            //BallTravelController.Instance.CancelTravel();
            PossessionManager.Instance.Gain(winner.Character);
            duel.LastOffense.Character.ApplyStatus(StatusEffect.Stunned);
        }
        */

        BattleUIManager.Instance.HideDuelParticipantsPanel();
        duel.IsResolved = true;
    }

    public void ApplyElementalEffectiveness(DuelParticipant offense, DuelParticipant defense)
    {
        if (DamageCalculator.IsEffective(defense.CurrentElement, offense.CurrentElement))
        {
            defense.Damage *= 2f;
            //DuelLogManager.Instance.AddElementDefense(defense.Category);
            LogManager.Info("[DuelManager] Defense element is effective!", this);
        }
        else if (DamageCalculator.IsEffective(offense.CurrentElement, defense.CurrentElement))
        {
            //currentDuel.AttackPressure -= offense.Damage;
            offense.Damage *= 2;
            //currentDuel.AttackPressure += offense.Damage;
            //DuelLogManager.Instance.AddElementOffense(offense.Category);
            LogManager.Info("[DuelManager] Offense element is effective!", this);
        }
    }

    public List<Character> FindNearbySupporters(Character character)
    {
        List<Character> supporters = new List<Character>();

        foreach (var teammate in character.GetTeammates())
        {
            if (supporters.Count >= maxSupporters)
                break;

            if (teammate == character ||
                teammate.IsKeeper ||
                !teammate.CanDuel())
                continue;
            
            if (
                Vector3.Distance(
                    character.transform.position, 
                    teammate.transform.position) < supporterRadius)
                supporters.Add(teammate);
        }

        return supporters;
    }

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

    public void SetShootTeamSide(TeamSide teamSide) {
        shootTeamSide = teamSide;
    } 

    public void SetIsKeeperDuel(bool isKeeperDuel) {
        duel.IsKeeperDuel = isKeeperDuel;
    } 

    public void SetOffenseSupports(List<Character> supports) 
    {
        duel.OffenseSupports.AddRange(supports);
    }

    public void SetDefenseSupports(List<Character> supports) 
    {
        duel.DefenseSupports.AddRange(supports);   
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

        //GameLogger.DebugLog($"[DuelManager] Created participant: {participant.Player.PlayerName}", this);
        AddParticipant(participant);
    }
    #endregion
}
