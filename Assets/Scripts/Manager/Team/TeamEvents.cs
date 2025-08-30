using System;
using Simulation.Enums.Character;

public static class TeamEvents
{
    public static event Action<Character, Team, int, FormationCoord> OnAssignToTeam;
    public static void RaiseAssignToTeam(Character character, Team team, int teamIndex, FormationCoord formationCoord)
    {
        OnAssignToTeam?.Invoke(character, team, teamIndex, formationCoord);
    }

    public static event Action<Character, Team, int, FormationCoord, ControlType, bool> OnAssignToTeamBattle;
    public static void RaiseAssignToTeamBattle(
        Character character, 
        Team team, 
        int teamIndex, 
        FormationCoord formationCoord, 
        ControlType controlType,
        bool isKeeper)
    {
        OnAssignToTeamBattle?.Invoke(character, team, teamIndex, formationCoord, controlType, isKeeper);
    }

}
