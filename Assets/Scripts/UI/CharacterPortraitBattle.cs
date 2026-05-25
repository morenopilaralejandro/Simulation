using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPortraitBattle : MonoBehaviour
{
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageKitPortrait;

    private readonly AddressableBinding<Sprite> _characterBinding = new();
    private readonly AddressableBinding<Sprite> _kitBinding = new();

    public async Task SetCharacterAsync(Character character)
    {
        var portraitTask =
            _characterBinding.LoadAsync(character.PortraitCharacterAddress);

        var kitTask =
            _kitBinding.LoadAsync(character.PortraitKitAddress);

        imageCharacterPortrait.sprite = await portraitTask;
        imageKitPortrait.sprite = await kitTask;
    }

    public void Clear()
    {
        imageCharacterPortrait.sprite = null;
        imageKitPortrait.sprite = null;

        _characterBinding.Release();
        _kitBinding.Release();

        _characterBinding.Cancel();
        _kitBinding.Cancel();
    }

    private void OnDestroy()
    {
        Clear();
    }
}
