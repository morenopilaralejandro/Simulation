using UnityEngine;
using System.Threading.Tasks;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

public class MoveParticlePlayer : MonoBehaviour
{
    private ParticleSystem activeParticle;
    private bool isPlayingMove;

    public bool IsPlayingMove => isPlayingMove;

    public async Task Play(Element element, Vector3 position)
    {
        if (SettingsManager.Instance.IsAutoBattleEnabled) return;
        BattleManager.Instance.Freeze();
        isPlayingMove = true;
        MoveEvents.RaiseMoveCutsceneStart();

        activeParticle = BattleEffectManager.Instance.GetMoveParticle(element);
        activeParticle.transform.position = position;
        activeParticle.Play();
        AudioManager.Instance.PlaySfx("sfx-secret_" + element.ToString().ToLower());

        while (activeParticle != null && activeParticle.IsAlive(true))
            await Task.Yield();

        BattleManager.Instance.Unfreeze();
        isPlayingMove = false;
        MoveEvents.RaiseMoveCutsceneEnd();
    }

}
