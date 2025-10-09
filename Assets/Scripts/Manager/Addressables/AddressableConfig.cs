using UnityEngine;

public static class AddressableConfig
{
    [Header("Character Paths")]
    public const string CharacterHeadPath = "characters/heads";
    public const string CharacterBodyPath = "characters/bodies";
    public const string CharacterPortraitPath = "characters/portraits";

    [Header("Kit Paths")]
    public const string KitBodyPath = "kits/bodies";
    public const string KitPortraitPath = "kits/portraits";

    [Header("Cache Settings")]
    [Tooltip("Maximum number of assets cached in memory using LRU cache.")]
    public const int MaxCacheSize = 50;
}
