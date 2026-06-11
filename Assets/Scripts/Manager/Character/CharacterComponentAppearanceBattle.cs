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
    [SerializeField] private SpriteLibrary wingFrontLibrary;
    [SerializeField] private SpriteLibrary wingBackLibrary;

    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer hairFrontRenderer;
    [SerializeField] private SpriteRenderer hairBackRenderer;
    [SerializeField] private SpriteRenderer wingFrontRenderer;
    [SerializeField] private SpriteRenderer wingBackRenderer;

    [SerializeField] private GameObject hairFrontObject;
    [SerializeField] private GameObject hairBackObject;
    [SerializeField] private GameObject wingFrontObject;
    [SerializeField] private GameObject wingBackObject;

    [SerializeField] private CharacterComponentAnimationController animationControllerComponent;
    private CharacterComponentAppearance appearanceComponent;
    private CharacterEntityBattle characterEntityBattle;

    private readonly AddressableBinding<SpriteLibraryAsset> _kitBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _hairFrontBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _hairBackBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _wingFrontBinding = new();
    private readonly AddressableBinding<SpriteLibraryAsset> _wingBackBinding = new();

    public bool IsLoaded =>
        kitSpriteLibrary.spriteLibraryAsset != null &&
        hairFrontLibrary.spriteLibraryAsset != null &&
        hairBackLibrary.spriteLibraryAsset != null;

    #endregion

    #region Initialization

    public void Initialize(CharacterComponentAppearance appearanceComponent)
    {
        this.appearanceComponent = appearanceComponent;
    }

    private void OnDestroy()
    {
        Clear();
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        UnloadWing();

        SetBodyColor();
        SetHairColor();

        await LoadHairFrontAsync();
        await LoadHairBackAsync();
    }

    public async Task AppearanceBattleLoadAsync(bool hasWingActivated = false)
    {
        SetBodyColor();
        SetHairColor();

        await LoadHairFrontAsync();
        await LoadHairBackAsync();
        await LoadKitAsync();

        if (hasWingActivated)
            await LoadWingAsync();
        else
            UnloadWing();
    }

    #endregion

    #region Body

    private void SetBodyColor()
    {
        bodyRenderer.color = appearanceComponent.ColorBody;
    }

    #endregion

    #region Kit

    public async Task LoadKitAsync()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _kitBinding.LoadAsync(appearanceComponent.KitAddress);

        if (asset == null)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Kit load failed: {appearanceComponent.KitAddress}");
            return;
        }

        kitSpriteLibrary.spriteLibraryAsset = asset;

        animationControllerComponent.RefreshAnimation();
    }

    #endregion

    #region Hair

    private void SetHairColor()
    {
        hairFrontRenderer.color = appearanceComponent.ColorHair;
        hairBackRenderer.color = appearanceComponent.ColorHair;
    }

    private async Task LoadHairFrontAsync()
    {
        hairFrontLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _hairFrontBinding.LoadOptionalAsync(appearanceComponent.HairFrontAddress);

        if (asset == null)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Hair front load failed: {appearanceComponent.HairFrontAddress}");

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
            await _hairBackBinding.LoadOptionalAsync(appearanceComponent.HairBackAddress);

        if (asset == null)
        {
            LogManager.Trace($"[CharacterComponentAppearanceBattle] Hair back load failed: {appearanceComponent.HairBackAddress}");

            hairBackObject.SetActive(false);
            return;
        }

        if (!hairBackObject.activeSelf)
            hairBackObject.SetActive(true);

        hairBackLibrary.spriteLibraryAsset = asset;
    }

    #endregion

    #region Hair

    private void SetWingColor()
    {
        wingFrontRenderer.color = appearanceComponent.ColorWing;
        wingBackRenderer.color = appearanceComponent.ColorWing;
    }

    private async Task LoadWingFrontAsync()
    {
        wingFrontLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _wingFrontBinding.LoadOptionalAsync(appearanceComponent.WingFrontAddress);

        if (asset == null)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Wing front load failed: {appearanceComponent.WingFrontAddress}");

            wingFrontObject.SetActive(false);
            return;
        }

        if (!wingFrontObject.activeSelf)
            wingFrontObject.SetActive(true);

        wingFrontLibrary.spriteLibraryAsset = asset;
    }

    private async Task LoadWingBackAsync()
    {
        wingBackLibrary.spriteLibraryAsset = null;

        SpriteLibraryAsset asset =
            await _wingBackBinding.LoadOptionalAsync(appearanceComponent.WingBackAddress);

        if (asset == null)
        {
            LogManager.Trace($"[CharacterComponentAppearanceBattle] Wing back load failed: {appearanceComponent.WingBackAddress}");

            wingBackObject.SetActive(false);
            return;
        }

        if (!wingBackObject.activeSelf)
            wingBackObject.SetActive(true);

        wingBackLibrary.spriteLibraryAsset = asset;
    }

    public async Task LoadWingAsync()
    {
        await LoadWingFrontAsync();
        await LoadWingBackAsync();
        SetWingColor();
    }

    public void UnloadWing()
    {
        wingFrontObject.SetActive(false);
        wingBackObject.SetActive(false);

        wingFrontLibrary.spriteLibraryAsset = null;
        wingBackLibrary.spriteLibraryAsset = null;

        _hairFrontBinding.Release();
        _hairBackBinding.Release();

        _hairFrontBinding.Cancel();
        _hairBackBinding.Cancel();
    }

    #endregion

    #region Cleanup

    public void Clear()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;
        hairFrontLibrary.spriteLibraryAsset = null;
        hairBackLibrary.spriteLibraryAsset = null;
        wingFrontLibrary.spriteLibraryAsset = null;
        wingBackLibrary.spriteLibraryAsset = null;

        _kitBinding.Release();
        _hairFrontBinding.Release();
        _hairBackBinding.Release();
        _wingFrontBinding.Release();
        _wingBackBinding.Release();

        _kitBinding.Cancel();
        _hairFrontBinding.Cancel();
        _hairBackBinding.Cancel();
        _wingFrontBinding.Cancel();
        _wingBackBinding.Cancel();
    }

    #endregion
}
