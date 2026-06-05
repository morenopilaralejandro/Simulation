using UnityEngine;

public static class AddressableConfig
{
    [Header("Character Paths")]
    public const string CharacterHeadPath = "characters-heads";
    public const string CharacterBodyPath = "library-body";
    public const string CharacterPortraitPath = "characters-portraits";
    public const string CharacterHairFrontPath = "library-hair";
    public const string CharacterHairBackPath = "library-hair";
    public const string CharacterHairWorldPath = "character-world-hair";
    public const string ItemIconPath = "item-icon";

    [Header("Kit Paths")]
    public const string KitBodyPath = "library";
    public const string KitPortraitPath = "portrait";

    [Header("Team Paths")]
    public const string TeamEmblemPath = "";

    [Header("Move Paths")]
    public const string MoveEvolutionPath = "evolution";

    [Header("NPC Paths")]
    public const string NpcPortraitPath = "npc-portraits";

    [Header("Wing Paths")]
    public const string WingFrontPath = "library-wings";
    public const string WingBackPath = "library-wings";
    public const string WingEvolutionPath = "wing-evolution";

    [Header("Path Settings")]
    public const string PathSeparator = "-";
    public const string SubSeparator = "_";

    [Header("Cache Settings")]
    [Tooltip("Maximum number of assets cached in memory using LRU cache.")]
    public const int MaxCacheSize = 50; //100
}
