using UnityEngine;
using System;
using System.Collections.Generic;
using Aremoreno.Enums.Animation;

[CreateAssetMenu(menuName = "ScriptableObject/Animation/CharacterAnimationConfig")]
public class CharacterAnimationConfig : ScriptableObject
{
    public List<CharacterAnimationEntry> animations = new(); 
}

[Serializable]
public class CharacterAnimationEntry
{
    public string name;
    public int frames;

    public CharacterAnimationEntryDirection direction;

    public bool loop = true;
    public float frameRate = 12f;
}
