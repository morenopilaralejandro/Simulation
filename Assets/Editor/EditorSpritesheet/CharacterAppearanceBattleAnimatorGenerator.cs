using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.U2D.Animation;
using Aremoreno.Enums.Character;

public class CharacterAppearanceBattleAnimatorGenerator : EditorWindow
{
    [SerializeField] private CharacterAnimationConfig config;

    [Header("Output")]
    [SerializeField] private string outputFolder = "Assets/Animations/AnimationsCharacter";
    [SerializeField] private string controllerName = "CharacterAppearanceBattle.controller";
    [SerializeField] private string clipPrefix = "ani-";

    [Header("Animation")]
    [SerializeField] private float frameRate = 12f;

    [Header("Resolver Paths")]
    [SerializeField] private string bodyPath = "Body";
    [SerializeField] private string kitPath = "Kit";
    [SerializeField] private string wingsPath = "Wings";

    private static readonly string[] FourDirections = { "down", "up", "left", "right" };
    private static readonly string[] DownOnly = { "down" };

    private static readonly int StateHash = Animator.StringToHash("State");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");

    [MenuItem("Tools/Animation/Generate CharacterAppearanceBattle Animator")]
    public static void Open()
    {
        GetWindow<CharacterAppearanceBattleAnimatorGenerator>("Battle Animator Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("CharacterAppearanceBattle Animator Generator", EditorStyles.boldLabel);

        config = (CharacterAnimationConfig)EditorGUILayout.ObjectField(
            "Config", config, typeof(CharacterAnimationConfig), false);

        EditorGUILayout.Space();

        outputFolder = EditorGUILayout.TextField("Output Folder", outputFolder);
        controllerName = EditorGUILayout.TextField("Controller Name", controllerName);
        clipPrefix = EditorGUILayout.TextField("Clip Prefix", clipPrefix);

        EditorGUILayout.Space();

        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);

        EditorGUILayout.Space();

        bodyPath = EditorGUILayout.TextField("Body Path", bodyPath);
        kitPath = EditorGUILayout.TextField("Kit Path", kitPath);
        wingsPath = EditorGUILayout.TextField("Wings Path", wingsPath);

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate"))
        {
            if (config == null)
            {
                Debug.LogError("CharacterAnimationConfig missing.");
                return;
            }

            Generate();
        }
    }

    private void Generate()
    {
        EnsureFolderExists(outputFolder);

        string controllerPath = Path.Combine(outputFolder, controllerName).Replace("\\", "/");

        var existingController = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
        if (existingController != null)
            AssetDatabase.DeleteAsset(controllerPath);

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);

        EnsureParameter(controller, "State", AnimatorControllerParameterType.Int);
        EnsureParameter(controller, "Direction", AnimatorControllerParameterType.Int);

        AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
        AnimatorState defaultState = null;

        foreach (var anim in config.animations)
        {
            if (anim == null || string.IsNullOrWhiteSpace(anim.name))
                continue;

            if (!Enum.TryParse(anim.name, true, out CharacterAnimationState animState))
            {
                Debug.LogWarning(
                    $"Animation '{anim.name}' does not match a CharacterAnimationState enum name. " +
                    $"Transitions were not generated for it."
                );
                continue;
            }

            string[] dirs = anim.direction == AnimationDirection.FourDir
                ? FourDirections
                : DownOnly;

            foreach (string dir in dirs)
            {
                if (!Enum.TryParse(dir, true, out CharacterDirection animDirection))
                    continue;

                AnimationClip clip = CreateClip(anim.name, dir, anim.frames);

                AnimatorState state = stateMachine.AddState(GetStateName(anim.name, dir));
                state.motion = clip;

                if (defaultState == null)
                    defaultState = state;

                AnimatorStateTransition transition = stateMachine.AddAnyStateTransition(state);
                transition.hasExitTime = false;
                transition.hasFixedDuration = true;
                transition.duration = 0.05f;
                transition.offset = 0f;
                transition.canTransitionToSelf = false;

                transition.AddCondition(AnimatorConditionMode.Equals, (float)animState, "State");
                transition.AddCondition(AnimatorConditionMode.Equals, (float)animDirection, "Direction");
            }
        }

        if (defaultState != null)
            stateMachine.defaultState = defaultState;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("CharacterAppearanceBattle animator generated.");
    }

    private AnimationClip CreateClip(string animName, string dir, int frames)
    {
        string clipName = $"{clipPrefix}{animName}_{dir}";
        string clipPath = Path.Combine(outputFolder, clipName + ".anim").Replace("\\", "/");

        AnimationClip existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
        if (existing != null)
            AssetDatabase.DeleteAsset(clipPath);

        AnimationClip clip = new AnimationClip
        {
            frameRate = frameRate,
            wrapMode = WrapMode.Loop
        };

        AddResolverBinding(clip, bodyPath, animName, dir, frames);
        AddResolverBinding(clip, kitPath, animName, dir, frames);
        AddResolverBinding(clip, wingsPath, animName, dir, frames);

        AssetDatabase.CreateAsset(clip, clipPath);
        EditorUtility.SetDirty(clip);

        return clip;
    }

    private void AddResolverBinding(
        AnimationClip clip,
        string objectPath,
        string animName,
        string dir,
        int frames
    )
    {
        var binding = new EditorCurveBinding
        {
            type = typeof(SpriteResolverFrameDriver),
            path = objectPath,
            propertyName = "frame" // IMPORTANT: serialized field name, not property
        };

        AnimationCurve curve = new AnimationCurve();
        float step = 1f / frameRate;

        for (int i = 0; i < frames; i++)
        {
            curve.AddKey(i * step, i);
        }

        AnimationUtility.SetEditorCurve(clip, binding, curve);
    }

    private void MakeConstant(AnimationCurve curve)
    {
        for (int i = 0; i < curve.length; i++)
        {
            AnimationUtility.SetKeyLeftTangentMode(
                curve,
                i,
                AnimationUtility.TangentMode.Constant
            );

            AnimationUtility.SetKeyRightTangentMode(
                curve,
                i,
                AnimationUtility.TangentMode.Constant
            );
        }
    }

    private static void EnsureParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        foreach (var p in controller.parameters)
        {
            if (p.name == name)
                return;
        }

        controller.AddParameter(name, type);
    }

    private string GetStateName(string animName, string dir) => $"{animName}_{dir}";

    private static void EnsureFolderExists(string folderPath)
    {
        if (AssetDatabase.IsValidFolder(folderPath))
            return;

        string parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
        string name = Path.GetFileName(folderPath);

        if (!AssetDatabase.IsValidFolder(parent))
            EnsureFolderExists(parent);

        AssetDatabase.CreateFolder(parent, name);
    }
}
