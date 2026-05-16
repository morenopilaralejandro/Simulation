using UnityEngine;
using UnityEditor;
using UnityEngine.U2D.Animation;
using System.Collections.Generic;
using System.Linq;

public class SpriteLibraryAutoBuilder : EditorWindow
{
    string outputFolder = "Assets/Addressables/AddressSpriteLibrary";

    static readonly string[] Directions =
    {
        "up",
        "left",
        "down",
        "right"
    };

    // frames per direction (4-dir animations)
    static readonly Dictionary<string, int> Animations = new()
    {
        { "1h_backslash", 13 },
        { "combat", 2 },
        { "emote", 3 },
        { "idle", 2 },
        { "jump", 5 },
        { "run", 8 },
        { "slash", 6 },
        { "spellcast", 7 },
        { "walk", 9 }
    };

    // single-direction animations (always down)
    static readonly Dictionary<string, int> DownOnlyAnimations = new()
    {
        { "hurt", 6 }
    };

    [MenuItem("Tools/Spritesheet/Build Sprite Libraries")]
    public static void Open()
    {
        GetWindow<SpriteLibraryAutoBuilder>("Sprite Library Builder");
    }

    void OnGUI()
    {
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);

        if (GUILayout.Button("Build"))
            Build();
    }

    class SpriteData
    {
        public string libraryKey;
        public string category;
        public string label;
        public Sprite sprite;
    }

    int ExtractDirectionIndex(string name)
    {
        if (name.Contains("_up_")) return 0;
        if (name.Contains("_left_")) return 1;
        if (name.Contains("_down_")) return 2;
        if (name.Contains("_right_")) return 3;
        return 0;
    }

    int ExtractFrame(string name)
    {
        var parts = name.Split('_');
        if (!int.TryParse(parts[^1], out int frame))
            return -1;
        return frame;
    }

    void Build()
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

        foreach (var s in sprites)
        {
            if (!TryParse(s.name, s, out var data))
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

    // ---------------- PARSER ----------------
    bool TryParse(string name, Sprite sprite, out SpriteData data)
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

    // ---------------- ANIMATION SYSTEM ----------------
    bool TryParseAnimation(string anim, out string category, out string label)
    {
        category = null;
        label = null;

        var parts = anim.Split('_');
        if (parts.Length < 2) return false;

        if (!int.TryParse(parts[^1], out int index))
            return false;

        category = string.Join("_", parts.Take(parts.Length - 1));

        // ---------------- DOWN ONLY ----------------
        if (DownOnlyAnimations.TryGetValue(category, out int downFrames))
        {
            int frame = index;

            if (frame < 0 || frame >= downFrames)
                return false;

            // ALWAYS keep direction = down
            label = $"{category}_down_{frame}";
            return true;
        }

        // ---------------- NORMAL 4-DIR ----------------
        if (!Animations.TryGetValue(category, out int framesPerDir))
            return false;

        int dirIndex = index / framesPerDir;
        int frameIndex = index % framesPerDir;

        if (dirIndex < 0 || dirIndex >= Directions.Length)
            return false;

        string dir = Directions[dirIndex];

        label = $"{category}_{dir}_{frameIndex}";
        return true;
    }

    // ---------------- BUILDER ----------------
    void CreateLibrary(string key, List<SpriteData> data)
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

        var grouped = data
            .GroupBy(x => x.category)
            .OrderBy(g => g.Key);

        int i = 0;

        foreach (var cat in grouped)
        {
            labels.InsertArrayElementAtIndex(i);

            var categoryProp = labels.GetArrayElementAtIndex(i);
            categoryProp.FindPropertyRelative("m_Name").stringValue = cat.Key;

            var list = categoryProp.FindPropertyRelative("m_CategoryList");
            list.ClearArray();

            int j = 0;

            foreach (var s in cat.OrderBy(x => x.label))
            {
                list.InsertArrayElementAtIndex(j);

                var entry = list.GetArrayElementAtIndex(j);

                entry.FindPropertyRelative("m_Name").stringValue = s.label;
                entry.FindPropertyRelative("m_Sprite").objectReferenceValue = s.sprite;

                j++;
            }

            i++;
        }

        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(asset);

        Debug.Log($"Created: {path}");
    }
}
