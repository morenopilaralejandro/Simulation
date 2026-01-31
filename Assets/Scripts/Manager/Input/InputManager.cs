using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Simulation.Enums.Input;
using Simulation.Enums.Duel;

///<summary>
/// Handles input with Unity New Input System. 
/// Supports keyboard, gamepad and touch screen.
/// Usage: InputManager.Instance.GetDown(CustomAction.Pass))
///</summary>
public class InputManager : MonoBehaviour
{
    #region Fields
    public static InputManager Instance { get; private set; }

    private GameInputActions input;   
    private Vector2 move;
    private readonly ButtonState[] buttons = new ButtonState[System.Enum.GetValues(typeof(CustomAction)).Length];
    private readonly Dictionary<InputAction, CustomAction> actionLookup = new();
    private GameObject onScreenControlsRoot;
    private Camera mainCamera;
    private float bufferDurationShoot = 1f;
    private bool isAndroid;
    private bool isLocked;
    [SerializeField] private PlayerInput playerInput;

    public bool IsAndroid => isAndroid;
    public bool IsLocked => isLocked;
    #endregion

    #region Lifecycle
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        input = new GameInputActions();

        //BattleActions

        // Movement
        input.BattleActions.Battle_Move.performed += OnMovePerformed;
        input.BattleActions.Battle_Move.canceled += _ => move = Vector2.zero;

        // Buttons
        BindButton(input.BattleActions.Battle_Pass, CustomAction.Battle_Pass);
        BindButton(input.BattleActions.Battle_Shoot, CustomAction.Battle_Shoot);
        BindButton(input.BattleActions.Battle_ChangeManual, CustomAction.Battle_ChangeManual);
        BindButton(input.BattleActions.Battle_ChangeAuto, CustomAction.Battle_ChangeAuto);
        BindButton(input.BattleActions.Battle_Dribble, CustomAction.Battle_Dribble);
        BindButton(input.BattleActions.Battle_Block, CustomAction.Battle_Block);

        //BattleUIActions
        BindButton(input.BattleUIActions.BattleUI_ClickEastButton, CustomAction.BattleUI_ClickEastButton);
        BindButton(input.BattleUIActions.BattleUI_ClickWestButton, CustomAction.BattleUI_ClickWestButton);
        BindButton(input.BattleUIActions.BattleUI_ClickNorthButton, CustomAction.BattleUI_ClickNorthButton);
        BindButton(input.BattleUIActions.BattleUI_ClickSouthButton, CustomAction.BattleUI_ClickSouthButton);
        BindButton(input.BattleUIActions.BattleUI_CloseMoveMenu, CustomAction.BattleUI_CloseMoveMenu);
        BindButton(input.BattleUIActions.BattleUI_NextMove, CustomAction.BattleUI_NextMove);
        BindButton(input.BattleUIActions.BattleUI_OpenBattleMenu, CustomAction.BattleUI_OpenBattleMenu);
        BindButton(input.BattleUIActions.BattleUI_CloseBattleMenu, CustomAction.BattleUI_CloseBattleMenu);
        BindButton(input.BattleUIActions.BattleUI_OpenDimensionMenu, CustomAction.BattleUI_OpenDimensionMenu);
        BindButton(input.BattleUIActions.BattleUI_CloseDimensionMenu, CustomAction.BattleUI_CloseDimensionMenu);
        BindButton(input.BattleUIActions.BattleUI_DimensionShortcutPause, CustomAction.BattleUI_DimensionShortcutPause);


        // Enable once here
        input.BattleActions.Enable();
        input.BattleUIActions.Enable();

