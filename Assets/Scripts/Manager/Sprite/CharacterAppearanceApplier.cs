using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using Aremoreno.Enums.Kit;

public class CharacterAppearanceApplier : MonoBehaviour
{
    /*
    [Header("Sprite Libraries")]
    [SerializeField] private SpriteLibrary hairLibrary;
    [SerializeField] private SpriteLibrary bodyLibrary;
    [SerializeField] private SpriteLibrary kitLibrary;

    private CharacterAppearanceLibraryLoader loader;

    public async void Initialize(CharacterData data)
    {
        loader = new CharacterAppearanceLibraryLoader();

        await loader.LoadHair(data.HairStyle);
        await loader.LoadBody(data.BodyColorType);

        Apply();
    }

    public async void ApplyKit(Kit kit, Variant variant)
    {
        await loader.LoadKit(kit, variant);
        Apply();
    }

    private void Apply()
    {
        if (loader.HairLibrary != null)
            hairLibrary.spriteLibraryAsset = loader.HairLibrary;

        if (loader.BodyLibrary != null)
            bodyLibrary.spriteLibraryAsset = loader.BodyLibrary;

        if (loader.KitLibrary != null)
            kitLibrary.spriteLibraryAsset = loader.KitLibrary;
    }
    */
}
