using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class AnimationClipGenerator : EditorWindow
{
    public CharacterAnimationConfig config;

    public string spritesFolder = "Assets/Sprites";
    public string outputFolder = "Assets/Animations/AnimationsCharacter";
    public float frameRate = 12f;

    [MenuItem("Tools/Animation/Generate Clips From Config")]
    public static void Open()
    {
        GetWindow<AnimationClipGenerator>("Clip Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Animation Clip Generator", EditorStyles.boldLabel);

        config = (CharacterAnimationConfig)EditorGUILayout.ObjectField(
            "Config",
            config,
            typeof(CharacterAnimationConfig),
            false
        );

        spritesFolder = EditorGUILayout.TextField("Sprites Folder", spritesFolder);
        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);

        if (GUILayout.Button("Generate Clips"))
        {
            if (config == null)
            {
                Debug.LogError("No config assigned.");
                return;
            }

            Generate();
        }
    }

    void Generate()
    {
        if (!AssetDatabase.IsValidFolder(outputFolder))
        {
            Directory.CreateDirectory(outputFolder);
            AssetDatabase.Refresh();
        }

        foreach (var anim in config.animations)
        {
            if (anim.direction == AnimationDirection.FourDir)
            {
                CreateDirectional(anim);
            }
            else
            {
                CreateDownOnly(anim);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Animation clips generated.");
    }

    void CreateDirectional(AnimationEntry anim)
    {
        string[] dirs = { "down", "up", "left", "right" };

        foreach (var dir in dirs)
        {
            CreateClip(anim.name, dir, anim.frames);
        }
    }

    void CreateDownOnly(AnimationEntry anim)
    {
        CreateClip(anim.name, "down", anim.frames);
    }

    void CreateClip(string animName, string dir, int frames)
    {
        List<Sprite> sprites = new List<Sprite>();

        for (int i = 0; i < frames; i++)
        {
            string spriteName = $"{animName}_{dir}_{i}";

            string[] guids = AssetDatabase.FindAssets(spriteName + " t:Sprite", new[] { spritesFolder });

            if (guids.Length == 0)
            {
                Debug.LogWarning($"Missing sprite: {spriteName}");
                continue;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (sprite != null)
                sprites.Add(sprite);
        }

        if (sprites.Count == 0)
        {
            Debug.LogWarning($"No frames found for {animName}_{dir}");
            return;
        }

        AnimationClip clip = new AnimationClip();
        clip.frameRate = frameRate;

        EditorCurveBinding binding = new EditorCurveBinding();
        binding.type = typeof(SpriteRenderer);
        binding.path = "";
        binding.propertyName = "m_Sprite";

        float step = 1f / frameRate;

        ObjectReferenceKeyframe[] keys = new ObjectReferenceKeyframe[sprites.Count];

        for (int i = 0; i < sprites.Count; i++)
        {
            keys[i] = new ObjectReferenceKeyframe();
            keys[i].time = i * step;
            keys[i].value = sprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);

        clip.wrapMode = WrapMode.Loop;

        string clipName = $"{animName}_{dir}";
        AssetDatabase.CreateAsset(clip, $"{outputFolder}/{clipName}.anim");
    }
}
