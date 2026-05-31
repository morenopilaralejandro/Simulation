using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitSpeaker : MonoBehaviour
{
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageKitPortrait;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup canvasGroupKit;

    private readonly AddressableBinding<Sprite> _characterBinding = new();
    private readonly AddressableBinding<Sprite> _kitBinding = new();

    private int _setVersion;

    public async Task SetSpeakerAsync(Speaker speaker)
    {
        int version = ++_setVersion;
        if (speaker.HasKit) 
        {
            var portraitTask = _characterBinding.LoadAsync(speaker.PortraitCharacterAddress);
            var kitTask = _kitBinding.LoadAsync(speaker.PortraitKitAddress);

            var portrait = await portraitTask;
            var kit = await kitTask;

            if (version != _setVersion) return;

            imageCharacterPortrait.sprite = portrait;
            imageKitPortrait.sprite = kit;
            SetVisible(true, canvasGroupKit);
        } else 
        {
            var portraitTask = _characterBinding.LoadAsync(speaker.PortraitCharacterAddress);
            var portrait = await portraitTask;

            if (version != _setVersion) return;

            imageCharacterPortrait.sprite = portrait;

            SetVisible(false, canvasGroupKit);
        }
        SetVisible(true, canvasGroup);
    }

    public void Clear()
    {
        SetVisible(false, canvasGroup);

        imageCharacterPortrait.sprite = null;
        imageKitPortrait.sprite = null;

        _characterBinding.Release();
        _kitBinding.Release();

        _characterBinding.Cancel();
        _kitBinding.Cancel();

        _setVersion++;
    }

    private void OnDestroy()
    {
        Clear();
    }

    public void SetVisible(bool boolValue, CanvasGroup canvas) 
    {
        canvas.alpha = boolValue ? 1f : 0f;
    }
}
