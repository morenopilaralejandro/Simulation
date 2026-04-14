using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteAtlasManager : MonoBehaviour
{
    #region Fields

    public static SpriteAtlasManager Instance { get; private set; }

    private readonly Dictionary<string, SpriteAtlas> spriteAtlasDict = new();

    public bool IsReady { get; private set; } = false;

    private const string TEAM_CREST_ATLAS_ID = "atlases-teams-crests";
    private const string CHARACTER_PORTRAIT_ATLAS_ID = "atlases-characters-portraits";
    private const string CHARACTER_COMMON_ATLAS_ID = "atlases-characters-common";
    private const string CHARACTER_WORLD_ATLAS_ID = "atlases-characters-world";
    private const string ITEM_ICON_ATLAS_ID = "atlases-items-icons";

    #endregion

    #region Lifecycle

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region async

    public async Task LoadAllSpriteAtlasAsync()
    {
        var handle = Addressables.LoadAssetsAsync<SpriteAtlas>(
            "Atlases-Sprites",
            atlas => spriteAtlasDict[atlas.name] = atlas
        );
        await handle.Task;
        IsReady = true;
        LogManager.Trace($"[SpriteAtlasManager] All SpriteAtlas loaded. Total count: {spriteAtlasDict.Count}", this);
    }

    #endregion

    #region Logic

    public Task<Sprite> GetSpriteAsync(string atlasId, string spriteId)
    {
        var atlas = spriteAtlasDict[atlasId];
        var sprite = atlas.GetSprite(spriteId);
        if (sprite == null)
            Debug.LogWarning($"Sprite '{spriteId}' not found in atlas '{atlasId}'");
        return Task.FromResult(sprite);
    }

    #endregion

    #region Get All

    public Dictionary<string, Sprite> GetAllSpritesFromAtlas(string atlasId)
    {
        var result = new Dictionary<string, Sprite>();

        if (!spriteAtlasDict.TryGetValue(atlasId, out var atlas))
        {
            Debug.LogWarning($"Atlas '{atlasId}' not found.");
            return result;
        }

        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        foreach (var sprite in sprites)
        {
            string cleanName = sprite.name.Replace("(Clone)", "").Trim();
            cleanName = cleanName.Replace("teams-crests-", "").Trim();
            result[cleanName] = sprite;
        }

        return result;
    }

    public Dictionary<string, Sprite> GetAllSpritesFromAtlasTeamCrest() => GetAllSpritesFromAtlas(TEAM_CREST_ATLAS_ID);

    #endregion


    #region Get One

    public Task<Sprite> GetTeamCrest(string id)
    {
        var atlasId = TEAM_CREST_ATLAS_ID;
        var spriteId = AddressableLoader.GetTeamCrestAddress(id);
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetCharacterPortrait(string id)
    {
        var atlasId = CHARACTER_PORTRAIT_ATLAS_ID;
        var spriteId = AddressableLoader.GetCharacterPortraitAddress(id);
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetCharacterHair(string id)
    {
        var atlasId = CHARACTER_COMMON_ATLAS_ID;
        var spriteId = AddressableLoader.GetCharacterHairAddress(id);
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetCharacterHairWorld(string id)
    {
        var atlasId = CHARACTER_WORLD_ATLAS_ID;
        var spriteId = AddressableLoader.GetCharacterHairWorldAddress(id);
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetItemIcon(string id)
    {
        var atlasId = ITEM_ICON_ATLAS_ID;
        var spriteId = AddressableLoader.GetItemIconAddress(id);
        return GetSpriteAsync(atlasId, spriteId);
    }

    #endregion

}
