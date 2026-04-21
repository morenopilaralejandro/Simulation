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

    public CharacterEntityBattle GetKicker(Team team)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (teammate.IsKeeper) continue;

            float dist = Vector3.Distance(
                manager.CachedBallPosition,
                teammate.transform.position);

            if (dist < closest)
            {
                closest = dist;
                nearest = teammate;
            }
        }

        return nearest;
    }

    public CharacterEntityBattle GetKickerIndirectFreeKick(Team team)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in team.GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            if (teammate.IsKeeper) continue;

            float dist = Vector3.Distance(
                manager.CachedBallPosition,
                teammate.FormationCoord.DefaultPosition);

            if (dist < closest)
            {
                closest = dist;
                nearest = teammate;
            }
        }

        return nearest;
    }

    public CharacterEntityBattle GetClosestTeammate(CharacterEntityBattle character)
    {
        CharacterEntityBattle nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in character.GetTeam().GetCharacterEntities(BattleManager.Instance.CurrentType))
        {
            //if (teammate.IsKeeper) continue;

            float dist = Vector3.Distance(
                character.transform.position,
                teammate.transform.position);

            if (dist < closest)
            {
                closest = dist;
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
        CharacterEntityBattle[] closest = new CharacterEntityBattle[count];
        float[] closestDistances = new float[count];

        for (int i = 0; i < count; i++)
            closestDistances[i] = float.MaxValue;

        Vector3 ballPos = manager.CachedBallPosition;

        List<CharacterEntityBattle> characters = team.GetCharacterEntities(BattleManager.Instance.CurrentType);
        int total = characters.Count;

        for (int i = 0; i < total; i++)
        {
            CharacterEntityBattle c = characters[i];

            if (c == kicker || c.IsKeeper)
                continue;

            Vector3 diff = c.transform.position - ballPos;
            float sqrDist = diff.sqrMagnitude;

            for (int j = 0; j < count; j++)
            {
                if (sqrDist < closestDistances[j])
                {
                    // Shift entries down
                    for (int k = count - 1; k > j; k--)
                    {
                        closestDistances[k] = closestDistances[k - 1];
                        closest[k] = closest[k - 1];
                    }

                    closestDistances[j] = sqrDist;
                    closest[j] = c;
                    break;
                }
            }
        }

        return closest;
    }

    public int GetDefaultReceiverIndex(CharacterEntityBattle[] receivers, CharacterEntityBattle kicker)
    {
        return kicker.IsEnemyAI ? receivers.Length - 1 : 0;
    }

    public int GetKickoffReceiverIndex(int kickerIndex, int receiverIndex)
    {
        int baseValue = Mathf.Max(kickerIndex, receiverIndex);
        int result;
        int randomOffset;

        if(BattleManager.Instance.CurrentType == BattleType.Full) 
        {
            randomOffset = Random.Range(1, 5);
        } else 
        {
            randomOffset = Random.Range(1, 2);
        }

        result = baseValue - randomOffset;

        if (result == kickerIndex)
            result = receiverIndex;

        return result;
    }
}
