using UnityEngine;
using Aremoreno.Enums.Input;

public class OnScreenControlsManager : MonoBehaviour
{
    #region Fields

    [Header("Root")]
    [SerializeField] private GameObject onScreenControlsRoot;

    [Header("Directional Input GameObjects (for show/hide)")]
    [SerializeField] private GameObject joystickObject;
    [SerializeField] private GameObject dpadObject;

    [Header("Control Groups")]
    [SerializeField] private OnScreenControlGroup joystickGroup;
    [SerializeField] private OnScreenControlGroup dpadGroup;
    [SerializeField] private OnScreenControlGroup buttonsGroup;
    [SerializeField] private OnScreenControlGroup shoulderGroup;
    [SerializeField] private OnScreenControlGroup miscGroup;

    private DirectionalInputMode directionalInputMode = DirectionalInputMode.Dpad;
    public DirectionalInputMode DirectionalInputMode => directionalInputMode;

    // Collect all groups for bulk operations
    private OnScreenControlGroup[] allGroups;

    #endregion

    #region Lifecycle

    private void Awake()
    {
        allGroups = new[]
        {
            joystickGroup,
            dpadGroup,
            buttonsGroup,
            shoulderGroup,
            miscGroup
        };

        InputManager.Instance.RegisterScreenControls(onScreenControlsRoot);
        ShowDpadOnly();
    }

    private void OnDestroy()
    {
        InputManager.Instance?.UnregisterScreenControls();
    }

    private void OnEnable()
    {
        InputEvents.OnDirectionalInputModeChanged += HandleDirectionalInputModeChanged;
    }

    private void OnDisable()
    {
        InputEvents.OnDirectionalInputModeChanged -= HandleDirectionalInputModeChanged;
    }

    #endregion

    #region Per-Group Accessors

    // --- Joystick ---
    public void SetJoystickScale(float s) => joystickGroup?.SetScale(s);
    public void SetJoystickOpacity(float a) => joystickGroup?.SetOpacity(a);
    public void ResetJoystickScale() => joystickGroup?.ResetScale();
    public void ResetJoystickOpacity() => joystickGroup?.ResetOpacity();

    // --- Dpad ---
    public void SetDpadScale(float s) => dpadGroup?.SetScale(s);
    public void SetDpadOpacity(float a) => dpadGroup?.SetOpacity(a);
    public void ResetDpadScale() => dpadGroup?.ResetScale();
    public void ResetDpadOpacity() => dpadGroup?.ResetOpacity();

    // --- Buttons ---
    public void SetButtonsScale(float s) => buttonsGroup?.SetScale(s);
    public void SetButtonsOpacity(float a) => buttonsGroup?.SetOpacity(a);
    public void ResetButtonsScale() => buttonsGroup?.ResetScale();
    public void ResetButtonsOpacity() => buttonsGroup?.ResetOpacity();

    // --- Shoulder ---
    public void SetShoulderScale(float s) => shoulderGroup?.SetScale(s);
    public void SetShoulderOpacity(float a) => shoulderGroup?.SetOpacity(a);
    public void ResetShoulderScale() => shoulderGroup?.ResetScale();
    public void ResetShoulderOpacity() => shoulderGroup?.ResetOpacity();

    // --- Misc ---
    public void SetMiscScale(float s) => miscGroup?.SetScale(s);
    public void SetMiscOpacity(float a) => miscGroup?.SetOpacity(a);
    public void ResetMiscScale() => miscGroup?.ResetScale();
    public void ResetMiscOpacity() => miscGroup?.ResetOpacity();

    #endregion

    #region Bulk Operations

    /// <summary>Set opacity for ALL groups at once (e.g., from a global slider).</summary>
    public void SetAllOpacity(float alpha)
    {
        foreach (var g in allGroups)
            g?.SetOpacity(alpha);
    }

    /// <summary>Set scale for ALL groups at once.</summary>
    public void SetAllScale(float scale)
    {
        foreach (var g in allGroups)
            g?.SetScale(scale);
    }

    public void ResetAll()
    {
        foreach (var g in allGroups)
            g?.ResetAll();
    }

    #endregion

    #region Visibility

    // input manager

    #endregion

    #region Directional Input Mode

    private void SetInputMode(DirectionalInputMode mode)
    {
        directionalInputMode = mode;
        ApplyInputMode(mode);
    }

    public void ShowJoystickOnly() => SetInputMode(DirectionalInputMode.Joystick);
    public void ShowDpadOnly() => SetInputMode(DirectionalInputMode.Dpad);
    public void ShowBothDirectionalInput() => SetInputMode(DirectionalInputMode.Both);

    private void ApplyInputMode(DirectionalInputMode mode)
    {
        joystickObject.SetActive(mode == DirectionalInputMode.Joystick || mode == DirectionalInputMode.Both);
        dpadObject.SetActive(mode == DirectionalInputMode.Dpad || mode == DirectionalInputMode.Both);
    }

    private void HandleDirectionalInputModeChanged(DirectionalInputMode mode)
    {
        SetInputMode(mode);
    }

    #endregion
}
