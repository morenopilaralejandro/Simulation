using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Move;

[CreateAssetMenu(fileName = "KitPortraitSprites", menuName = "ScriptableObject/KitPortraitSprites")]
public class KitPortraitSprites : ScriptableObject 
{ 
    public Sprite SpriteBase;
    public Sprite SpriteDetail;
    public Sprite SpriteNeck;
}
