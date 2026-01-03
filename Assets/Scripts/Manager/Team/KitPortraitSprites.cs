using UnityEngine;
using System;
using System.Collections.Generic;
using Simulation.Enums.Character;
using Simulation.Enums.Move;

[CreateAssetMenu(fileName = "KitPortraitSprites", menuName = "ScriptableObject/KitPortraitSprites")]
public class KitPortraitSprites : ScriptableObject 
{ 
    public Sprite SpriteBase;
    public Sprite SpriteDetail;
    public Sprite SpriteNeck;
}
