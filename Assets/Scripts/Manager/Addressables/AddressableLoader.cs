using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        // If cached, increment reference and return.
        if (_assets.TryGetValue(address, out var refAsset))
        {
            refAsset.RefCount++;
            if (_cache.TryGet(address, out var cached))
                return cached as T;

            // Safety: object may have been GC'd but still referenced.
            return refAsset.Handle.Result as T;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[AddressableLoader] Failed to load asset at address: {address}");
            return null;
        }

        _assets[address] = new RefCountedAsset { Handle = handle, RefCount = 1 };
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
                _cache.Clear(); // Optional, or selectively remove
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
    public static string GetCharacterHairAddress(string id) =>
        $"{AddressableConfig.CharacterHairPath}{AddressableConfig.PathSeparator}{id}";


    public static string GetKitBodyAddress(string kitId, string variant, string role) =>
        $"{AddressableConfig.KitBodyPath}{AddressableConfig.PathSeparator}{kitId}{AddressableConfig.PathSeparator}{variant}{AddressableConfig.PathSeparator}{role}";
    public static string GetKitPortraitAddress(string kitId, string variant, string role, string size) =>
        $"{AddressableConfig.KitPortraitPath}{AddressableConfig.PathSeparator}{kitId}{AddressableConfig.PathSeparator}{variant}{AddressableConfig.PathSeparator}{role}{AddressableConfig.PathSeparator}{size}";

    public static string GetTeamCrestAddress(string id) =>
        $"{AddressableConfig.TeamCrestPath}{AddressableConfig.PathSeparator}{id}";
}
