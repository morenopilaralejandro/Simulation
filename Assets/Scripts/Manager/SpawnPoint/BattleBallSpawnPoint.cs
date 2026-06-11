using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBallSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.RegisterSpawnPointBall(this.transform);
        }
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.UnregisterSpawnPointBall();
        }
    }
}
