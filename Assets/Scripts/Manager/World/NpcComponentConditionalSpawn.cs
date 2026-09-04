using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.World;

public class NpcComponentConditionalSpawn : MonoBehaviour
{
    [SerializeField] private ConditionalMode conditionMode = ConditionalMode.All;
    [SerializeField] private List<ConditionalSpawn> conditions = new();

    private void Start()
    {
        gameObject.SetActive(ResolveSpawn());
    }

    private bool ResolveSpawn()
    {
        // No conditions = spawn normally
        if (conditions == null || conditions.Count == 0) return true;

        switch (conditionMode)
        {
            case ConditionalMode.Any:
                return ResolveAny();

            case ConditionalMode.All:
                return ResolveAll();

            default:
                return true;
        }
    }

    private bool ResolveAny()
    {
        foreach (var condition in conditions)
        {
            if (string.IsNullOrEmpty(condition.flagName))
                continue;

            bool currentValue =
                StorySystemManager.Instance.GetFlag(condition.flagName);

            if (currentValue == condition.flagValue)
            {
                return true;
            }
        }

        return false;
    }

    private bool ResolveAll()
    {
        foreach (var condition in conditions)
        {
            if (string.IsNullOrEmpty(condition.flagName))
                continue;

            bool currentValue =
                StorySystemManager.Instance.GetFlag(condition.flagName);

            if (currentValue != condition.flagValue)
            {
                return false;
            }
        }

        return true;
    }

}
