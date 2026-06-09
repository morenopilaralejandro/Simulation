using UnityEngine;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingParticlePlayer
{
    private ParticleSystem wingParticleSystem;
    private bool isPlaying;

    public bool IsPlaying => isPlaying;

    public WingParticlePlayer(ParticleSystem particleSystem) 
    {
        this.wingParticleSystem = particleSystem;
    }

    public async Task Play(Wing wing, Vector3 position)
    {
        if (SettingsManager.Instance.IsAutoBattleEnabled) return;

        WingEvents.RaiseWingCutsceneStart(wing);
        //Element element = move.Element;
        BattleManager.Instance.Freeze();
        isPlaying = true;

        var main = wingParticleSystem.main;
        main.startColor = ColorManager.GetWingColor(wing.WingColorType);
        wingParticleSystem.transform.position = position;
        wingParticleSystem.Play();
        AudioManager.Instance.PlaySfx("sfx-dimension_wings_particles");

        while (wingParticleSystem != null && wingParticleSystem.IsAlive(true))
            await Task.Yield();

        BattleManager.Instance.Unfreeze();
        isPlaying = false;
        WingEvents.RaiseWingCutsceneEnd();
    }

}
