using UnityEngine;

[CreateAssetMenu(fileName = "DefaultSettings", menuName = "Settings/DefaultSettings")]
public class DefaultSettings : ScriptableObject
{
    public GameSettings PresetDefault;

    [Header("Presets")]
    public GameSettings PresetTouch;
    public GameSettings PresetTraditional;
}
