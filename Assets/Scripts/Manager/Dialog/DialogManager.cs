using UnityEngine;
using System;
using System.Collections.Generic;
using Simulation.Enums.Dialog;
using Simulation.Enums.Input;
using Simulation.Enums.World;

/// <summary>
/// Main entry point for the dialog system.
/// Coordinates InkStoryManager, UI, localization, and game systems.
/// 
/// USAGE:
///   DialogManager.Instance.StartDialog("elder_oak_talk");
///   DialogManager.Instance.StartDialog("merchant_rosa_talk", "side_stories");
/// </summary>
public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private InkStoryManager _storyManager;
    [SerializeField] private DialogLocalizationBridge _locBridge;
    [SerializeField] private DialogGameDataProvider _gameDataProvider;
    [SerializeField] private DialogSpeakerCache _speakerCache;

    private DialogUIController _uiController;
    private DialogState _state = DialogState.Inactive;
    private AudioManager audioManager;

    public bool IsDialogActive => _state != DialogState.Inactive;
    public bool CanAcceptInput => IsDialogActive && !_uiController.IsFading;
    public DialogGameDataProvider DialogGameDataProvider => _gameDataProvider;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _storyManager.Initialize(_locBridge, _gameDataProvider);
    }

    private void OnEnable()
    {
        // Subscribe to story events
        DialogEvents.OnLineReady += HandleLineReady;
        DialogEvents.OnChoicesReady += HandleChoicesReady;
        DialogEvents.OnDialogComplete += HandleDialogComplete;
        DialogEvents.OnCommandExecuted += HandleCommand;

        // Subscribe to UI events
        DialogEvents.OnTextDisplayComplete += HandleTextDisplayComplete;
        DialogEvents.OnChoiceSelected += HandleChoiceSelected;
        DialogEvents.OnContinueRequested += HandleContinueRequested;

        DialogEvents.OnDialogMenuClosed += HandleDialogMenuClosed;

    }

    private void OnDisable()
    {
        DialogEvents.OnLineReady -= HandleLineReady;
        DialogEvents.OnChoicesReady -= HandleChoicesReady;
        DialogEvents.OnDialogComplete -= HandleDialogComplete;
        DialogEvents.OnCommandExecuted -= HandleCommand;

        DialogEvents.OnTextDisplayComplete -= HandleTextDisplayComplete;
        DialogEvents.OnChoiceSelected -= HandleChoiceSelected;
        DialogEvents.OnContinueRequested -= HandleContinueRequested;

        DialogEvents.OnDialogMenuClosed -= HandleDialogMenuClosed;
    }

    // ============ Registration ============
    public void RegisterUIController(DialogUIController dialogUIController)
    {
        _uiController = dialogUIController;
        _uiController.Initialize(_locBridge);
    }

    public void UnregisterUIController()
    {
        _uiController = null;
    }

    // ============ PUBLIC API ============

    /// <summary>
    /// Start a dialog. This is the main method you call.
    /// knotName: the ink knot to start from (e.g., "elder_oak_talk")
    /// storyId: which story file to use (default "main")
    /// </summary>
    public void StartDialog(string knotName, string storyId = "main")
    {
        if (IsDialogActive)
        {
            LogManager.Trace("[DialogManager] Dialog already active!");
            return;
        }

        _state = DialogState.Processing;

        // >>> ACTION MAP SWITCH — this is the key fix <<<
        InputManager.Instance.DisableWorldActions();
        InputManager.Instance.EnableDialogActions();

        _uiController.Show();
        DialogEvents.RaiseDialogStarted();
        _storyManager.StartDialog(storyId, knotName);
    }

    /// <summary>
    /// Force-close the dialog (for emergencies, cutscene skips, etc.)
    /// </summary>
    public void ForceEndDialog()
    {
        _state = DialogState.Inactive;
        _uiController.Hide();

        // >>> Restore action maps <<<
        InputManager.Instance.DisableDialogActions();
        InputManager.Instance.EnableWorldActions();

        DialogEvents.RaiseDialogEnded();
    }

    public Speaker Speaker => _speakerCache.Speaker;
    public Speaker GetSpeakerById(string speakerId) => _speakerCache.GetSpeakerById(speakerId);

    // ============ INPUT ============

    public void InputPressed()
    {
        if (!CanAcceptInput) return;

        switch (_state)
        {
            case DialogState.ShowingText:
                // Skip typewriter
                _uiController.HandleAdvanceInput();
                break;

            case DialogState.WaitingForInput:
                // Advance to next line
                _uiController.HandleAdvanceInput();
                break;

            case DialogState.WaitingForChoice:
                // Choices handle their own input via buttons
                break;
        }
    }

    // ============ STORY CALLBACKS ============

    private void HandleLineReady(DialogLine line)
    {
        _state = DialogState.ShowingText;
        _uiController.ShowLine(line);
    }

    private void HandleTextDisplayComplete()
    {
        // Text finished typing. Check if there are choices.
        if (_storyManager.HasChoices)
        {
            // Don't wait for input; immediately check for choices
            _storyManager.CheckForChoices();
        }
        else if (_storyManager.CanContinue)
        {
            _state = DialogState.WaitingForInput;
        }
        else
        {
            // No more content - wait for final confirm then close
            _state = DialogState.WaitingForInput;
        }
    }

    private void HandleChoicesReady(List<DialogChoice> choices)
    {
        _state = DialogState.WaitingForChoice;
        _uiController.ShowChoices(choices);
    }

    private void HandleChoiceSelected(int choiceIndex)
    {
        _state = DialogState.Processing;
        _uiController.HideChoices();
        _storyManager.SelectChoice(choiceIndex);
    }

    private void HandleContinueRequested()
    {
        if (_storyManager.CanContinue)
        {
            _state = DialogState.Processing;
            _storyManager.ContinueDialog();
        }
        else
        {
            // No more content, close dialog
            HandleDialogComplete();
        }
    }

    private void HandleDialogComplete()
    {
        if (_state == DialogState.Inactive) return; // Already completed

        _state = DialogState.Inactive;
        _uiController.Hide();

        // >>> Restore action maps <<<
        InputManager.Instance.DisableDialogActions();
        InputManager.Instance.EnableWorldActions();

        DialogEvents.RaiseDialogEnded();
    }

    // ============ COMMAND HANDLING ============

    private void HandleCommand(DialogCommand command)
    {
        switch (command.CommandName)
        {
            case "sfx":
                audioManager.PlaySfx(command.Parameters[0]);
                break;

            case "anim":
                // Hook into your animation system
                LogManager.Trace($"[DialogManager] Play animation: {command.Parameters[0]}");
                break;

            case "event":
                DialogEvents.RaiseGameEvent(command.Parameters[0]);
                break;

            case "give_item":
                // Already handled in ink via GiveItem(), this is for UI feedback
                break;

            case "screen_shake":
                // Hook into camera system
                break;

            case "open_menu":
                HandleOpenMenu(command.Parameters);
                break;

            default:
                LogManager.Trace($"[DialogManager] Unhandled command: {command.CommandName}");
                break;
        }
    }

    private void HandleOpenMenu(string[] parameters)
    {
        // parameters[0] = menu type ("shop", "inventory", "crafting")
        // parameters[1] = context id ("blacksmith", "potion_seller")
        
        string menuType = parameters[0];
        string contextId = parameters.Length > 1 ? parameters[1] : "";

        // 1. Pause the dialog
        _storyManager.Pause();

        // 2. Optionally hide the dialog box
        _uiController.Hide();

        // 3. Open the menu
        switch (menuType)
        {
            case "shop":
                //ShopManager.Instance.OpenShop(contextId);
                break;
            case "inventory":
                //InventoryUI.Instance.Open();
                break;
            case "crafting":
                //CraftingUI.Instance.Open(contextId);
                break;
        }
    }

    private void HandleDialogMenuClosed()
    {
        // 1. Show dialog box again
        _uiController.Show();

        // 2. Resume dialog
        _storyManager.Resume();
    }
}
