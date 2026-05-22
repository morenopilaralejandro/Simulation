using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using System.Threading.Tasks;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;
using Aremoreno.Enums.SpriteLayer;

public class CharacterComponentAppearanceBattle : MonoBehaviour, IAsyncSceneLoader
{
    #region Fields
    //[SerializeField] private SpriteLibrary bodySpriteLibrary;
    [SerializeField] private SpriteLibrary kitSpriteLibrary;
    [SerializeField] private SpriteLibrary hairFrontLibrary;
    [SerializeField] private SpriteLibrary hairBackLibrary;

    [SerializeField] private SpriteRenderer bodyRenderer;
    [SerializeField] private SpriteRenderer hairFrontRenderer;
    [SerializeField] private SpriteRenderer hairBackRenderer;
    [SerializeField] private GameObject hairBackObject;

    private CharacterEntityBattle characterEntityBattle;
    private AsyncOperationHandle<SpriteLibraryAsset>? hairFrontHandle;
    private AsyncOperationHandle<SpriteLibraryAsset>? hairBackHandle;
    private AsyncOperationHandle<SpriteLibraryAsset>? kitHandle;

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
        ReleaseHairFront();
        ReleaseHairBack();
        ReleaseKit();
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

    #endregion

    #region Body

    private void SetBodyColor() 
    {
        bodyRenderer.color = characterEntityBattle.ColorBody;
    }

    /*
    private async Task LoadBodyAsync()
    {
        //ReleaseBody();

        string address = AddressableLoader.GetCharacterBodyAddress("chara-00001-are");
            //AddressableLoader.GetCharacterBodyAddress(characterEntityBattle.CharacterId);

        bodyHandle = Addressables.LoadAssetAsync<SpriteLibraryAsset>(address);
        await bodyHandle.Value.Task;

        if (bodyHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Body load failed: {address}");
            return;
        }

        //bodySpriteLibrary.spriteLibraryAsset = bodyHandle.Value.Result;
    }
    */

    #endregion

    #region Kit

    public async Task LoadKitAsync()
    {
        ReleaseKit();

        kitHandle = Addressables.LoadAssetAsync<SpriteLibraryAsset>(characterEntityBattle.KitAddress);
        await kitHandle.Value.Task;

        if (kitHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Kit load failed: {characterEntityBattle.KitAddress}");
            return;
        }

        kitSpriteLibrary.spriteLibraryAsset = kitHandle.Value.Result;
        // play animation so that the new library is applied with the resolver
        //characterEntityBattle.RequestAction(Aremoreno.Enums.Animation.CharacterAnimationState.Slash, Vector2.left);
        //characterEntityBattle.SetLocomotion(Aremoreno.Enums.Animation.CharacterAnimationState.Idle);
        //characterEntityBattle.Play(Aremoreno.Enums.Animation.CharacterAnimationState.Idle, characterEntityBattle.FormationDirection);
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
        ReleaseHairFront();

        hairFrontHandle = Addressables.LoadAssetAsync<SpriteLibraryAsset>(characterEntityBattle.HairFrontAddress);
        await hairFrontHandle.Value.Task;

        if (hairFrontHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Hair front load failed: {characterEntityBattle.HairFrontAddress}");
            return;
        }

        hairFrontLibrary.spriteLibraryAsset = hairFrontHandle.Value.Result;
    }

    private async Task LoadHairBackAsync()
    {
        ReleaseHairBack();

        hairBackHandle = Addressables.LoadAssetAsync<SpriteLibraryAsset>(characterEntityBattle.HairBackAddress);
        await hairBackHandle.Value.Task;

        if (hairBackHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Trace($"[CharacterComponentAppearanceBattle] Hair back load failed: {characterEntityBattle.HairBackAddress}");
            hairBackObject.SetActive(false);
            return;
        }

        hairBackLibrary.spriteLibraryAsset = hairBackHandle.Value.Result;
    }

    #endregion

    #region Helpers

    /*
    private void ReleaseBody()
    {
        bodySpriteLibrary.spriteLibraryAsset = null;

        if (bodyHandle.HasValue)
        {
            Addressables.Release(bodyHandle.Value);
            bodyHandle = null;
        }
    }
    */

    private void ReleaseKit()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;

        if (kitHandle.HasValue)
        {
            Addressables.Release(kitHandle.Value);
            kitHandle = null;
        }
    }

    private void ReleaseHairFront()
    {
        hairFrontLibrary.spriteLibraryAsset = null;

        if (hairFrontHandle.HasValue)
        {
            Addressables.Release(hairFrontHandle.Value);
            hairFrontHandle = null;
        }
    }

    private void ReleaseHairBack()
    {
        hairBackLibrary.spriteLibraryAsset = null;

        if (hairBackHandle.HasValue)
        {
            Addressables.Release(hairBackHandle.Value);
            hairBackHandle = null;
        }
    }

    public async Task AppearanceBattleLoadAsync() 
    {
        await LoadHairFrontAsync();
        await LoadHairBackAsync();
        await LoadKitAsync();
        SetBodyColor();
        SetHairColor();
    }

    #endregion
}
