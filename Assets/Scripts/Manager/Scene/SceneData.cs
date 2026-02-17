using UnityEngine;

[CreateAssetMenu(fileName = "SceneData", menuName = "ScriptableObject/Scene/SceneData")]
public class SceneData : ScriptableObject
{
    [Tooltip("Must match the actual scene name in Build Settings")]
    public string sceneName;

    [Tooltip("If true, this scene is only loaded in development/editor builds")]
    public bool debugOnly;

    [Tooltip("If true, loading this scene will go through the loading screen")]
    public bool useLoadingScreen;
}
