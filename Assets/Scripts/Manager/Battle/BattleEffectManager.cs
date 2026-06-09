using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class BattleEffectManager : MonoBehaviour
{
    public static BattleEffectManager Instance;

    [SerializeField] private MoveParticlePlayer moveParticlePlayer;
    private WingParticlePlayer wingParticlePlayer;
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

    //register
    public void RegisterSpawnPoint(BattleEffectSpawnPoint spawner)
    {
        spawnPoint = spawner;
        wingParticlePlayer = new WingParticlePlayer(spawnPoint.WingEffect);
    }

    public void UnregisterSpawnPoint()
    {
        spawnPoint = null;
    }

    //duel start
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

    //duel wing
    public void PlayDuelWinEffect(Transform originTransform)
    {
        var psTransform = spawnPoint.DuelWinEffect.transform;

        psTransform.position = originTransform.position;
        psTransform.rotation = originTransform.rotation;

        spawnPoint.DuelWinEffect.Play(true);
    }


    //move
    public ParticleSystem GetMoveParticle(Element element) => spawnPoint.GetMoveParticle(element);
    public async Task PlayMoveParticle(Move move, Vector3 position) => await moveParticlePlayer.Play(move, position);
    public bool IsPlayingMove => moveParticlePlayer.IsPlayingMove;

    //wing
    public async Task PlayWingParticle(Wing wing, Vector3 position) => await wingParticlePlayer.Play(wing, position);
    public bool IsPlayingWing => wingParticlePlayer.IsPlaying;

}
