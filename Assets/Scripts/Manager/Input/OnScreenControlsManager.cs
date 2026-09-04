using UnityEngine;
using Aremoreno.Enums.Battle;
using Aremoreno.Enums.Input;

public class OnScreenControlsManager : MonoBehaviour
{
    #region Fields

    [Header("Root")]
    [SerializeField] private GameObject onScreenControlsRoot;
    [SerializeField] private CanvasGroup canvasGroup;

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

    private bool isVisibleContextual = false;
    private bool isBattleContextual = false;

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

        //ShowDpadOnly();
    }

    private void OnDestroy()
    {

    }

    private void OnEnable()
    {
        InputEvents.OnDirectionalInputModeChanged += HandleDirectionalInputModeChanged;
        InputEvents.OnScreenControlsShowRequested += HandleScreenControlsShowRequested;
        InputEvents.OnScreenControlsHideRequested += HandleScreenControlsHideRequested;
        InputEvents.OnDeviceTypeChanged += HandleDeviceTypeChanged;

        UIEvents.OnMenuTeamBattleRequested += HandleMenuTeamBattleRequested;
        UIEvents.OnBackFromTeamRequested += HandleBackFromTeamRequested;
        BattleEvents.OnBattleStarted += HandleBattleStarted;
        BattleEvents.OnBattleEnded += HandleBattleEnded;
        TeamEvents.OnTeamPreviewEnded += HandleTeamPreviewEnded;
        UIEvents.OnMenuSideClosed += HandleMenuSideClosed;
        UIEvents.OnMenuSideOpened += HandleMenuSideOpened;
        DialogEvents.OnDialogStarted += HandleDialogStarted;
        DialogEvents.OnDialogEnded += HandleDialogEnded;
    }

    private void OnDisable()
    {
        InputEvents.OnDirectionalInputModeChanged -= HandleDirectionalInputModeChanged;
        InputEvents.OnScreenControlsShowRequested -= HandleScreenControlsShowRequested;
        InputEvents.OnScreenControlsHideRequested -= HandleScreenControlsHideRequested;
        InputEvents.OnDeviceTypeChanged -= HandleDeviceTypeChanged;

        UIEvents.OnMenuTeamBattleRequested -= HandleMenuTeamBattleRequested;
        UIEvents.OnBackFromTeamRequested -= HandleBackFromTeamRequested;
        BattleEvents.OnBattleStarted -= HandleBattleStarted;
        BattleEvents.OnBattleEnded -= HandleBattleEnded;
        TeamEvents.OnTeamPreviewEnded -= HandleTeamPreviewEnded;
        UIEvents.OnMenuSideClosed -= HandleMenuSideClosed;
        UIEvents.OnMenuSideOpened -= HandleMenuSideOpened;
        DialogEvents.OnDialogStarted -= HandleDialogStarted;
        DialogEvents.OnDialogEnded -= HandleDialogEnded;
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

    #region Visibility 

    private bool shouldShow => isVisibleContextual && InputManager.Instance.IsAndroid;

    private void HandleScreenControlsHideRequested() 
    {
        isVisibleContextual = false;
        //SetCanvasGroupVisible(false);
        SetControlsVisible(false);
    }

    private void HandleScreenControlsShowRequested() 
    {
        //if (onScreenControlsRoot.activeSelf != true && isAndroid && !IsUsingController)
        isVisibleContextual = true;
        UpdateVisibility();
    }

    private void SetCanvasGroupVisible(bool visible)
    {
        canvasGroup.alpha = visible ? 1f : 0f;
        canvasGroup.interactable = visible;
        canvasGroup.blocksRaycasts = visible;
    }

    private void SetControlsVisible(bool visible)
    {
        onScreenControlsRoot.SetActive(visible);
    }

    private void HandleDeviceTypeChanged(InputDeviceType inputDeviceType) 
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        SetControlsVisible(shouldShow);
        //SetCanvasGroupVisible(shouldShow);
    }

    private void HandleMenuTeamBattleRequested(Team team) 
    {
        InputEvents.RaiseScreenControlsHideRequested();
    }

    private void HandleBackFromTeamRequested(Team currentTeam, bool hasSwapped) 
    {
        if (!isBattleContextual) return;
        InputEvents.RaiseScreenControlsShowRequested();
    }

    private void HandleBattleStarted(BattleType battleType) 
    {
        isBattleContextual = true;
        if (battleType == BattleType.Mini)
        {
            InputEvents.RaiseDirectionalInputModeChanged(DirectionalInputMode.Joystick);
            InputEvents.RaiseScreenControlsShowRequested();
        } else 
        {
            InputEvents.RaiseScreenControlsHideRequested();
        }
    }

    private void HandleBattleEnded() 
    {
        isBattleContextual = false;
        InputEvents.RaiseScreenControlsHideRequested();
    }

    private void HandleTeamPreviewEnded() 
    {
        InputEvents.RaiseDirectionalInputModeChanged(DirectionalInputMode.Joystick);
        InputEvents.RaiseScreenControlsShowRequested();
    }

    //shown on worldManager

    private void HandleMenuSideOpened() 
    {
        InputEvents.RaiseScreenControlsHideRequested();
    }

    private void HandleMenuSideClosed() 
    {
        InputEvents.RaiseScreenControlsShowRequested();
    }

    private void HandleDialogStarted()
    {
        InputEvents.RaiseScreenControlsHideRequested();
    }

    private void HandleDialogEnded()
    {
        InputEvents.RaiseScreenControlsShowRequested();
    }

    #endregion
}
