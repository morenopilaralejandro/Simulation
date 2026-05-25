using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using System.Threading.Tasks;

public class CharacterComponentAppearanceBattle : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields

    [SerializeField] private SpriteLibrary kitSpriteLibrary;
    [SerializeField] private SpriteLibrary hairFrontLibrary;
    [SerializeField] private SpriteLibrary hairBackLibrary;

    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer hairFrontRenderer;
    [SerializeField] private SpriteRenderer hairBackRenderer;

    [SerializeField] private GameObject hairFrontObject;
    [SerializeField] private GameObject hairBackObject;

    private CharacterEntityBattle characterEntityBattle;

    private readonly AddressableBinding<SpriteLibraryAsset> _kitBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _hairFrontBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _hairBackBinding = new();

    public bool IsLoaded =>
        kitSpriteLibrary.spriteLibraryAsset != null &&
        hairFrontLibrary.spriteLibraryAsset != null &&
        hairBackLibrary.spriteLibraryAsset != null;

    #endregion

    #region Initialization

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    private void OnDestroy()
    {
        Clear();
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        SetBodyColor();
        SetHairColor();

        await LoadHairFrontAsync();
        await LoadHairBackAsync();
    }

    public async Task AppearanceBattleLoadAsync()
    {
        SetBodyColor();
        SetHairColor();

        await LoadHairFrontAsync();
        await LoadHairBackAsync();
        await LoadKitAsync();
    }

    #endregion

    #region Body

    private void SetBodyColor()
    {
        bodyRenderer.color = characterEntityBattle.ColorBody;
    }

    #endregion

    #region Kit

    public async Task LoadKitAsync()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _kitBinding.LoadAsync(characterEntityBattle.KitAddress);

        if (asset == null)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Kit load failed: {characterEntityBattle.KitAddress}");
            return;
        }

        kitSpriteLibrary.spriteLibraryAsset = asset;

        characterEntityBattle.RefreshAnimation();
    }

    #endregion

    #region Hair

    private void SetHairColor()
    {
        hairFrontRenderer.color = characterEntityBattle.ColorHair;
        hairBackRenderer.color = characterEntityBattle.ColorHair;
    }

    private async Task LoadHairFrontAsync()
    {
        hairFrontLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _hairFrontBinding.LoadOptionalAsync(characterEntityBattle.HairFrontAddress);

        if (asset == null)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Hair front load failed: {characterEntityBattle.HairFrontAddress}");

            hairFrontObject.SetActive(false);
            return;
        }

        if (!hairFrontObject.activeSelf)
            hairFrontObject.SetActive(true);

        hairFrontLibrary.spriteLibraryAsset = asset;
    }

    private async Task LoadHairBackAsync()
    {
        hairBackLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _hairBackBinding.LoadOptionalAsync(characterEntityBattle.HairBackAddress);

        if (asset == null)
        {
            LogManager.Trace($"[CharacterComponentAppearanceBattle] Hair back load failed: {characterEntityBattle.HairBackAddress}");

            hairBackObject.SetActive(false);
            return;
        }

        if (!hairBackObject.activeSelf)
            hairBackObject.SetActive(true);

        hairBackLibrary.spriteLibraryAsset = asset;
    }

    #endregion

    #region Cleanup

    public void Clear()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;
        hairFrontLibrary.spriteLibraryAsset = null;
        hairBackLibrary.spriteLibraryAsset = null;

        _kitBinding.Release();
        _hairFrontBinding.Release();
        _hairBackBinding.Release();

        _kitBinding.Cancel();
        _hairFrontBinding.Cancel();
        _hairBackBinding.Cancel();
    }

    #endregion
}
