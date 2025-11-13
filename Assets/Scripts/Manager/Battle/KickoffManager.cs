using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using UnityEngine.Localization;
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

        position1 =
        new Dictionary<TeamSide, Vector3>
        {
            { TeamSide.Home, new Vector3(-1f, 0.34f, -0.1f) },
            { TeamSide.Away, new Vector3(1f, 0.34f, 0.1f) }
        };

        isTeamReady =
        new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
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
    }

    private void ResetReady() 
    {
        isTeamReady =
        new Dictionary<TeamSide, bool>
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
        } else 
        {
            isKickoffReady =  isTeamReady[TeamSide.Home];
        }

        if (isKickoffReady) 
            PerformKickOff();
    }

    private void PerformKickOff()
    {
        BattleManager.Instance.SetBattlePhase(BattlePhase.Battle);
        BattleManager.Instance.Unfreeze();
        if(character0.IsEnemyAI)
            character0.KickBallTo(character1.transform.position);
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
