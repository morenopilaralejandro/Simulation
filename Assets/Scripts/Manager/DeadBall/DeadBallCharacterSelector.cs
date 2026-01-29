using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Battle;
using Simulation.Enums.Character;
using Simulation.Enums.DeadBall;
using Simulation.Enums.Input;

public class DeadBallCharacterSelector
{
    private readonly DeadBallManager manager;

    public DeadBallCharacterSelector(DeadBallManager manager)
    {
        this.manager = manager;
    }

    public Character GetKicker(Team team)
    {
        Character nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in team.CharacterList)
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

    public Character GetKickerIndirectFreeKick(Team team)
    {
        Character nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in team.CharacterList)
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

    public Character GetClosestTeammate(Character character)
    {
        Character nearest = null;
        float closest = Mathf.Infinity;

        foreach (var teammate in character.GetTeam().CharacterList)
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

    public Character[] GetClosestSupporters(
        Team team,
        Character kicker,
        int count = 3)
    {
        Character[] closest = new Character[count];
        float[] closestDistances = new float[count];

        for (int i = 0; i < count; i++)
            closestDistances[i] = float.MaxValue;

        Vector3 ballPos = manager.CachedBallPosition;

        List<Character> characters = team.CharacterList;
        int total = characters.Count;

        for (int i = 0; i < total; i++)
        {
            Character c = characters[i];

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

    public int GetDefaultReceiverIndex(Character[] receivers, Character kicker)
    {
        return kicker.IsEnemyAI ? receivers.Length - 1 : 0;
    }
}
