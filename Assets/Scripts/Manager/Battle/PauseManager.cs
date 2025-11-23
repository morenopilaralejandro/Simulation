using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.Input;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [SerializeField] private bool isPaused;
    [SerializeField] private bool isResumeReady;
    private Dictionary<TeamSide, bool> isTeamReady;
    private float actionTimer = 10f;


    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        isTeamReady = new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
    }

    private void OnDestroy()
    {

    }

    public bool CanPause()
    {
        return BattleManager.Instance.CurrentPhase == BattlePhase.Battle &&
            !BattleManager.Instance.IsTimeFrozen 
            && !isPaused;
    }

    public void StartPause() 
    {
        if (isPaused) return;

        BattleManager.Instance.Freeze();
        ResetReady();
        BattleEvents.RaisePauseBattle();
        //BattleManager.Instance.SetBattlePhase(BattlePhase.Deadball);
        //AudioManager.Instance.PlaySfx("sfx-whistle_single");

        //if (BattleManager.Instance.IsMultiplayer)
            //StartCoroutine(ActionTimerRoutine());

    }

    private void ResetReady() 
    {
        isTeamReady = new Dictionary<TeamSide, bool>
        {
            { TeamSide.Home, false },
            { TeamSide.Away, false }
        };
        isResumeReady = false;
        isPaused = true;
    }

    public void SetTeamReady(TeamSide teamSide)
    {
        isTeamReady[teamSide] = true;
        TryResolve();
    }

    private void TryResolve()
    {
        bool isMultiplayer = false;
        if (isMultiplayer) 
        {
            isResumeReady = 
                isTeamReady[TeamSide.Home] && 
                isTeamReady[TeamSide.Away];
        } 
        else 
        {
            isResumeReady = isTeamReady[TeamSide.Home];
        }

        if (isResumeReady) 
            Resume();
    }

    private void Resume()
    {
        AudioManager.Instance.PlaySfx("sfx-menu_tap");
        //BattleManager.Instance.SetBattlePhase(BattlePhase.Battle);
        BattleManager.Instance.Unfreeze();
        BattleEvents.RaiseResumeBattle();
        isPaused = false;
    }

    private IEnumerator ActionTimerRoutine()
    {
        while (actionTimer > 0f)
        {
            actionTimer -= Time.deltaTime;
            if (isResumeReady) break;
            yield return null;
        }
        
        SetTeamReady(BattleManager.Instance.GetUserSide());
    }
}
