using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.DeadBall;
using Aremoreno.Enums.Input;

public class DeadBallCharacterSelector
{
    private readonly DeadBallManager manager;

    public DeadBallCharacterSelector(DeadBallManager manager)
    {
        this.manager = manager;
    }

    private bool IsAvailable(CharacterEntityBattle character)
    {
        return character != null && !character.IsFainted;
    }

    public CharacterEntityBattle GetKickoffKicker(Team team)
    {
        List<CharacterEntityBattle> characters =
            team.GetCharacterEntities(BattleManager.Instance.CurrentType);

        int kickoffIndex =
            team.GetFormation(BattleManager.Instance.CurrentType).Kickoff0;

        CharacterEntityBattle kicker = characters[kickoffIndex];

        if (IsAvailable(kicker))
            return kicker;

        // Search in reverse order.
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            CharacterEntityBattle character = characters[i];

            if (IsAvailable(character))
                return character;
        }

        return null;
    }

    public CharacterEntityBattle GetKickoffReceiver(
        Team team,
        CharacterEntityBattle kicker)
    {
        List<CharacterEntityBattle> characters =
            team.GetCharacterEntities(BattleManager.Instance.CurrentType);

        int kickoffIndex =
            team.GetFormation(BattleManager.Instance.CurrentType).Kickoff1;

        CharacterEntityBattle receiver = characters[kickoffIndex];

        if (IsAvailable(receiver) && receiver != kicker)
            return receiver;

        // Search in reverse order, excluding kicker.
        for (int i = characters.Count - 1; i >= 0; i--)
        {
            CharacterEntityBattle character = characters[i];

            if (!IsAvailable(character))
                continue;

            if (character == kicker)
                continue;

            return character;
        }

        return null;
    }

    public CharacterEntityBattle GetPassTargetAi(
        Team team,
        CharacterEntityBattle kicker)
    {
        List<CharacterEntityBattle> candidates = new List<CharacterEntityBattle>();

        foreach (CharacterEntityBattle character in
                 team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (!IsAvailable(character))
                continue;

            if (character == kicker)
                continue;

            candidates.Add(character);
        }

        if (candidates.Count == 0)
            return null;

        // Player kicker:
        // return the closest available teammate.
        if (!kicker.IsEnemyAI)
        {
            CharacterEntityBattle closest = null;
            float closestDistance = Mathf.Infinity;

            foreach (CharacterEntityBattle character in candidates)
            {
                float distance =
                    (character.transform.position -
                     kicker.transform.position).sqrMagnitude;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = character;
                }
            }

            return closest;
        }

        // AI kicker:
        // Find the 3 closest available teammates.
        candidates.Sort((a, b) =>
        {
            float distanceA =
                (a.transform.position -
                 kicker.transform.position).sqrMagnitude;

            float distanceB =
                (b.transform.position -
                 kicker.transform.position).sqrMagnitude;

            return distanceA.CompareTo(distanceB);
        });

        int targetCount = Mathf.Min(3, candidates.Count);

        return candidates[Random.Range(0, targetCount)];
    }

    public CharacterEntityBattle GetGoalKickKicker(Team team)
    {
        CharacterEntityBattle keeper =
            GoalManager.Instance.Keepers[team.TeamSide];

        if (IsAvailable(keeper))
            return keeper;

        // Keeper is fainted.
        // Search in default team order.
        foreach (CharacterEntityBattle character in
                 team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (IsAvailable(character))
                return character;
        }

        return null;
    }

    public CharacterEntityBattle GetKicker(Team team)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (CharacterEntityBattle teammate in
                 team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (teammate.IsKeeper || teammate.IsFainted)
                continue;

            float sqrDist =
                (manager.CachedBallPosition -
                 teammate.transform.position).sqrMagnitude;

            if (sqrDist < closest)
            {
                closest = sqrDist;
                nearest = teammate;
            }
        }

        return nearest;
    }

    public CharacterEntityBattle GetKickerIndirectFreeKick(Team team)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (CharacterEntityBattle teammate in
                 team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (teammate.IsKeeper || teammate.IsFainted)
                continue;

            float sqrDist =
                (manager.CachedBallPosition -
                 teammate.FormationCoord.DefaultPosition).sqrMagnitude;

            if (sqrDist < closest)
            {
                closest = sqrDist;
                nearest = teammate;
            }
        }

        return nearest;
    }

    public CharacterEntityBattle GetClosestTeammate(
        CharacterEntityBattle character)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (CharacterEntityBattle teammate in
                 character.GetTeam().GetCharacterEntities(
                     BattleManager.Instance.CurrentType))
        {
            if (teammate == character || teammate.IsFainted)
                continue;

            float sqrDist =
                (character.transform.position -
                 teammate.transform.position).sqrMagnitude;

            if (sqrDist < closest)
            {
                closest = sqrDist;
                nearest = teammate;
            }
        }

        return nearest;
    }

    public CharacterEntityBattle[] GetClosestSupporters(
        Team team,
        CharacterEntityBattle kicker,
        int count = 3)
    {
        List<CharacterEntityBattle> candidates =
            new List<CharacterEntityBattle>();

        foreach (CharacterEntityBattle character in
                 team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (!IsAvailable(character))
                continue;

            if (character == kicker)
                continue;

            if (character.IsKeeper)
                continue;

            candidates.Add(character);
        }

        candidates.Sort((a, b) =>
        {
            float distanceA =
                (a.transform.position -
                 manager.CachedBallPosition).sqrMagnitude;

            float distanceB =
                (b.transform.position -
                 manager.CachedBallPosition).sqrMagnitude;

            return distanceA.CompareTo(distanceB);
        });

        int resultCount = Mathf.Min(count, candidates.Count);

        CharacterEntityBattle[] result =
            new CharacterEntityBattle[resultCount];

        for (int i = 0; i < resultCount; i++)
            result[i] = candidates[i];

        return result;
    }
}
