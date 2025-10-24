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
        duelHandler.StartDuel();
    }

    public void AddParticipant(DuelParticipant participant)
    {
        duelHandler?.AddParticipant(participant);
    }

    public void Reset() 
    {
        duel.Reset();
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

    private void Cancel()
    {
        //GameLogger.Warning("[DuelManager] Duel cancelled", this);

        duelHandler.Cancel(); 
        duel.IsResolved = true;
        //ShootTriangle.Instance.SetTriangleVisible(false);
        //BallTrail.Instance.SetTrailVisible(false);
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

    public void SetIsKeeper(bool isKeeper) {
        duel.IsKeeper = isKeeper;
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
            pd.IsDirect
        );

        //GameLogger.DebugLog($"[DuelManager] Created participant: {participant.Player.PlayerName}", this);
        AddParticipant(participant);
    }
    #endregion
}
