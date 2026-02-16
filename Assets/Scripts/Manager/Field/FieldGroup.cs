using UnityEngine;
using System.Collections.Generic;
using Simulation.Enums.Battle;

public class FieldGroup : MonoBehaviour
{
    [SerializeField] private GameObject group;
    [SerializeField] private BattleType battleType;

    void Awake()
    {
        if (BattleManager.Instance == null) return;
        BattleType currentType = BattleArgs.BattleType;
        group.SetActive(currentType == battleType);
    }

}
