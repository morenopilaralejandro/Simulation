using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "ScriptableObject/Spritesheet/CharacterAnimationConfig")]
public class CharacterAnimationConfig : ScriptableObject
{
    public List<AnimationEntry> animations = new();
}

[Serializable]
public class AnimationEntry
{
    public string name;
    public int frames;

    public AnimationDirection direction;
}

public enum AnimationDirection
{
    FourDir,
    DownOnly
}
