using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Aremoreno.Enums.Character;

public class CharacterPortraitSpeaker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image imageCharacterPortrait;
    [SerializeField] private Image imageKitPortrait;

    private string _currentCharacterAddress;
    private string _currentKitAddress;

    private AsyncOperationHandle<Sprite>? _characterHandle;
    private AsyncOperationHandle<Sprite>? _kitHandle;

    private int _version;

    public async Task SetSpeakerAsync(Speaker speaker)
    {
        int version = ++_version;

        await UpdateCharacterPortraitAsync(speaker, version);

        if (speaker.HasKit)
            await UpdateKitPortraitAsync(speaker, version);
        else
            DisableKit();
    }

    #region Character

    private async Task UpdateCharacterPortraitAsync(Speaker speaker, int version)
    {
        /*
        if (speaker.HasKit 
        {
            address from character            
        } else 
        {
            address from npc
        }
        */

        string address =
            AddressableLoader.GetCharacterPortraitAddress(speaker.SpeakerId);

        if (address == _currentCharacterAddress && _characterHandle.HasValue)
            return;

        _currentCharacterAddress = address;

        ReleaseCharacter();

        _characterHandle = Addressables.LoadAssetAsync<Sprite>(address);
        await _characterHandle.Value.Task;

        if (version != _version)
            return;

        if (_characterHandle.Value.Status != AsyncOperationStatus.Succeeded)
            return;

        imageCharacterPortrait.sprite = _characterHandle.Value.Result;
    }

    #endregion

    #region Kit

    private async Task UpdateKitPortraitAsync(Speaker speaker, int version)
    {
        /*
        string address =
            AddressableLoader.GetKitPortraitAddress(
                speaker.KitId,
                speaker.KitVariant.ToString(),
                speaker.KitRole.ToString(),
                speaker.PortraitSize.ToString()
            );
        */

        string address = "";

        if (address == _currentKitAddress && _kitHandle.HasValue)
            return;

        _currentKitAddress = address;

        EnableKit();

        ReleaseKit();

        _kitHandle = Addressables.LoadAssetAsync<Sprite>(address);
        await _kitHandle.Value.Task;

        if (version != _version)
            return;

        if (_kitHandle.Value.Status != AsyncOperationStatus.Succeeded)
            return;

        imageKitPortrait.sprite = _kitHandle.Value.Result;
    }

    #endregion

    #region Kit Visibility

    private void EnableKit()
    {
        imageKitPortrait.enabled = true;
    }

    private void DisableKit()
    {
        imageKitPortrait.enabled = false;
        imageKitPortrait.sprite = null;
        _currentKitAddress = null;

        ReleaseKit();
    }

    #endregion

    #region Cleanup

    private void ReleaseCharacter()
    {
        if (_characterHandle.HasValue)
        {
            Addressables.Release(_characterHandle.Value);
            _characterHandle = null;
        }
    }

    private void ReleaseKit()
    {
        if (_kitHandle.HasValue)
        {
            Addressables.Release(_kitHandle.Value);
            _kitHandle = null;
        }
    }

    private void OnDestroy()
    {
        ReleaseCharacter();
        ReleaseKit();
    }

    public void Clear()
    {
        ReleaseCharacter();
        ReleaseKit();

        imageCharacterPortrait.sprite = null;
        imageKitPortrait.sprite = null;
        imageKitPortrait.enabled = false;

        _currentCharacterAddress = null;
        _currentKitAddress = null;
    }

    #endregion
}
