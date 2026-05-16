using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using System.IO;

public class SpriteLibraryAssetGenerator : EditorWindow
{
    string saveFolder =
        "Assets/Addressables/AddressSpriteLibrary/";

    string fileName =
        "sprite-library-character-base.asset";

    Sprite placeholderSprite;

    static readonly string[] Categories =
    {
        "1h_backslash",
        //"1h_halfslash",
        //"1h_slash",
        //"climb",
        "combat",
        "emote",
        "hurt",
        "idle",
        "jump",
        "run",
        //"shoot",
        //"sit",
        "slash",
        "spellcast",
        //"thrust",
        "walk"
        //"watering",
    };

    static readonly string[] Directions =
    {
        "up",
        "left",
        "down",
        "right"
    };

    static readonly Dictionary<string, int> Animations = new()
    {
        { "1h_backslash", 13 },
        //{ "1h_halfslash", 6 },
        //{ "1h_slash", 13 },
        { "combat", 2 },
        { "emote", 3 },
        { "idle", 2 },
        { "jump", 5 },
        { "run", 8 },
        //{ "shoot", 13 },
        //{ "sit", 3 },
        { "slash", 6 },
        { "spellcast", 7 },
        //{ "thrust", 8 },
        { "walk", 9 }
        //{ "watering", 8 }
    };

    static readonly Dictionary<string, int> DownOnlyAnimations = new()
    {
        //{ "climb", 6 },
        { "hurt", 6 }
    };

    [MenuItem("Tools/Spritesheet/Create Sprite Library Asset")]
    public static void ShowWindow()
    {
        GetWindow<SpriteLibraryAssetGenerator>(
            "Sprite Library Generator");
    }

    void OnGUI()
    {
        GUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Create Sprite Library Asset",
            EditorStyles.boldLabel);

        GUILayout.Space(10);

        saveFolder = EditorGUILayout.TextField(
            "Save Folder",
            saveFolder);

        fileName = EditorGUILayout.TextField(
            "File Name",
            fileName);

        GUILayout.Space(5);

        placeholderSprite = (Sprite)
            EditorGUILayout.ObjectField(
                "Placeholder Sprite",
                placeholderSprite,
                typeof(Sprite),
                false);

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "Creates a fully editable SpriteLibraryAsset " +
            "stored as a normal .asset file.\n\n" +
            "This avoids Unity's importer-managed " +
            ".spriteLib pipeline.",
            MessageType.Info);

        GUILayout.Space(15);

        GUI.enabled =
            placeholderSprite != null &&
            !string.IsNullOrWhiteSpace(saveFolder) &&
            !string.IsNullOrWhiteSpace(fileName);

        if (GUILayout.Button(
            "Create Sprite Library Asset",
            GUILayout.Height(40)))
        {
            CreateLibrary();
        }

        GUI.enabled = true;
    }

    void CreateLibrary()
    {
        if (!AssetDatabase.IsValidFolder(saveFolder))
        {
            Debug.LogError(
                $"Folder does not exist:\n{saveFolder}");
            return;
        }

        if (!fileName.EndsWith(".asset"))
        {
            fileName += ".asset";
        }

        string fullPath =
            Path.Combine(saveFolder, fileName);

        fullPath = fullPath.Replace("\\", "/");

        Object existing =
            AssetDatabase.LoadAssetAtPath<Object>(
                fullPath);

        if (existing != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "Asset Exists",
                $"Asset already exists:\n{fullPath}\n\nOverwrite?",
                "Overwrite",
                "Cancel");

            if (!overwrite)
                return;

            AssetDatabase.DeleteAsset(fullPath);
        }

        // Create editable SpriteLibraryAsset
        SpriteLibraryAsset asset =
            ScriptableObject.CreateInstance<
                SpriteLibraryAsset>();

        AssetDatabase.CreateAsset(
            asset,
            fullPath);

        SerializedObject so =
            new SerializedObject(asset);

        SerializedProperty labels =
            so.FindProperty("m_Labels");

        if (labels == null)
        {
            Debug.LogError(
                "Could not find m_Labels.\n\n" +
                "Your Unity version may use a " +
                "different internal structure.");
            return;
        }

        labels.ClearArray();

        int categoryIndex = 0;

        foreach (string categoryName in Categories)
        {
            labels.InsertArrayElementAtIndex(
                categoryIndex);

            SerializedProperty category =
                labels.GetArrayElementAtIndex(
                    categoryIndex);

            category.FindPropertyRelative("m_Name")
                .stringValue = categoryName;

            SerializedProperty hashProp =
                category.FindPropertyRelative(
                    "m_Hash");

            if (hashProp != null)
            {
                hashProp.intValue =
                    categoryName.GetHashCode();
            }

            SerializedProperty categoryList =
                category.FindPropertyRelative(
                    "m_CategoryList");

            categoryList.ClearArray();

            int labelIndex = 0;

            // Generate only labels for this category
            if (Animations.ContainsKey(categoryName))
            {
                int frameCount = Animations[categoryName];

                foreach (string dir in Directions)
                {
                    for (int i = 0; i < frameCount; i++)
                    {
                        categoryList.InsertArrayElementAtIndex(labelIndex);

                        SerializedProperty entry =
                            categoryList.GetArrayElementAtIndex(labelIndex);

                        string label = $"{categoryName}_{dir}_{i}";

                        entry.FindPropertyRelative("m_Name")
                            .stringValue = label;

                        entry.FindPropertyRelative("m_Sprite")
                            .objectReferenceValue = placeholderSprite;

                        labelIndex++;
                    }
                }
            }

            // Down-only categories
            if (DownOnlyAnimations.ContainsKey(categoryName))
            {
                int frameCount = DownOnlyAnimations[categoryName];

                for (int i = 0; i < frameCount; i++)
                {
                    categoryList.InsertArrayElementAtIndex(labelIndex);

                    SerializedProperty entry =
                        categoryList.GetArrayElementAtIndex(labelIndex);

                    string label = $"{categoryName}_down_{i}";

                    entry.FindPropertyRelative("m_Name")
                        .stringValue = label;

                    entry.FindPropertyRelative("m_Sprite")
                        .objectReferenceValue = placeholderSprite;

                    labelIndex++;
                }
            }

            categoryIndex++;
        }

        so.ApplyModifiedPropertiesWithoutUndo();

        EditorUtility.SetDirty(asset);

        AssetDatabase.SaveAssets();

        AssetDatabase.ImportAsset(
            fullPath,
            ImportAssetOptions.ForceUpdate);

        AssetDatabase.Refresh();

        Selection.activeObject = asset;

        Debug.Log(
            $"Created SpriteLibraryAsset:\n{fullPath}");
    }
}
