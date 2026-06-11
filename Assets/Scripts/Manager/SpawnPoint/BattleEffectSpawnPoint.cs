using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

public class BattleEffectSpawnPoint : MonoBehaviour
{
    public ParticleSystem DuelStartEffect;
    public ParticleSystem DuelWinEffect;
    public ParticleSystem WingEffect;

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

        if (BattleManager.Instance != null)
            BattleManager.Instance.RegisterSpawnPointEffect(this);
    }

    private void OnDestroy()
    {
        if (BattleManager.Instance != null)
            BattleManager.Instance.UnregisterSpawnPointEffect();
    }

    public ParticleSystem GetMoveParticle(Element element) 
    {
        if (moveEffectDict.TryGetValue(element, out ParticleSystem particleSystem))
            return particleSystem;
        return null;
    }
}
