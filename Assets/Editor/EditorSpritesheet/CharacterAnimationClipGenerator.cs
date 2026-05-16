using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class CharacterAnimationClipGenerator : EditorWindow
{
    [SerializeField] private CharacterAnimationConfig config;

    [Header("Output")]
    [SerializeField] private string outputFolder = "Assets/Animations/AnimationsCharacter";
    [SerializeField] private string clipPrefix = "ani-";

    [Header("Animation")]
    [SerializeField] private float frameRate = 12f;
    [SerializeField] private bool overwriteExisting = true;

    [Header("SpriteResolver Paths")]
    [SerializeField] private string bodyResolverPath = "Body";
    [SerializeField] private string kitResolverPath = "Kit";

    private static readonly string[] FourDirections =
    {
        "down",
        "up",
        "left",
        "right"
    };

    private static readonly string[] DownOnly =
    {
        "down"
    };

    [MenuItem("Tools/Animation/Generate Clips Character")]
    public static void Open()
    {
        GetWindow<CharacterAnimationClipGenerator>("Clip Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label(
            "Dual SpriteResolver Animation Generator",
            EditorStyles.boldLabel
        );

        config = (CharacterAnimationConfig)EditorGUILayout.ObjectField(
            "Config",
            config,
            typeof(CharacterAnimationConfig),
            false
        );

        EditorGUILayout.Space();

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        clipPrefix = EditorGUILayout.TextField("Clip Prefix", clipPrefix);

        EditorGUILayout.Space();

        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);
        overwriteExisting = EditorGUILayout.Toggle(
            "Overwrite Existing",
            overwriteExisting
        );

        EditorGUILayout.Space();

        bodyResolverPath = EditorGUILayout.TextField(
            "Body Resolver Path",
            bodyResolverPath
        );

        kitResolverPath = EditorGUILayout.TextField(
            "Kit Resolver Path",
            kitResolverPath
        );

        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Generates AnimationClips that animate TWO SpriteResolvers.\n\n" +
            "Example labels:\n" +
            "run_down_0\n" +
            "run_down_1\n\n" +
            "Both resolvers receive synchronized label animation.",
            MessageType.Info
        );

        if (GUILayout.Button("Generate Clips"))
        {
            if (config == null)
            {
                Debug.LogError("Missing CharacterAnimationConfig.");
                return;
            }

            Generate();
        }
    }

    private void Generate()
    {
        EnsureFolderExists(outputFolder);

        foreach (var anim in config.animations)
        {
            if (anim == null || string.IsNullOrWhiteSpace(anim.name))
                continue;

            string[] dirs = anim.direction == AnimationDirection.FourDir
                ? FourDirections
                : DownOnly;

            foreach (string dir in dirs)
            {
                CreateClip(anim.name, dir, anim.frames);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Dual SpriteResolver clips generated.");
    }

    private void CreateClip(string animName, string dir, int frames)
    {
        if (frames <= 0)
            return;

        string clipName = $"{clipPrefix}{animName}_{dir}";
        string assetPath = Path.Combine(outputFolder, clipName + ".anim");

        if (overwriteExisting &&
            AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath) != null)
        {
            AssetDatabase.DeleteAsset(assetPath);
        }
        else
        {
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
        }

        AnimationClip clip = new AnimationClip
        {
            frameRate = frameRate,
            wrapMode = WrapMode.Loop
        };

        float step = 1f / Mathf.Max(frameRate, 0.0001f);

        Keyframe[] keys = new Keyframe[frames];

        for (int i = 0; i < frames; i++)
        {
            keys[i] = new Keyframe(i * step, i);
        }

        AnimationCurve curve = new AnimationCurve(keys);

        // BODY RESOLVER
        EditorCurveBinding bodyBinding = new EditorCurveBinding
        {
            type = typeof(SpriteResolver),
            path = bodyResolverPath,
            propertyName = "m_Label"
        };

        // Kit RESOLVER
        EditorCurveBinding kitBinding = new EditorCurveBinding
        {
            type = typeof(SpriteResolver),
            path = kitResolverPath,
            propertyName = "m_Label"
        };

        AnimationUtility.SetEditorCurve(
            clip,
            bodyBinding,
            curve
        );

        AnimationUtility.SetEditorCurve(
            clip,
            kitBinding,
            curve
        );

        AssetDatabase.CreateAsset(clip, assetPath);

        EditorUtility.SetDirty(clip);

        Debug.Log($"Created clip: {clipName}");
    }

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string parent = Path.GetDirectoryName(folderPath)
            ?.Replace("\\", "/");

        string name = Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
        {
            EnsureFolderExists(parent);
        }

        AssetDatabase.CreateFolder(parent, name);
    }
}
