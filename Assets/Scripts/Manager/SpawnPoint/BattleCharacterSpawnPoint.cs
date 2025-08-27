using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleCharacterSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        if (BattleCharacterManager.Instance != null)
        {
            BattleCharacterManager.Instance.RegisterSpawnPoint(this.transform);
        }
    }

    private void OnDestroy()
    {
        if (BattleCharacterManager.Instance != null)
        {
            BattleCharacterManager.Instance.UnregisterSpawnPoint();
        }
    }
}
