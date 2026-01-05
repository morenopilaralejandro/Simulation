using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BattleEffectManager : MonoBehaviour
{
    public static BattleEffectManager Instance;

    private BattleEffectSpawnPoint spawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void RegisterSpawnPoint(BattleEffectSpawnPoint spawner)
    {
        spawnPoint = spawner;
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    public void PlayDuelStartEffect(Transform originTransform)
    {
        spawnPoint.DuelStartEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        spawnPoint.DuelStartEffect.transform.position = originTransform.position;
        spawnPoint.DuelStartEffect.Play();
    }

    public void StopDuelStartEffect()
    {
        spawnPoint.DuelStartEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void PlayDuelWinEffect(Transform originTransform)
    {
        var psTransform = spawnPoint.DuelWinEffect.transform;

        psTransform.position = originTransform.position;
        psTransform.rotation = originTransform.rotation;

        spawnPoint.DuelWinEffect.Play(true);
    }

}
