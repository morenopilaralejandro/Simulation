using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class BattleManagerTeam
{
    #region Fields

    private Dictionary<TeamSide, Team> teams = new Dictionary<TeamSide, Team>();
    public Dictionary<TeamSide, Team> Teams => teams;

    #endregion

    #region Constructor

    public BattleManagerTeam()
    {

    }

    #endregion

    #region Logic

    public void AssignTeamToSide(Team team, TeamSide teamSide) {
        teams[teamSide] = team;
        team.SetSide(teamSide);
    }

    public void AssignVariantToTeam(Team team, Variant variant) {
        team.SetVariant(variant);
    }

    public void AssignVariants() 
    {
        Variant awayVariant = teams[TeamSide.Away].Kit == teams[TeamSide.Home].Kit ? Variant.Away : Variant.Home;
        AssignVariantToTeam(teams[TeamSide.Home], Variant.Home);
        AssignVariantToTeam(teams[TeamSide.Away], awayVariant);
    }
        
    public TeamSide GetUserSide()
    {
        return TeamSide.Home; //Single Player only
    }

    public void Reset()
    {
        teams.Clear();
    }

    public Team ResolveTeamForSide(TeamSide side)
    {
        string teamId = null;
        string teamGuid = null;
        
        if (side == TeamSide.Home) 
        {
            teamId = BattleArgs.HomeTeamId;
            teamGuid = BattleArgs.HomeTeamGuid;
        } else 
        {
            teamId = BattleArgs.AwayTeamId;
            teamGuid = BattleArgs.AwayTeamGuid;
        }

        bool usesLoadout = teamGuid != null;

        if (usesLoadout)
            return CreateTeamFromLoadout(teamGuid, side);
        else
            return CreateTeamFromData(teamId);
    }

    private Team CreateTeamFromLoadout(string teamGuid, TeamSide side)
    {
        // Different for each user depending on the side
        Team team = TeamManager.Instance.GetLoadout(teamGuid);
        return team;
    }

    private Team CreateTeamFromData(string teamId)
    {
        Team team = DatabaseManager.Instance.GetTeam(teamId);
        return team;
    }

    #endregion

    #region Helpers

    #endregion

    #region Events

    public void Subscribe() 
    { 
        BattleEvents.OnBattleEnd += HandleBattleEnd;
    }

    public void Unsubscribe() 
    { 
        BattleEvents.OnBattleEnd -= HandleBattleEnd;
    }

    private void HandleBattleEnd()
    {
        teams[TeamSide.Home].ResetSideAndVariant();
        teams[TeamSide.Away].ResetSideAndVariant();
    }

    #endregion

}
