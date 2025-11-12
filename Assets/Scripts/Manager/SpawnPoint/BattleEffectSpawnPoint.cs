using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEffectSpawnPoint : MonoBehaviour
{
    public ParticleSystem DuelStartEffect;
    public ParticleSystem DuelWinEffect;

    private void Awake()
    {
        if (BattleEffectManager.Instance != null)
        {
            BattleEffectManager.Instance.RegisterSpawnPoint(this);
        }
    }

    private void OnDestroy()
    {
        if (BattleEffectManager.Instance != null)
        {
            BattleEffectManager.Instance.UnregisterSpawnPoint();
        }
    }
}
