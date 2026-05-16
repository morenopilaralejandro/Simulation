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

    public async Task SetCharacterAsync(Character character)
    {
        int version = ++_version;

        string characterAddress =
            AddressableLoader.GetCharacterPortraitAddress(character.CharacterId);

        string kitAddress =
            AddressableLoader.GetKitPortraitAddress(
                character.KitId,
                character.KitVariant,
                character.KitRole,
                character.PortraitSize
            );

        var characterTask = LoadSpriteAsync(characterAddress);
        var kitTask = LoadSpriteAsync(kitAddress);

        Sprite characterSprite = await characterTask;
        Sprite kitSprite = await kitTask;

        // discard stale results
        if (version != _version)
            return;

        imageCharacterPortrait.sprite = characterSprite;
        imageKitPortrait.sprite = kitSprite;
    }

    private async Task<Sprite> LoadSpriteAsync(string address)
    {
        if (string.IsNullOrEmpty(address))
            return null;

        var handle = Addressables.LoadAssetAsync<Sprite>(address);

        try
        {
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
                return null;

            return handle.Result;
        }
        finally
        {
            // release immediately after fetch to avoid leaks
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }

    public void Clear()
    {
        imageCharacterPortrait.sprite = null;
        imageKitPortrait.sprite = null;

        _version++;
    }

    private void OnDestroy()
    {
        Clear();
    }
}
