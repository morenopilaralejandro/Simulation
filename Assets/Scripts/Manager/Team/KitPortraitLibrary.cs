using UnityEngine;
using System;
using System.Collections.Generic;
using Simulation.Enums.Character;

[CreateAssetMenu(fileName = "KitPortraitSprites", menuName = "Library/Portrait/KitPortraitLibrary")]
public class KitPortraitLibrary : ScriptableObject 
{ 
    [Header("Portrait Sprite Kits")]
    public KitPortraitSprites SpritesXS;
    public KitPortraitSprites SpritesS;
    public KitPortraitSprites SpritesSM;
    public KitPortraitSprites SpritesM;
    public KitPortraitSprites SpritesML;
    public KitPortraitSprites SpritesL;
    public KitPortraitSprites SpritesXL;

    public KitPortraitSprites Get(PortraitSize size)
    {
        return size switch
        {
            PortraitSize.XS => SpritesXS,
            PortraitSize.S  => SpritesS,
            PortraitSize.SM => SpritesSM,
            PortraitSize.M  => SpritesM,
            PortraitSize.ML => SpritesML,
            PortraitSize.L  => SpritesL,
            PortraitSize.XL => SpritesXL,

            _ => SpritesM
        };
    }

}
