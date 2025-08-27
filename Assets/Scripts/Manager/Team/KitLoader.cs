using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Simulation.Enums.Character;
using Simulation.Enums.Kit;

public static class KitLoader
{
    /// <summary>
    /// Loads Kit sprites using Addressables.
    /// </summary>
    /// <param name="kitId">The kit it (e.g. "Red").</param>
    /// <param name="role">The role (Field or Keeper).</param>
    /// <param name="variant">The variant (Home or Away).</param>
    /// <param name="size">The portrait size (XS, S, SM, etc.).</param>
    /// <param name="onLoaded">Callback when the prefab is loaded.</param>
    /// <param name="onFailed">Callback if loading fails.</param>
    public static void GetKitPortrait(
        string kitId, 
        Role role, 
        Variant variant, 
        PortraitSize size,
        Action<Sprite> onLoaded,
        Action onFailed = null)
    {
        string address = $"Kit_{kitId}_{role}_{variant}_{size}";

        Addressables.LoadAssetAsync<Sprite>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"[KitLoader] GetKitPortrait: failed to load asset at address: {address}");
                onFailed?.Invoke();
            }
        };
    }
    /*
        KitLoader.GetKitPortrait("Red", Role.Field, Variant.Home, PortraitSize.XL, sprite =>
        {
            portraitImage.sprite = sprite;
            LogManager.Debug("Loaded Kit Sprite: " + sprite.name);
        },
        () =>
        {
            LogManager.Error("Kit Portrait not found");
        });
    */

    public static void GetKitBody(
        string kitId, 
        Role role, 
        Variant variant, 
        Action<Sprite> onLoaded,
        Action onFailed = null)
    {
        string address = $"Kit_{kitId}_{role}_{variant}";

        Addressables.LoadAssetAsync<Sprite>(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(handle.Result);
            }
            else
            {
                Debug.LogError($"[KitLoader] GetKitBody: failed to load asset at address: {address}");
                onFailed?.Invoke();
            }
        };
    }

}