        // Respond to control scheme changes (requires a PlayerInput component on the same GO)
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.onControlsChanged += OnControlsChanged;
        }

        DuelEvents.OnDuelStart += HandleDuelStart;
    }

    private void Start() 
    {
        mainCamera = Camera.main;
    }

    private void OnDestroy()
    {
        DuelEvents.OnDuelStart += HandleDuelStart;

        if (input != null)
        {
            input.BattleActions.Battle_Move.performed -= OnMovePerformed;

            UnbindButton(input.BattleActions.Battle_Pass);
            UnbindButton(input.BattleActions.Battle_Shoot);
            UnbindButton(input.BattleActions.Battle_ChangeManual);
            UnbindButton(input.BattleActions.Battle_ChangeAuto);
            UnbindButton(input.BattleActions.Battle_Dribble);
            UnbindButton(input.BattleActions.Battle_Block);

            input.BattleActions.Disable();
            input.Dispose();
        }

        if (playerInput != null)
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }

        if (Instance == this) Instance = null;
    }

    public void RegisterScreenControls(GameObject screenControls) 
    {
        onScreenControlsRoot = screenControls;
        UpdateOnScreenVisibility();
    }

    public void UnregisterScreenControls()
    {
        onScreenControlsRoot = null;
    }

    private void Update()
    {
        // Accumulate hold time for held buttons
        float deltaTime = Time.unscaledDeltaTime;
        for (int i = 0; i < System.Enum.GetValues(typeof(CustomAction)).Length; i++)
        {
            if (buttons[i].Held)
            {
                var button = buttons[i];
                button.HoldTime += deltaTime;
                buttons[i] = button;
            }
        }
    }
    #endregion

    #region Binding helpers
    private void BindButton(InputAction inputAction, CustomAction customAction)
    {
        actionLookup[inputAction] = customAction;
        inputAction.started += OnAnyButtonStarted;
        inputAction.canceled += OnAnyButtonCanceled;
    }

    private void UnbindButton(InputAction inputAction)
    {
        if (inputAction == null) return;
            inputAction.started -= OnAnyButtonStarted;
            inputAction.canceled -= OnAnyButtonCanceled;
            actionLookup.Remove(inputAction);
    }
    #endregion

    #region Callbacks
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        move = ctx.ReadValue<Vector2>();
    }

    private void OnAnyButtonStarted(InputAction.CallbackContext ctx)
    {
        if (actionLookup.TryGetValue(ctx.action, out var customAction))
            OnButtonDown(customAction);
    }

    private void OnAnyButtonCanceled(InputAction.CallbackContext ctx)
    {
        if (actionLookup.TryGetValue(ctx.action, out var customAction))
            OnButtonUp(customAction);
    }

    private void OnButtonDown(CustomAction customAction)
    {
        LogManager.Trace($"[InputManager] OnButtonDown: {customAction}");
        var button = buttons[(int)customAction];

        float bufferDuration = 
            (customAction == CustomAction.Battle_Shoot) ? 
            bufferDurationShoot : 0f;

        if (!BattleManager.Instance.IsTimeFrozen)
            button.SetDown(bufferDuration);
        else
            button.SetDown(0f);

        buttons[(int)customAction] = button;
    }

    private void OnButtonUp(CustomAction customAction)
    {
        var button = buttons[(int)customAction];
        button.SetUp();
        buttons[(int)customAction] = button;
    }
    #endregion

    #region Event Handler
    private void HandleDuelStart(DuelMode duelMode) => InvalidateAllBuffers();
    private void InvalidateAllBuffers()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var b = buttons[i];
            if (b.BufferedUntil > 0)
                b.BufferedUntil = 0;
            buttons[i] = b;
        }
    }
    #endregion

    #region API
    public Vector2 GetMove() => move;
    public Vector2 GetMouse() => Mouse.current.position.ReadValue();
    public bool GetDown(CustomAction customAction) => buttons[(int)customAction].DownFrame == (uint)Time.frameCount;
    public bool GetHeld(CustomAction customAction) => buttons[(int)customAction].Held;
    public bool GetUp(CustomAction customAction) => buttons[(int)customAction].UpFrame == (uint)Time.frameCount;

    public bool ConsumeBuffered(CustomAction customAction, out bool wasBuffered)
    {
        var button = buttons[(int)customAction];
        wasBuffered = false;

        // Determine if we can consume a press
        if (button.IsBuffered() || button.DownFrame == (uint)Time.frameCount)
        {
            wasBuffered = button.IsBuffered() && button.DownFrame != (uint)Time.frameCount;
            button.ConsumedDown = true;
            button.WasBufferedConsume = wasBuffered;

            buttons[(int)customAction] = button;
            return true;
        }

        return false;
    }

    public Vector3 ConvertToWorldPositionOnGround(Vector2 position)
    {
        Ray ray = mainCamera.ScreenPointToRay(position);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        if (groundPlane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);
        return transform.position;
    }
    #endregion

    #region Scheme helpers
    private void OnControlsChanged(PlayerInput playerInput)
    {
        LogManager.Trace($"[InputManager] OnControlsChanged: {playerInput.currentControlScheme}");
        UpdateOnScreenVisibility();
    }

    public void UpdateOnScreenVisibility()
    {
        isAndroid = Application.platform == RuntimePlatform.Android;
        if (onScreenControlsRoot && onScreenControlsRoot.activeSelf != isAndroid)
            onScreenControlsRoot.SetActive(isAndroid);
    }
    #endregion

    #region Lock
    public void LockInput()
    {
        if (isLocked) return;
        isLocked = true;

        if (playerInput != null)
            playerInput.DeactivateInput();

        // Clear buffered inputs so nothing fires on unlock
        InvalidateAllBuffers();
        move = Vector2.zero;

        LogManager.Trace("[InputManager] Input locked");
    }

    public void UnlockInput()
    {
        if (!isLocked) return;
        isLocked = false;

        // Re-enable input actions
        if (playerInput != null)
            playerInput.ActivateInput();

        LogManager.Trace("[InputManager] Input unlocked");
    }
    #endregion
}
