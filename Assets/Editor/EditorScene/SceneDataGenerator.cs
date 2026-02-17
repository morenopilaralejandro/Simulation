using UnityEngine;
using UnityEditor;
using System.IO;

public class SceneDataGenerator
{
    private const string OutputFolder =
        "Assets/Addressables/AddressScene/AddressSceneData";

    [MenuItem("Tools/Scene/Generate SceneData")]
    public static void GenerateSceneData()
    {
        // Ensure the output folder exists
        if (!AssetDatabase.IsValidFolder(OutputFolder))
        {
            CreateFolderRecursively(OutputFolder);
        }

        // Find all scene assets in the project
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        int created = 0;
        int skipped = 0;
        int ignored = 0;

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath).ToLower();

            // Skip scenes whose name starts with "~"
            if (sceneName.StartsWith("~"))
            {
                ignored++;
                continue;
            }

            string assetPath = $"{OutputFolder}/scene_data-{sceneName}.asset";

            // Skip if a SceneData for this scene already exists
            if (AssetDatabase.LoadAssetAtPath<SceneData>(assetPath) != null)
            {
                skipped++;
                continue;
            }

            // Create the ScriptableObject instance
            SceneData sceneData = ScriptableObject.CreateInstance<SceneData>();
            sceneData.sceneName = sceneName;
            sceneData.debugOnly = false;
            sceneData.useLoadingScreen = false;

            AssetDatabase.CreateAsset(sceneData, assetPath);
            created++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[SceneDataGenerator] Done. Created: {created}, Skipped (already exist): {skipped}, Ignored (~prefix): {ignored}");
    }

    private static void CreateFolderRecursively(string folderPath)
    {
        folderPath = folderPath.Replace("\\", "/");

        string[] parts = folderPath.Split('/');
        string current = parts[0]; // "Assets"

        for (int i = 1; i < parts.Length; i++)
        {
            string next = $"{current}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
            {
                AssetDatabase.CreateFolder(current, parts[i]);
            }
            current = next;
        }
    }
}
