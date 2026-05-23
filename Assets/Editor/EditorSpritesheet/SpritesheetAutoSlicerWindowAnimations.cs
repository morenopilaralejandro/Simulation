using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.U2D;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.U2D;
using Aremoreno.Enums.Animation;

public class SpritesheetAutoSlicerWindowAnimations : EditorWindow
{
    private const int TileSize = 64;

    [SerializeField]
    private CharacterAnimationConfig config;

    private List<SpriteAtlas> selectedAtlases = new List<SpriteAtlas>();
    private bool isProcessing;

    private Vector2 scroll;

    private static readonly string[] Directions =
    {
        "up",
        "left",
        "down",
        "right"
    };

    [MenuItem("Tools/Spritesheet/Spritesheet Auto Slicer Animations")]
    public static void Open()
    {
        GetWindow<SpritesheetAutoSlicerWindowAnimations>("Spritesheet Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Spritesheet Auto Slicer Animations", EditorStyles.boldLabel);

        config = (CharacterAnimationConfig)EditorGUILayout.ObjectField(
            "Config",
            config,
            typeof(CharacterAnimationConfig),
            false
        );

        scroll = EditorGUILayout.BeginScrollView(scroll);

        GUILayout.Label("Sprite Atlases", EditorStyles.boldLabel);

        if (selectedAtlases == null)
            selectedAtlases = new List<SpriteAtlas>();

        for (int i = 0; i < selectedAtlases.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            selectedAtlases[i] = (SpriteAtlas)EditorGUILayout.ObjectField(
                selectedAtlases[i],
                typeof(SpriteAtlas),
                false
            );

            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                selectedAtlases.RemoveAt(i);
                i--;
                EditorGUILayout.EndHorizontal();
                continue;
            }

            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(5);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Add Atlas"))
            selectedAtlases.Add(null);

        if (GUILayout.Button("Clear"))
            selectedAtlases.Clear();

        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUI.enabled = !isProcessing;

        if (GUILayout.Button("Process Assets", GUILayout.Height(35)))
            ProcessAll();

        GUI.enabled = true;

        if (isProcessing)
            GUILayout.Label("Processing...", EditorStyles.helpBox);

        EditorGUILayout.EndScrollView();
    }

    private void OnEnable()
    {
        if (selectedAtlases == null)
            selectedAtlases = new List<SpriteAtlas>();

        AutoLoadAtlases();
    }

    private void AutoLoadAtlases()
    {
        selectedAtlases.Clear();

        string[] atlasNames =
        {
            "atlases-characters-animations",
            "atlases-kits-portraits"
        };

        foreach (var name in atlasNames)
        {
            string[] guids = AssetDatabase.FindAssets($"{name} t:SpriteAtlas");

            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);

                if (atlas != null && !selectedAtlases.Contains(atlas))
                    selectedAtlases.Add(atlas);
            }
        }
    }

    private void ProcessAll()
    {
        if (config == null)
        {
            Debug.LogError("CharacterAnimationConfig missing.");
            return;
        }

        isProcessing = true;

        try
        {
            if (selectedAtlases != null && selectedAtlases.Count > 0)
            {
                SpriteAtlasUtility.PackAtlases(
                    selectedAtlases.ToArray(),
                    EditorUserBuildSettings.activeBuildTarget
                );
            }

            string[] textures = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });
            HashSet<string> validFolders = FindMarkedFolders();

            AssetDatabase.StartAssetEditing();

            foreach (string guid in textures)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (!IsInValidFolder(path, validFolders))
                    continue;

                ProcessTexture(path);
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }
        finally
        {
            if (selectedAtlases != null && selectedAtlases.Count > 0)
            {
                SpriteAtlasUtility.PackAtlases(
                    selectedAtlases.ToArray(),
                    EditorUserBuildSettings.activeBuildTarget
                );
            }

            isProcessing = false;
        }
    }

    private HashSet<string> FindMarkedFolders()
    {
        HashSet<string> result = new HashSet<string>();

        string[] markers = AssetDatabase.FindAssets("process_spritesheet_slice_animation t:TextAsset");

        foreach (string guid in markers)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string folder = Path.GetDirectoryName(path).Replace("\\", "/");
            result.Add(folder);
        }

        return result;
    }

    private bool IsInValidFolder(string assetPath, HashSet<string> validFolders)
    {
        assetPath = assetPath.Replace("\\", "/");

        foreach (var folder in validFolders)
        {
            if (assetPath.StartsWith(folder + "/"))
                return true;
        }

        return false;
    }

    private void ProcessTexture(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
            return;

        var factory = new SpriteDataProviderFactories();
        factory.Init();

        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        var existingRects = dataProvider.GetSpriteRects();

        // already sliced
        if (existingRects != null && existingRects.Length > 0)
            return;

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.isReadable = true;

        importer.SaveAndReimport();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);

        if (texture == null)
            return;

        string baseName = Path.GetFileNameWithoutExtension(path);

        string animationName = ExtractAnimationName(baseName);

        if (string.IsNullOrEmpty(animationName))
        {
            Debug.LogWarning($"Could not extract animation name from: {baseName}");
            return;
        }

        CharacterAnimationEntry entry = config.animations.FirstOrDefault(a =>
            a != null && a.name == animationName);

        if (entry == null)
        {
            Debug.LogWarning($"Animation config not found for: {animationName}");
            return;
        }

        int rows = texture.height / TileSize;

        int framesPerRow = entry.frames;

        var spriteRects = new List<SpriteRect>();

        int index = 0;

        // top -> bottom logical order
        for (int row = 0; row < rows; row++)
        {
            // texture coords are bottom-left
            int y = texture.height - ((row + 1) * TileSize);

            for (int frame = 0; frame < framesPerRow; frame++)
            {
                int x = frame * TileSize;

                // avoid overflow if texture smaller than expected
                if (x + TileSize > texture.width)
                    break;

                spriteRects.Add(new SpriteRect
                {
                    name = $"{baseName}_{index++}",
                    rect = new Rect(x, y, TileSize, TileSize),
                    alignment = SpriteAlignment.Center
                });
            }
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        dataProvider.Apply();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Debug.Log($"Sliced: {baseName} ({spriteRects.Count} sprites)");
    }

    private string ExtractAnimationName(string baseName)
    {
        var parts = baseName.Split('-');

        if (parts.Length == 0)
            return null;

        return parts[^1];
    }
}
