using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using UnityEditor.U2D.Sprites;

public class SpritesheetAutoSlicerWindow : EditorWindow
{
    private const int TileSize = 64;

    private List<SpriteAtlas> selectedAtlases = new List<SpriteAtlas>();
    private bool isProcessing;

    private Vector2 scroll;

    [MenuItem("Tools/Spritesheet/Spritesheet Auto Slicer")]
    public static void Open()
    {
        GetWindow<SpritesheetAutoSlicerWindow>("Spritesheet Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Spritesheet Auto Slicer", EditorStyles.boldLabel);

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

        string[] markers = AssetDatabase.FindAssets("process_spritesheet_slice t:TextAsset");

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
        if (importer == null) return;

        // check slicing first
        var factory = new SpriteDataProviderFactories();
        factory.Init();

        var dataProvider = factory.GetSpriteEditorDataProviderFromObject(importer);
        dataProvider.InitSpriteEditorDataProvider();

        var existingRects = dataProvider.GetSpriteRects();
        if (existingRects != null && existingRects.Length > 0)
            return; // already sliced → DO NOTHING

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.isReadable = true;
        importer.SaveAndReimport();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        AssetDatabase.Refresh();

        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null) return;

        string baseName = Path.GetFileNameWithoutExtension(path);

        var rows = new List<List<Rect>>();

        for (int y = 0; y < texture.height; y += TileSize)
        {
            var row = new List<Rect>();

            for (int x = 0; x < texture.width; x += TileSize)
            {
                if (IsTileEmpty(texture, x, y))
                    continue;

                row.Add(new Rect(x, y, TileSize, TileSize));
            }

            if (row.Count > 0)
                rows.Add(row);
        }

        rows.Reverse();

        var spriteRects = new List<SpriteRect>();
        int index = 0;

        for (int r = 0; r < rows.Count; r++)
        {
            var row = rows[r];

            for (int c = 0; c < row.Count; c++)
            {
                spriteRects.Add(new SpriteRect
                {
                    name = $"{baseName}_{index++}",
                    rect = row[c],
                    alignment = SpriteAlignment.Center
                });
            }
        }

        dataProvider.SetSpriteRects(spriteRects.ToArray());
        dataProvider.Apply();

        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    private bool IsTileEmpty(Texture2D tex, int startX, int startY)
    {
        Color[] pixels = tex.GetPixels(startX, startY, TileSize, TileSize);

        for (int i = 0; i < pixels.Length; i++)
        {
            if (pixels[i].a > 0.01f)
                return false;
        }

        return true;
    }
}
