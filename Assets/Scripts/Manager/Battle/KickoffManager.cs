using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Input;

public class KickoffManager : MonoBehaviour
{
    public static KickoffManager Instance { get; private set; }

    private TeamSide teamSide;
    private bool isKickoffReady;
    private Dictionary<TeamSide, bool> isTeamReady;

    private Character character0;
    private Character character1;
    private Vector3 position0;
    private Dictionary<TeamSide, Vector3> position1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        position0 = new Vector3(0, 0.34f, 0);
        position1 = new Dictionary<TeamSide, Vector3>
        {
            { TeamSide.Home, new Vector3(-1f, 0.34f, -0.1f) },
            { TeamSide.Away, new Vector3(1f, 0.34f, 0.1f) }
        };

        isTeamReady = new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
    }

    private void OnDestroy()
    {
        // Safety cleanup
        BallEvents.OnGained -= OnBallGained;
    }

    void Update() 
    {
        if (InputManager.Instance.GetDown(CustomAction.Pass) && 
            BattleManager.Instance.CurrentPhase == BattlePhase.Deadball)
            SetTeamReady(BattleManager.Instance.GetUserSide());
    }

    public void StartKickoff(TeamSide teamSide) 
    {
        BattleManager.Instance.Freeze();
        this.teamSide = teamSide;
        ResetPositions();
        ResetReady();
        BattleManager.Instance.SetBattlePhase(BattlePhase.Deadball);
        AudioManager.Instance.PlaySfx("sfx-whistle_single");

        BallEvents.OnGained -= OnBallGained;
        BallEvents.OnGained += OnBallGained;
    }

    private void OnBallGained(Character character)
    {
        if (character != character0)
            return;

        if (BattleManager.Instance.CurrentPhase == BattlePhase.Deadball && 
            AutoBattleManager.Instance.IsAutoBattleEnabled)
            SetTeamReady(BattleManager.Instance.GetUserSide());

        BallEvents.OnGained -= OnBallGained;
    }

    private void ResetReady() 
    {
        isTeamReady = new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
        isKickoffReady = false;
    }

    private void SetTeamReady(TeamSide teamSide)
    {
        isTeamReady[teamSide] = true;
        TryResolve();
    }

    private void TryResolve()
    {
        bool isMultiplayer = false;
        if (isMultiplayer) 
        {
            isKickoffReady = 
                isTeamReady[TeamSide.Home] && 
                isTeamReady[TeamSide.Away];
        } 
        else 
        {
            isKickoffReady = isTeamReady[TeamSide.Home];
        }

        if (isKickoffReady) 
            PerformKickOff();
    }

    private void PerformKickOff()
    {
        BattleManager.Instance.SetBattlePhase(BattlePhase.Battle);
        BattleManager.Instance.Unfreeze();

        Character target = BattleManager.Instance.TargetedCharacter[character0.TeamSide];
        if (!target || character0.IsEnemyAI) 
        {
            character0.KickBallTo(character1.transform.position);
        }
        else 
        {
            character0.KickBallTo(target.transform.position);
            CharacterChangeControlManager.Instance.SetControlledCharacter(target, target.TeamSide);
        }
    }

    private void ResetPositions() 
    {
        Team team = BattleManager.Instance.Teams[teamSide];
        character0 = team.CharacterList[team.Formation.Kickoff0];
        character1 = team.CharacterList[team.Formation.Kickoff1];
        character0.transform.position = position0;
        character1.transform.position = position1[teamSide];
        PossessionManager.Instance.Release();
        PossessionManager.Instance.GiveBallToCharacter(character0);
    }
}
