using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using Simulation.Enums.Input;

///<summary>
/// Handles input with Unity New Input System. 
/// Supports keyboard, gamepad and touch screen.
/// Usage: InputManager.Instance.GetDown(BattleAction.Pass))
///</summary>
public class InputManager : MonoBehaviour
{
    #region Fields
    public static InputManager Instance { get; private set; }

    private GameInputActions input;   
    private Vector2 move;
    private readonly ButtonState[] buttons = new ButtonState[System.Enum.GetValues(typeof(BattleAction)).Length];
    private readonly Dictionary<InputAction, BattleAction> actionLookup = new();
    [SerializeField] private GameObject onScreenControlsRoot;
    [SerializeField] private PlayerInput playerInput;
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

        // Movement
        input.BattleActions.Move.performed += OnMovePerformed;
        input.BattleActions.Move.canceled += _ => move = Vector2.zero;

        // Buttons
        BindButton(input.BattleActions.Pass,    BattleAction.Pass);
        BindButton(input.BattleActions.Shoot,   BattleAction.Shoot);
        BindButton(input.BattleActions.Change,  BattleAction.Change);
        BindButton(input.BattleActions.Dribble, BattleAction.Dribble);
        BindButton(input.BattleActions.Block,  BattleAction.Block);

        // Enable once here
        input.BattleActions.Enable();

        // Respond to control scheme changes (requires a PlayerInput component on the same GO)
        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.onControlsChanged += OnControlsChanged;
        }
    }

    private void OnDestroy()
    {
        if (input != null)
        {
            input.BattleActions.Move.performed -= OnMovePerformed;

            UnbindButton(input.BattleActions.Pass);
            UnbindButton(input.BattleActions.Shoot);
            UnbindButton(input.BattleActions.Change);
            UnbindButton(input.BattleActions.Dribble);
            UnbindButton(input.BattleActions.Block);

            input.BattleActions.Disable();
            input.Dispose();
        }

        if (playerInput != null)
        {
            playerInput.onControlsChanged -= OnControlsChanged;
        }

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        // Accumulate hold time for held buttons
        float deltaTime = Time.unscaledDeltaTime;
        for (int i = 0; i < System.Enum.GetValues(typeof(BattleAction)).Length; i++)
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
    private void BindButton(InputAction inputAction, BattleAction battleAction)
    {
        actionLookup[inputAction] = battleAction;
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
        if (actionLookup.TryGetValue(ctx.action, out var battleAction))
            OnButtonDown(battleAction);
    }

    private void OnAnyButtonCanceled(InputAction.CallbackContext ctx)
    {
        if (actionLookup.TryGetValue(ctx.action, out var battleAction))
            OnButtonUp(battleAction);
    }

    private void OnButtonDown(BattleAction battleAction)
    {
        LogManager.Trace($"[InputManager] OnButtonDown: {battleAction}");
        var button = buttons[(int)battleAction];
        button.SetDown();
        buttons[(int)battleAction] = button;
    }

    private void OnButtonUp(BattleAction battleAction)
    {
        var button = buttons[(int)battleAction];
        button.SetUp();
        buttons[(int)battleAction] = button;
    }
    #endregion

    #region API
    public Vector2 GetMove() => move;
    public bool GetDown(BattleAction battleAction) => buttons[(int)battleAction].DownFrame == (uint)Time.frameCount;
    public bool GetHeld(BattleAction battleAction) => buttons[(int)battleAction].Held;
    public bool GetUp(BattleAction battleAction) => buttons[(int)battleAction].UpFrame == (uint)Time.frameCount;
    #endregion

    #region Scheme helpers
    private void OnControlsChanged(PlayerInput playerInput)
    {
        LogManager.Trace($"[InputManager] OnControlsChanged: {playerInput.currentControlScheme}");
        UpdateOnScreenVisibility();
    }

    public void UpdateOnScreenVisibility()
    {
        bool isAndroid = Application.platform == RuntimePlatform.Android;
        if (onScreenControlsRoot && onScreenControlsRoot.activeSelf != isAndroid)
            onScreenControlsRoot.SetActive(isAndroid);
    }
    #endregion
}
