using System;

public static class TeamEvents
{
    public static event Action<Character, Team, int, FormationCoord> OnAssignToTeam;

    public static void RaiseAssignToTeam(Character character, Team team, int teamIndex, FormationCoord formationCoord)
    {
        OnAssignToTeam?.Invoke(character, team, teamIndex, formationCoord);
    }

    /*
        OnAssignToTeamBattle
Character
Team
TeamIndex
FormationCoord
ControlType
IsKeeper
    */
}
