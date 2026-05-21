using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using System.Linq;
using Aremoreno.Enums.Animation;

public class SpriteLibraryAutoBuilder : EditorWindow
{
    [SerializeField] private CharacterAnimationConfig config;
    [SerializeField] private string outputFolder = "Assets/Addressables/AddressSpriteLibrary";

    private static readonly string[] Directions =
    {
        "up",
        "left",
        "down",
        "right"
    };

    [MenuItem("Tools/Spritesheet/Build Sprite Libraries")]
    public static void Open()
    {
        GetWindow<SpriteLibraryAutoBuilder>("Sprite Library Builder");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Library Builder", EditorStyles.boldLabel);

        config = (CharacterAnimationConfig)EditorGUILayout.ObjectField(
            "Config",
            config,
            typeof(CharacterAnimationConfig),
            false
        );

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        EditorGUILayout.Space();

        if (GUILayout.Button("Build"))
        {
            if (config == null)
            {
                Debug.LogError("CharacterAnimationConfig missing.");
                return;
            }

            Build();
        }
    }

    class SpriteData
    {
        public string libraryKey;
        public string category;
        public string label;
        public Sprite sprite;
    }

    private int ExtractDirectionIndex(string name)
    {
        if (name.Contains("_up_")) return 0;
        if (name.Contains("_left_")) return 1;
        if (name.Contains("_down_")) return 2;
        if (name.Contains("_right_")) return 3;
        return 0;
    }

    private int ExtractFrame(string name)
    {
        var parts = name.Split('_');
        return int.TryParse(parts[^1], out int frame) ? frame : -1;
    }

    private void Build()
    {
        var sprites = AssetDatabase.FindAssets("t:Sprite")
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p))
            .OfType<Sprite>()
            .Where(s => s != null)
            .OrderBy(s => ExtractDirectionIndex(s.name))
            .ThenBy(s => ExtractFrame(s.name))
            .ToList();

        var map = new Dictionary<string, List<SpriteData>>();

        foreach (var sprite in sprites)
        {
            if (!TryParse(sprite.name, sprite, out var data))
                continue;

            if (!map.ContainsKey(data.libraryKey))
                map[data.libraryKey] = new List<SpriteData>();

            map[data.libraryKey].Add(data);
        }

        foreach (var kv in map)
            CreateLibrary(kv.Key, kv.Value);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Built {map.Count} SpriteLibraries");
    }

    private bool TryParse(string name, Sprite sprite, out SpriteData data)
    {
        data = null;

        if (!name.StartsWith("sprite-character-"))
            return false;

        string body = name.Substring("sprite-character-".Length);

        int firstDash = body.IndexOf('-');
        if (firstDash < 0) return false;

        string type = body.Substring(0, firstDash);
        string rest = body.Substring(firstDash + 1);

        var parts = rest.Split('-');
        if (parts.Length < 2) return false;

        string animationPart = parts[^1];

        if (!TryParseAnimation(animationPart, out string category, out string label))
            return false;

        string libraryKey;

        if (type == "body")
        {
            string id = string.Join("-", parts.Take(parts.Length - 1));
            libraryKey = $"library-body-{id}";
        }
        else if (type == "kit")
        {
            if (parts.Length < 4) return false;

            string id = string.Join("-", parts.Take(parts.Length - 3));
            string variant = parts[^3];
            string role = parts[^2];

            libraryKey = $"library-kit-{id}-{variant}-{role}";
        }
        else if (type == "wings" || type == "hair")
        {
            if (parts.Length < 3) return false;

            string id = string.Join("-", parts.Take(parts.Length - 2));
            string position = parts[^2];

            libraryKey = $"library-{type}-{id}-{position}";
        }
        else
        {
            return false;
        }

        data = new SpriteData
        {
            libraryKey = libraryKey,
            category = category,
            label = label,
            sprite = sprite
        };

        return true;
    }

    private bool TryParseAnimation(string anim, out string category, out string label)
    {
        category = null;
        label = null;

        var parts = anim.Split('_');
        if (parts.Length < 2) return false;

        if (!int.TryParse(parts[^1], out int index))
            return false;

        category = string.Join("_", parts.Take(parts.Length - 1));

        string animationName = category;

        CharacterAnimationEntry entry = config.animations.FirstOrDefault(a =>
            a != null && a.name == animationName);

        if (entry == null)
            return false;

        // DownOnly animation
        if (entry.direction == CharacterAnimationEntryDirection.DownOnly)
        {
            if (index < 0 || index >= entry.frames)
                return false;

            label = $"{category}_down_{index}";
            return true;
        }

        // FourDir animation
        int dirIndex = index / entry.frames;
        int frameIndex = index % entry.frames;

        if (dirIndex < 0 || dirIndex >= Directions.Length)
            return false;

        string dir = Directions[dirIndex];
        label = $"{category}_{dir}_{frameIndex}";
        return true;
    }

    private void CreateLibrary(string key, List<SpriteData> data)
    {
        string path = $"{outputFolder}/{key}.asset".Replace("\\", "/");

        var existing = AssetDatabase.LoadAssetAtPath<SpriteLibraryAsset>(path);
        if (existing != null)
            AssetDatabase.DeleteAsset(path);

        var asset = ScriptableObject.CreateInstance<SpriteLibraryAsset>();
        AssetDatabase.CreateAsset(asset, path);

        var so = new SerializedObject(asset);
        var labels = so.FindProperty("m_Labels");
        labels.ClearArray();

        var grouped = data.GroupBy(x => x.category).OrderBy(g => g.Key);

        int i = 0;

        foreach (var category in grouped)
        {
            labels.InsertArrayElementAtIndex(i);

            var categoryProp = labels.GetArrayElementAtIndex(i);
            categoryProp.FindPropertyRelative("m_Name").stringValue = category.Key;

            var list = categoryProp.FindPropertyRelative("m_CategoryList");
            list.ClearArray();

            int j = 0;

            foreach (var sprite in category.OrderBy(x => x.label))
            {
                list.InsertArrayElementAtIndex(j);

                var entry = list.GetArrayElementAtIndex(j);
                entry.FindPropertyRelative("m_Name").stringValue = sprite.label;
                entry.FindPropertyRelative("m_Sprite").objectReferenceValue = sprite.sprite;

                j++;
            }

            i++;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);

        Debug.Log($"Created: {path}");
    }
}
