using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBallSpawnPoint : MonoBehaviour
{
    private void Awake()
    {
        if (BattleBallManager.Instance != null)
        {
            BattleBallManager.Instance.RegisterSpawnPoint(this.transform);
        }
    }

    private void OnDestroy()
    {
        if (BattleBallManager.Instance != null)
        {
            BattleBallManager.Instance.UnregisterSpawnPoint();
        }
    }
}
