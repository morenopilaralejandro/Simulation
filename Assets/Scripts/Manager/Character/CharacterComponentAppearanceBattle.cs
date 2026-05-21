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
    [SerializeField] private SpriteLibrary bodySpriteLibrary;
    [SerializeField] private SpriteLibrary kitSpriteLibrary;

    private CharacterEntityBattle characterEntityBattle;
    private AsyncOperationHandle<SpriteLibraryAsset>? bodyHandle;
    private AsyncOperationHandle<SpriteLibraryAsset>? kitHandle;

    public bool IsLoaded =>
        bodySpriteLibrary.spriteLibraryAsset != null &&
        kitSpriteLibrary.spriteLibraryAsset != null;

    #endregion

    #region Initialization

    public void Initialize(CharacterEntityBattle characterEntityBattle)
    {
        this.characterEntityBattle = characterEntityBattle;
    }

    private void OnDestroy()
    {
        ReleaseBody();
        ReleaseKit();
    }

    #endregion

    #region Async Loading

    public async Task LoadAsync()
    {
        await LoadBodyAsync();
    }


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
    

    #endregion

    #region Kit

    public async Task LoadKitAsync()
    {
        ReleaseKit();

        string address =
            AddressableLoader.GetKitBodyAddress(
                characterEntityBattle.KitId,
                characterEntityBattle.KitVariant.ToString().ToLower(),
                characterEntityBattle.KitRole.ToString().ToLower()
            );

        kitHandle = Addressables.LoadAssetAsync<SpriteLibraryAsset>(address);
        await kitHandle.Value.Task;

        if (kitHandle.Value.Status != AsyncOperationStatus.Succeeded)
        {
            LogManager.Error($"[CharacterComponentAppearanceBattle] Kit load failed: {address}");
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

    #region Helpers

    private void ReleaseBody()
    {
        bodySpriteLibrary.spriteLibraryAsset = null;

        if (bodyHandle.HasValue)
        {
            Addressables.Release(bodyHandle.Value);
            bodyHandle = null;
        }
    }

    private void ReleaseKit()
    {
        kitSpriteLibrary.spriteLibraryAsset = null;

        if (kitHandle.HasValue)
        {
            Addressables.Release(kitHandle.Value);
            kitHandle = null;
        }
    }

    public async Task AppearanceBattleLoadAsync() 
    {
        await LoadBodyAsync();
        await LoadKitAsync();
    }

    #endregion
}
