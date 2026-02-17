using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SceneGroup", menuName = "ScriptableObject/Scene/SceneGroup")]
public class SceneGroup : ScriptableObject
{
    [Tooltip("Display name for this group (e.g., 'Battle', 'MainMenu')")]
    public string groupName;

    [Tooltip("Ordered list of scenes to load as part of this group")]
    public List<SceneData> scenes = new List<SceneData>();

    [Tooltip("If true, the entire group will go through the loading screen")]
    public bool useLoadingScreen = true;

    /// <summary>
    /// Returns only the scenes that should be loaded in the current build context.
    /// </summary>
    public List<SceneData> GetValidScenes()
    {
        List<SceneData> validScenes = new List<SceneData>();

        foreach (SceneData scene in scenes)
        {
            if (scene == null) continue;

            if (scene.debugOnly)
            {
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                validScenes.Add(scene);
                #endif
            }
            else
            {
                validScenes.Add(scene);
            }
        }

        return validScenes;
    }
}
