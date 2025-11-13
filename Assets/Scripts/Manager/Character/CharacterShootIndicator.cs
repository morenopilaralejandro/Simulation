using UnityEngine;
using Simulation.Enums.Battle;

public class CharacterShootIndicator : MonoBehaviour
{
    [SerializeField] private ParticleSystem auraEffect;
    [SerializeField] private Color colorRegular;
    [SerializeField] private Color colorLong;

    private Character character;

    private void Awake() 
    {
        var emission = auraEffect.emission;
        emission.enabled = false;
    }

    private void OnEnable() 
    {
        BallEvents.OnGained += HandleOnGained;
        BallEvents.OnReleased += HandleOnReleased;
    }

    private void OnDisable() 
    {
        BallEvents.OnGained -= HandleOnGained;
        BallEvents.OnReleased -= HandleOnReleased;
    }

    private void Update()
    {
        if (character == null || 
            BattleManager.Instance.IsTimeFrozen) 
            return;

        if (character.CanShoot())
        {
            auraEffect.transform.SetParent(character.transform);
            auraEffect.transform.localPosition = Vector3.zero;

            var main = auraEffect.main;
            var emission = auraEffect.emission;
            emission.enabled = true;
            
            if (GoalManager.Instance.IsInShootDistance(character))
            {
                main.startColor = colorRegular;
                AudioManager.Instance.PlaySfxLoop("sfx-shoot_aura_regular");
            } else {
                main.startColor = colorLong;
                AudioManager.Instance.PlaySfxLoop("sfx-shoot_aura_long");
            }

            if (!auraEffect.isPlaying) 
                auraEffect.Play();
            return;
        }

        HideAura();

    }

    private void HideAura() 
    {
        var emission = auraEffect.emission;
        emission.enabled = false;
        auraEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        AudioManager.Instance.StopSfxLoop("sfx-shoot_aura_regular");
        AudioManager.Instance.StopSfxLoop("sfx-shoot_aura_long");
    }

    private void HandleOnGained(Character currentCharacter) 
    {
        character = currentCharacter;
    }

    private void HandleOnReleased(Character lastCharacter) 
    {
        if (lastCharacter != character) return;

        character = null;
        HideAura();
    }

}
