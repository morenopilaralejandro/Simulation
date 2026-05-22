using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Character;

public class CharacterPortraitBattle : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageKitPortrait;

    private int _version;

    private AsyncOperationHandle<Sprite>? _characterHandle;
    private AsyncOperationHandle<Sprite>? _kitHandle;

    public async Task SetCharacterAsync(Character character)
    {
        int version = ++_version;

        ReleaseHandles();

        _characterHandle = Addressables.LoadAssetAsync<Sprite>(character.PortraitCharacterAddress);
        _kitHandle = Addressables.LoadAssetAsync<Sprite>(character.PortraitKitAddress);

        await _characterHandle.Value.Task;
        await _kitHandle.Value.Task;

        if (version != _version)
            return;

        imageCharacterPortrait.sprite =
            _characterHandle.Value.Status == AsyncOperationStatus.Succeeded
                ? _characterHandle.Value.Result
                : null;

        imageKitPortrait.sprite =
            _kitHandle.Value.Status == AsyncOperationStatus.Succeeded
                ? _kitHandle.Value.Result
                : null;
    }

    private void ReleaseHandles()
    {
        if (_characterHandle.HasValue && _characterHandle.Value.IsValid())
            Addressables.Release(_characterHandle.Value);

        if (_kitHandle.HasValue && _kitHandle.Value.IsValid())
            Addressables.Release(_kitHandle.Value);

        _characterHandle = null;
        _kitHandle = null;
    }

    public void Clear()
    {
        imageCharacterPortrait.sprite = null;
        imageKitPortrait.sprite = null;

        ReleaseHandles();
        _version++;
    }

    private void OnDestroy()
    {
        Clear();
    }
}
