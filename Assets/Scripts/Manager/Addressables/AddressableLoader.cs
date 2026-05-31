using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

/// <summary>
/// Addressable loader with LRU cache + reference counting for shared assets.
/// </summary>
public static class AddressableLoader
{
    private static LruCache<string, Object> _cache;
    private static readonly Dictionary<string, RefCountedAsset> _assets = new();

    public static async Task<T> LoadAsync<T>(string address) where T : Object
    {
        _cache ??= new LruCache<string, Object>(AddressableConfig.MaxCacheSize);

        if (string.IsNullOrEmpty(address))
        {
            LogManager.Error("[AddressableLoader] Address is null or empty");
            return null;
        }

        // Cached
        if (_assets.TryGetValue(address, out var refAsset))
        {
            refAsset.RefCount++;
            if (_cache.TryGet(address, out var cached)) return cached as T;
            return refAsset.Handle.Result as T;
        }

        AsyncOperationHandle<T> handle;

        try
        {
            handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;
        }
        catch (System.Exception ex)
        {
            LogManager.Error($"[AddressableLoader] Exception loading address [{address}] \n{ex}");
            return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[AddressableLoader] Failed to load asset at address: {address}");
            return null;
        }

        _assets[address] = new RefCountedAsset
        {
            Handle = handle,
            RefCount = 1
        };

        _cache.Add(address, handle.Result);

        return handle.Result;
    }

    public static async Task<T> LoadOptionalAsync<T>(string address) where T : Object
    {
        _cache ??= new LruCache<string, Object>(AddressableConfig.MaxCacheSize);

        if (string.IsNullOrEmpty(address)) return null;

        // Cached
        if (_assets.TryGetValue(address, out var refAsset))
        {
            refAsset.RefCount++;

            if (_cache.TryGet(address, out var cached)) return cached as T;

            return refAsset.Handle.Result as T;
        }

        // Prevent InvalidKeyException spam
        var locationsHandle = Addressables.LoadResourceLocationsAsync(address);

        await locationsHandle.Task;

        bool valid =
            locationsHandle.Status == AsyncOperationStatus.Succeeded &&
            locationsHandle.Result != null &&
            locationsHandle.Result.Count > 0;

        Addressables.Release(locationsHandle);

        if (!valid) return null;

        AsyncOperationHandle<T> handle;

        try
        {
            handle = Addressables.LoadAssetAsync<T>(address);
            await handle.Task;
        }
        catch (System.Exception ex)
        {
            LogManager.Error($"[AddressableLoader] Exception loading optional asset [{address}] \n{ex}");

            return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
            return null;

        _assets[address] = new RefCountedAsset
        {
            Handle = handle,
            RefCount = 1
        };

        _cache.Add(address, handle.Result);

        return handle.Result;
    }

    public static void Release(string address)
    {
        if (_assets.TryGetValue(address, out var refAsset))
        {
            refAsset.RefCount--;
            if (refAsset.RefCount <= 0)
            {
                Addressables.Release(refAsset.Handle);
                _assets.Remove(address);
                //_cache.Clear(); Optional, or selectively remove
            }
        }
    }

    public static void ReleaseAll()
    {
        foreach (var kvp in _assets.Values)
        {
            Addressables.Release(kvp.Handle);
        }

        _assets.Clear();
        _cache.Clear();
    }

    public static void PrintRefCounts()
    {
        foreach (var kvp in _assets)
            Debug.Log($"{kvp.Key} => {kvp.Value.RefCount}");
    }

    // Address
    public static string GetCharacterHeadAddress(string id) =>
        $"{AddressableConfig.CharacterHeadPath}{AddressableConfig.PathSeparator}{id}";
    public static string GetCharacterBodyAddress(string tone) =>
        $"{AddressableConfig.CharacterBodyPath}{AddressableConfig.PathSeparator}{tone}";
    public static string GetCharacterPortraitAddress(string id) =>
        $"{AddressableConfig.CharacterPortraitPath}{AddressableConfig.PathSeparator}{id}";
    public static string GetCharacterHairFrontAddress(string id) =>
        $"{AddressableConfig.CharacterHairFrontPath}{AddressableConfig.PathSeparator}{id}{AddressableConfig.PathSeparator}front";
    public static string GetCharacterHairBackAddress(string id) =>
        $"{AddressableConfig.CharacterHairBackPath}{AddressableConfig.PathSeparator}{id}{AddressableConfig.PathSeparator}back";
    public static string GetCharacterHairWorldAddress(string id) =>
        $"{AddressableConfig.CharacterHairWorldPath}{AddressableConfig.PathSeparator}{id}";

    public static string GetKitBodyAddress(string kitId, string variant, string role) =>
        $"{AddressableConfig.KitBodyPath}{AddressableConfig.PathSeparator}{kitId}{AddressableConfig.PathSeparator}{variant}{AddressableConfig.PathSeparator}{role}";

    public static string GetKitPortraitAddress(
        string kitId,
        Variant variant,
        Role role,
        PortraitSize size)
    {
        int portraitIndex = GetPortraitIndex(
            variant,
            role,
            size);

        string baseAddress = string.Concat(
            AddressableConfig.KitPortraitPath,
            AddressableConfig.PathSeparator,
            kitId);

        return string.Concat(
            baseAddress,
            "[",
            baseAddress,
            AddressableConfig.SubSeparator,
            portraitIndex.ToString(),
            "]"
        );
    }

    public static int GetPortraitIndex(Variant variant, Role role, PortraitSize size)
    {
        return ((int)variant * 14) +
               ((int)role * 7) +
               (int)size;
    }

    /*
    public static string GetTeamEmblemAddress(string id) =>
        $"{AddressableConfig.TeamEmblemPath}{AddressableConfig.PathSeparator}{id}";
    */

    public static string GetTeamEmblemAddress(string id) => $"{id}";

    public static string GetMoveEvolutionAddress(string id, string languageCode, string localizationSyle) =>
        $"{AddressableConfig.MoveEvolutionPath}{AddressableConfig.PathSeparator}{id}{AddressableConfig.PathSeparator}{languageCode}{AddressableConfig.PathSeparator}{localizationSyle}";

    public static string GetItemIconAddress(string id) =>
        $"{AddressableConfig.ItemIconPath}{AddressableConfig.PathSeparator}{id}";

    public static string GetNpcPortraitAddress(string id) =>
        $"{AddressableConfig.NpcPortraitPath}{AddressableConfig.PathSeparator}{id}";
}
