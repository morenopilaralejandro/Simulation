using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.U2D.Animation;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Kit;

public class CharacterAppearanceLibraryLoader
{
    /*
    public SpriteLibraryAsset HairLibrary { get; private set; }
    public SpriteLibraryAsset BodyLibrary { get; private set; }
    public SpriteLibraryAsset KitLibrary { get; private set; }

    public async Task LoadHair(HairStyle style)
    {
        string key = GetHairKey(style);
        //HairLibrary = await SpriteLibraryAddressables.Load(key);
    }

    public async Task LoadKit(Kit kit, Variant variant)
    {
        string key = GetKitKey(kit, variant);
        //KitLibrary = await SpriteLibraryAddressables.Load(key);
    }

    public async Task LoadBody(BodyColorType body)
    {
        string key = GetBodyKey(body);
        //BodyLibrary = await SpriteLibraryAddressables.Load(key);
    }

    private string GetHairKey(HairStyle style)
    {

        return style switch
        {
            HairStyle.Long => "hair_long",
            HairStyle.Short => "hair_short",
            _ => "hair_default"
        };
 
        return "";
    }

    private string GetKitKey(Kit kit, Variant variant)
    {
        return kit.KitId + "_" + variant.ToString().ToLower();
    }

    private string GetBodyKey(BodyColorType body)
    {
        return "body_default";
    }
    */
}
