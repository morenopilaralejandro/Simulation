using UnityEngine;

public static class AddressableConfig
{
    [Header("Character Paths")]
    public const string CharacterHeadPath = "characters-heads";
    public const string CharacterBodyPath = "characters-bodies";
    public const string CharacterPortraitPath = "characters-portraits";
    public const string CharacterHairPath = "character-hair";
    public const string CharacterHairWorldPath = "character-world-hair";
    public const string ItemIconPath = "item-icon";

    [Header("Kit Paths")]
    public const string KitBodyPath = "kits-bodies";
    public const string KitPortraitPath = "portrait";

    [Header("Team Paths")]
    public const string TeamCrestPath = "teams-crests";

    [Header("Path Settings")]
    public const string PathSeparator = "-";
    public const string SubSeparator = "_";

    [Header("Cache Settings")]
    [Tooltip("Maximum number of assets cached in memory using LRU cache.")]
    public const int MaxCacheSize = 100;
}
