using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitBattle : MonoBehaviour
{
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageKitPortrait;
    [SerializeField] private CanvasGroup canvasGroup;

    private readonly AddressableBinding<Sprite> _characterBinding = new();
    private readonly AddressableBinding<Sprite> _kitBinding = new();

    private int _setVersion;

    public async Task SetCharacterAsync(Character character)
    {
        int version = ++_setVersion;

        var portraitTask = _characterBinding.LoadAsync(character.PortraitCharacterAddress);

        var kitTask = _kitBinding.LoadAsync(character.PortraitKitAddress);

        var portrait = await portraitTask;
        var kit = await kitTask;

        if (version != _setVersion) return;

        imageCharacterPortrait.sprite = portrait;
        imageKitPortrait.sprite = kit;

        SetVisible(true);
    }

    public void Clear()
    {
        SetVisible(false);

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

    public void SetVisible(bool boolValue) 
    {
        canvasGroup.alpha = boolValue ? 1f : 0f;
    }
}
