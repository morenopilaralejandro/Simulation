using UnityEngine;
using UnityEngine.UI;
using Simulation.Enums.Input;

/// <summary>
/// Manages the scaling and opacity of the joystick and buttons.
/// </summary>

public class OnScreenControlsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject onScreenControlsRoot;

    [SerializeField] private GameObject joystickObject;
    [SerializeField] private GameObject dpadObject;

    [SerializeField] private RectTransform onScreenJoystick;
    [SerializeField] private RectTransform onScreenButtons;

    [Header("Default Scales")]
    [SerializeField] private float joystickDefaultScale = 0.18f;
    [SerializeField] private float buttonsDefaultScale = 0.2f;
    [SerializeField] private float joystickDefaultOpacity = 1f;
    [SerializeField] private float buttonsDefaultOpacity = 1f;

    [Header("Current Settings (Runtime)")]
    [Range(0.01f, 0.5f)] public float joystickScale = 0.18f;
    [Range(0.01f, 0.5f)] public float buttonsScale = 0.2f;
    [Range(0f, 1f)] public float joystickOpacity = 1f;
    [Range(0f, 1f)] public float buttonsOpacity = 1f;

    private DirectionalInputMode directionalInputMode = DirectionalInputMode.Dpad;
    public DirectionalInputMode DirectionalInputMode => directionalInputMode;


    private void Awake()
    {
        if (onScreenJoystick == null) 
            onScreenJoystick = transform.Find("OnScreenJoystick")?.GetComponent<RectTransform>();
        
        if (onScreenButtons == null) 
            onScreenButtons = transform.Find("OnScreenButtons")?.GetComponent<RectTransform>();

        InputManager.Instance.RegisterScreenControls(onScreenControlsRoot);
        ShowDpadOnly();
    }

    private void OnDestroy() 
    {
        InputManager.Instance?.UnregisterScreenControls();
    }

    #region Scale Controls
    public void SetJoystickScale(float scale)
    {
        joystickScale = scale;
        if (onScreenJoystick != null)
            onScreenJoystick.localScale = Vector3.one * scale;
    }

    public void ResetJoystickScale() => SetJoystickScale(joystickDefaultScale);

    public void SetButtonsScale(float scale)
    {
        buttonsScale = scale;
        if (onScreenButtons != null)
            onScreenButtons.localScale = Vector3.one * scale;
    }

    public void ResetButtonsScale() => SetButtonsScale(buttonsDefaultScale);
    #endregion

    #region Opacity Controls
    public void SetJoystickOpacity(float alpha)
    {
        joystickOpacity = alpha;
        if (onScreenJoystick != null)
            SetChildrenImageAlpha(onScreenJoystick, alpha);
    }

    public void ResetJoystickOpacity() => SetJoystickOpacity(joystickDefaultOpacity);

    public void SetButtonsOpacity(float alpha)
    {
        buttonsOpacity = alpha;
        if (onScreenButtons != null)
            SetChildrenImageAlpha(onScreenButtons, alpha);
    }

    public void ResetButtonsOpacity() => SetButtonsOpacity(buttonsDefaultOpacity);


    private void SetChildrenImageAlpha(Transform parent, float alpha)
    {
        foreach (var image in parent.GetComponentsInChildren<Image>())
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
    #endregion

    #region ResetAll
    public void ResetAll()
    {
        ResetJoystickScale();
        ResetButtonsScale();
        ResetJoystickOpacity();
        ResetButtonsOpacity();
    }
    #endregion
    
    #region Visibility
    public void HideOnScreenControls() 
    {
        onScreenControlsRoot.SetActive(false);
    }

    public void ShowOnScreenControls() 
    {
        if (InputManager.Instance)
            InputManager.Instance.UpdateOnScreenVisibility();            
    }
    #endregion

    #region Directional Input Mode

    /// <summary>
    /// Switch between Joystick, DPad, or Both at runtime.
    /// Call this from gameplay code when the context changes.
    /// </summary>
    private void SetInputMode(DirectionalInputMode mode)
    {
        directionalInputMode = mode;
        ApplyInputMode(mode);
    }

    /// <summary>Convenience shortcut – show only the joystick.</summary>
    public void ShowJoystickOnly() => SetInputMode(DirectionalInputMode.Joystick);

    /// <summary>Convenience shortcut – show only the D-Pad.</summary>
    public void ShowDpadOnly() => SetInputMode(DirectionalInputMode.Dpad);

    /// <summary>Convenience shortcut – show both controls.</summary>
    public void ShowBothDirectionalInput() => SetInputMode(DirectionalInputMode.Both);

    private void ApplyInputMode(DirectionalInputMode mode)
    {
        switch (mode)
        {
            case DirectionalInputMode.Joystick:
                joystickObject.SetActive(true);
                dpadObject.SetActive(false);
                break;

            case DirectionalInputMode.Dpad:
                joystickObject.SetActive(false);
                dpadObject.SetActive(true);
                break;

            case DirectionalInputMode.Both:
                joystickObject.SetActive(true);
                dpadObject.SetActive(true);
                break;
        }
    }

    #endregion

    #region Event

    private void OnEnable() 
    {
        InputEvents.OnDirectionalInputModeChanged += HandleDirectionalInputModeChanged;
    }

    private void OnDisable() 
    {
        InputEvents.OnDirectionalInputModeChanged -= HandleDirectionalInputModeChanged;
    }

    private void HandleDirectionalInputModeChanged(DirectionalInputMode directionalInputMode) 
    {
        SetInputMode(directionalInputMode);
    }

    #endregion
}
