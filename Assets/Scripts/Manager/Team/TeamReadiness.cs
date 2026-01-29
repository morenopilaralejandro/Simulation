using UnityEngine;
using Simulation.Enums.Character;

public class TeamReadiness
{
    private bool isHomeReady;
    private bool isAwayReady;

    public bool AreBothReady => isHomeReady && isAwayReady;

    public void SetReady(TeamSide side)
    {
        if (side == TeamSide.Home)
            isHomeReady = true;
        else
            isAwayReady = true;
    }

    public void SetUserReady()
    {
        TeamSide side = BattleManager.Instance.GetUserSide();
        SetReady(side);
    }

    public void SetBothReady()
    {
        isHomeReady = true;
        isAwayReady = true;
    }

    public void Reset()
    {
        isHomeReady = false;
        isAwayReady = false;
    }
}
