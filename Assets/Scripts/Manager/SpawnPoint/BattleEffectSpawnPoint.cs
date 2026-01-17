using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class BattleEffectSpawnPoint : MonoBehaviour
{
    public ParticleSystem DuelStartEffect;
    public ParticleSystem DuelWinEffect;

    [SerializeField] private ParticleSystem MoveFireEffect;
    [SerializeField] private ParticleSystem MoveIceEffect;
    [SerializeField] private ParticleSystem MoveHolyEffect;
    [SerializeField] private ParticleSystem MoveEvilEffect;
    [SerializeField] private ParticleSystem MoveAirEffect;
    [SerializeField] private ParticleSystem MoveForestEffect;
    [SerializeField] private ParticleSystem MoveEarthEffect;
    [SerializeField] private ParticleSystem MoveElectricEffect;
    [SerializeField] private ParticleSystem MoveWaterEffect;

    private Dictionary<Element, ParticleSystem> moveEffectDict;

    private void Awake()
    {
        moveEffectDict = new Dictionary<Element, ParticleSystem>()
        {
            { Element.Fire,     MoveFireEffect },
            { Element.Ice,      MoveIceEffect },
            { Element.Holy,     MoveHolyEffect },
            { Element.Evil,     MoveEvilEffect },
            { Element.Air,      MoveAirEffect },
            { Element.Forest,   MoveForestEffect },
            { Element.Earth,    MoveEarthEffect },
            { Element.Electric, MoveElectricEffect },
            { Element.Water,    MoveWaterEffect }
        };

        if (BattleEffectManager.Instance != null)
            BattleEffectManager.Instance.RegisterSpawnPoint(this);
    }

    private void OnDestroy()
    {
        if (BattleEffectManager.Instance != null)
            BattleEffectManager.Instance.UnregisterSpawnPoint();
    }

    public ParticleSystem GetMoveParticle(Element element) 
    {
        if (moveEffectDict.TryGetValue(element, out ParticleSystem particleSystem))
            return particleSystem;
        return null;
    }
}
