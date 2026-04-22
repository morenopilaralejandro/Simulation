using System;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public static class TeamEvents
{
    public static event Action<Team, TeamSide> OnAssignTeamToSide;
    public static void RaiseAssignTeamToSide(Team team, TeamSide teamSide)
    {
        OnAssignTeamToSide?.Invoke(team, teamSide);
    }

    public static event Action<Team, Variant> OnAssignVariantToTeam;
    public static void RaiseAssignVariantToTeam(Team team, Variant variant)
    {
        OnAssignVariantToTeam?.Invoke(team, variant);
    }

    public static event Action<Character, Team, FormationCoord> OnAssignCharacterToTeam;
    public static void RaiseAssignCharacterToTeam(Character character, Team team, FormationCoord formationCoord)
    {
        OnAssignCharacterToTeam?.Invoke(character, team, formationCoord);
    }

    public static event Action<CharacterEntityBattle, Team, FormationCoord> OnAssignCharacterToTeamBattle;
    public static void RaiseAssignCharacterToTeamBattle(
        CharacterEntityBattle characterEntityBattle, 
        Team team,
        FormationCoord formationCoord)
    {
        OnAssignCharacterToTeamBattle?.Invoke(characterEntityBattle, team, formationCoord);
    }

    public static event Action<Team, Formation> OnFormationChanged;
    public static void RaiseFormationChanged(Team team, Formation formation)
    {
        OnFormationChanged?.Invoke(team, formation);
    }

    public static event Action<Team, Kit> OnKitChanged;
    public static void RaiseKitChanged(Team team, Kit kit)
    {
        OnKitChanged?.Invoke(team, kit);
    }

    public static event Action<Team> OnLoadoutCreated;
    public static void RaiseLoadoutCreated(Team team)
    {
        OnLoadoutCreated?.Invoke(team);
    }

    public static event Action<Team> OnLoadoutDeleted;
    public static void RaiseLoadoutDeleted(Team team)
    {
        OnLoadoutDeleted?.Invoke(team);
    }

    public static event Action<Team> OnLoadoutUpdated;
    public static void RaiseLoadoutUpdated(Team team)
    {
        OnLoadoutUpdated?.Invoke(team);
    }

    public static event Action<Team> OnActiveLoadoutChanged;
    public static void RaiseActiveLoadoutChanged(Team team)
    {
        OnActiveLoadoutChanged?.Invoke(team);
    }

    public static event Action<Team> OnTeamCrestSpriteUpdated;
    public static void RaiseTeamCrestSpriteUpdated(Team team)
    {
        OnTeamCrestSpriteUpdated?.Invoke(team);
    }


    public static event Action<TeamSide, int> OnSubstitutionMade;
    public static void RaiseSubstitutionMade(TeamSide teamSide, int intValue)
    {
        OnSubstitutionMade?.Invoke(teamSide, intValue);
    }

    public static event Action<TeamSide> OnSubstitutionDenied;
    public static void RaiseSubstitutionDenied(TeamSide teamSide)
    {
        OnSubstitutionDenied?.Invoke(teamSide);
    }

    public static event Action<TeamSide> OnSubstitutionResetPositions;
    public static void RaiseSubstitutionResetPositions(TeamSide teamSide)
    {
        OnSubstitutionResetPositions?.Invoke(teamSide);
    }

    public static event Action<Character, Character, TeamSide> OnCharacterSubstituted;
    public static void RaiseCharacterSubstituted(Character characterIn, Character characterOut, TeamSide teamSide)
    {
        OnCharacterSubstituted?.Invoke(characterIn, characterOut, teamSide);
    }
}
