using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;

public class OffsideManager : MonoBehaviour
{
    public static OffsideManager Instance;

    private OffsideSnapshot snapshot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        snapshot = new OffsideSnapshot();
    }

    public void TakeSnapshot(Character passer)
    {
        if (snapshot.isActive) return;
        if (passer == null || IsDeadBallPlay()) return;

        Team attacking = passer.GetTeam();
        Team defending = passer.GetOpponentTeam();

        bool attacksPositiveZ = attacking.TeamSide == TeamSide.Home;
        float ballZ = BattleManager.Instance.Ball.transform.position.z;
        float offsideLineZ = GetSecondLastDefenderZ(defending, attacksPositiveZ);

        if ((attacksPositiveZ && ballZ >= offsideLineZ) ||
            (!attacksPositiveZ && ballZ <= offsideLineZ))
        {
            snapshot.isActive = false;
            snapshot.offsideCandidates.Clear();
            LogManager.Trace("[OffsideManager] [TakeSnapshot] Ball beyond offside line. Offside disabled for this pass");
            return;
        }

        snapshot.attackingTeam = attacking;
        snapshot.attacksPositiveZ = attacksPositiveZ;
        snapshot.ballZ = ballZ;
        snapshot.offsideLineZ = offsideLineZ;
        snapshot.isActive = true;
        snapshot.offsideCandidates.Clear();
        
        PremarkOffsideCharacters(attacking.CharacterList, attacksPositiveZ, ballZ, offsideLineZ, passer);

        LogManager.Trace(
            $"[OffsideManager] [TakeSnapshot] Snapshot taken. AttackingTeam={attacking.TeamSide}, " +
            $"AttacksPositiveZ={attacksPositiveZ}, BallZ={ballZ}, OffsideLineZ={offsideLineZ}"
        );
    }

    private void PremarkOffsideCharacters(
        List<Character> attackers, 
        bool attacksPositiveZ,
        float ballZ,
        float offsideLineZ,
        Character passer)
    {
        foreach (var attacker in attackers)
        {
            if (attacker == passer) continue;
            float z = attacker.transform.position.z;

            bool offside = attacksPositiveZ
                ? z > ballZ && z > offsideLineZ
                : z < ballZ && z < offsideLineZ;

            if (offside)
            {
                snapshot.offsideCandidates.Add(attacker);
                LogManager.Trace(
                    $"[OffsideManager] [PremarkOffsideCharacters] Pre marked as offside: {attacker.CharacterId} Z={z}"
                );
            }
        }
    }

    private bool AttacksPositiveZ(Team team)
    {
        return team.TeamSide == TeamSide.Home;
    }

    private float GetSecondLastDefenderZ(Team defending, bool attacksPositiveZ)
    {
        float last = attacksPositiveZ ? float.MinValue : float.MaxValue;
        float secondLast = last;

        foreach (var defender in defending.CharacterList)
        {
            if (defender.IsStunned()) continue;

            float z = defender.transform.position.z;

            LogManager.Trace(
                $"[OffsideManager] Defender={defender.CharacterId}, " +
                $"Team={defender.GetTeam().TeamSide}, Z={z}, Active={defender.gameObject.activeInHierarchy}"
            );

            if (attacksPositiveZ)
            {
                if (z >= last)
                {
                    secondLast = last;
                    last = z;
                }
                else if (z > secondLast)
                {
                    secondLast = z;
                }
            }
            else
            {
                if (z <= last)
                {
                    secondLast = last;
                    last = z;
                }
                else if (z < secondLast)
                {
                    secondLast = z;
                }
            }
        }

        LogManager.Trace($"[OffsideManager] [GetSecondLastDefenderZ] Calculated z: {secondLast}");

        return secondLast;
    }

    private bool IsOffside(Character attacker)
    {
        return snapshot.isActive &&
               attacker.GetTeam() == snapshot.attackingTeam &&
               snapshot.offsideCandidates.Contains(attacker);
    }

    // Called whenever ANY character gains possession
    public void OnBallTouched(Character toucher)
    {
        if (!snapshot.isActive) return;

        // Opponent touch cancels offside
        if (toucher.GetTeam() != snapshot.attackingTeam)
        {
            snapshot.isActive = false;
            snapshot.offsideCandidates.Clear();
            LogManager.Trace("[OffsideManager] [OnBallTouched] Snapshot cancelled by opponent touch");
            return;
        }

        if (IsOffside(toucher))
        {
            LogManager.Trace($"[OffsideManager] [OnBallTouched] Offside by {toucher.CharacterId}");
            CallOffside(toucher);
            return;
        }

        if (BattleManager.Instance.Ball.IsTraveling) return;

        snapshot.isActive = false;
        snapshot.offsideCandidates.Clear();
        LogManager.Trace("[OffsideManager] [OnBallTouched] Legal touch.");
    }

    private void CallOffside(Character offender)
    {
        BattleManager.Instance.Ball.CancelTravel();
        BattleManager.Instance.Freeze();
        snapshot.isActive = false;
        snapshot.offsideCandidates.Clear();
        DeadBallManager.Instance.SetBallPosition(offender.transform.position);
        BattleManager.Instance.StartOffside(offender.GetOpponentSide());
    }

    private bool IsDeadBallPlay()
    {
        var db = DeadBallManager.Instance;
        return db != null && db.IsDeadBallInProgress && 
               (db.DeadBallType == DeadBallType.ThrowIn ||
                db.DeadBallType == DeadBallType.CornerKick ||
                db.DeadBallType == DeadBallType.GoalKick);
                //db.DeadBallType == DeadBallType.Kickoff
    }
}
