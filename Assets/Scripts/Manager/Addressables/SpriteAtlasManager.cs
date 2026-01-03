using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteAtlasManager : MonoBehaviour
{
    public static SpriteAtlasManager Instance { get; private set; }

    private readonly Dictionary<string, SpriteAtlas> spriteAtlasDict = new();

    public bool IsReady { get; private set; } = false;

    private const string TEAM_CREST_ATLAS_ID = "atlases-teams-crests";
    private const string CHARACTER_PORTRAIT_ATLAS_ID = "atlases-characters-portraits";
    private const string CHARACTER_COMMON_ATLAS_ID = "atlases-characters-common";

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

    public Task<Sprite> GetSpriteAsync(string atlasId, string spriteId)
    {
        var atlas = spriteAtlasDict[atlasId];
        var sprite = atlas.GetSprite(spriteId);
        if (sprite == null)
            Debug.LogWarning($"Sprite '{spriteId}' not found in atlas '{atlasId}'");
        return Task.FromResult(sprite);
    }

    public Task<Sprite> GetTeamCrest(string id)
    {
        var atlasId = TEAM_CREST_ATLAS_ID;
        var spriteId = AddressableLoader.GetTeamCrestAddress(id);;
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetCharacterPortrait(string id)
    {
        var atlasId = CHARACTER_PORTRAIT_ATLAS_ID;
        var spriteId = AddressableLoader.GetCharacterPortraitAddress(id);;
        return GetSpriteAsync(atlasId, spriteId);
    }

    public Task<Sprite> GetCharacterHair(string id)
    {
        var atlasId = CHARACTER_COMMON_ATLAS_ID;
        var spriteId = AddressableLoader.GetCharacterHairAddress(id);;
        return GetSpriteAsync(atlasId, spriteId);
    }

}
