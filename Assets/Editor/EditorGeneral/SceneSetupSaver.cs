using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.IO;

public class SceneSetupSaver
{
    private static string SavePath => "Assets/Editor/EditorGeneral/SceneSetup.json";

    [MenuItem("Tools/General/Scenes/Save Scene Setup")]
    public static void SaveSetup()
    {
        var setup = EditorSceneManager.GetSceneManagerSetup();
        string json = JsonUtility.ToJson(new SceneSetupWrapper { setups = setup }, true);
        File.WriteAllText(SavePath, json);
        AssetDatabase.Refresh();
        Debug.Log("Scene setup saved!");
    }

    [MenuItem("Tools/General/Scenes/Restore Scene Setup")]
    public static void RestoreSetup()
    {
        if (!File.Exists(SavePath)) 
        {
            Debug.LogWarning("No saved scene setup found.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        var wrapper = JsonUtility.FromJson<SceneSetupWrapper>(json);
        EditorSceneManager.RestoreSceneManagerSetup(wrapper.setups);
        Debug.Log("Scene setup restored!");
    }

    [System.Serializable]
    private class SceneSetupWrapper
    {
        public SceneSetup[] setups;
    }
}
