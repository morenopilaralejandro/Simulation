using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;

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
    /*
        public static DialogManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private InkStoryManager _storyManager;
        [SerializeField] private DialogUIController _uiController;
        [SerializeField] private DialogLocalizationBridge _locBridge;
        //[SerializeField] private CharacterDatabase _characterDb;
        [SerializeField] private SimpleGameDataProvider _gameDataProvider;

        [Header("Input")]
        [SerializeField] private InputActionReference _confirmAction;
        [SerializeField] private InputActionReference _cancelAction;

        private DialogState _state = DialogState.Inactive;

        // Events for other game systems to hook into
        public event Action OnDialogStarted;
        public event Action OnDialogEnded;
        public event Action<string> OnGameEvent; // from ink TriggerEvent()

        public bool IsDialogActive => _state != DialogState.Inactive;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _characterDb.Initialize();
            _storyManager.Initialize(_locBridge, _gameDataProvider);
            _uiController.Initialize(_characterDb, _locBridge);
        }

        private void OnEnable()
        {
            // Subscribe to story events
            _storyManager.OnLineReady += HandleLineReady;
            _storyManager.OnChoicesReady += HandleChoicesReady;
            _storyManager.OnDialogComplete += HandleDialogComplete;
            _storyManager.OnCommandExecuted += HandleCommand;
            
            // Subscribe to UI events
            _uiController.OnTextDisplayComplete += HandleTextDisplayComplete;
            _uiController.OnChoiceSelected += HandleChoiceSelected;
            _uiController.OnContinueRequested += HandleContinueRequested;
            
            // Input
            if (_confirmAction != null)
            {
                _confirmAction.action.Enable();
                _confirmAction.action.performed += OnConfirmPressed;
            }
        }

        private void OnDisable()
        {
            _storyManager.OnLineReady -= HandleLineReady;
            _storyManager.OnChoicesReady -= HandleChoicesReady;
            _storyManager.OnDialogComplete -= HandleDialogComplete;
            _storyManager.OnCommandExecuted -= HandleCommand;
            
            _uiController.OnTextDisplayComplete -= HandleTextDisplayComplete;
            _uiController.OnChoiceSelected -= HandleChoiceSelected;
            _uiController.OnContinueRequested -= HandleContinueRequested;

            if (_confirmAction != null)
            {
                _confirmAction.action.performed -= OnConfirmPressed;
            }
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
            _uiController.Show();
            OnDialogStarted?.Invoke();
            
            _storyManager.StartDialog(storyId, knotName);
        }

        /// <summary>
        /// Force-close the dialog (for emergencies, cutscene skips, etc.)
        /// </summary>
        public void ForceEndDialog()
        {
            _state = DialogState.Inactive;
            _uiController.Hide();
            OnDialogEnded?.Invoke();
        }

        // ============ INPUT ============

        private void OnConfirmPressed(InputAction.CallbackContext ctx)
        {
            if (!IsDialogActive) return;
            
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
            _state = DialogState.Inactive;
            _uiController.Hide();
            OnDialogEnded?.Invoke();
        }

        // ============ COMMAND HANDLING ============

        private void HandleCommand(DialogCommand command)
        {
            switch (command.CommandName)
            {
                case "sfx":
                    PlaySFX(command.Parameters[0]);
                    break;
                    
                case "anim":
                    // Hook into your animation system
                    LogManager.Trace($"[DialogManager] Play animation: {command.Parameters[0]}");
                    break;
                    
                case "event":
                    OnGameEvent?.Invoke(command.Parameters[0]);
                    break;
                    
                case "give_item":
                    // Already handled in ink via GiveItem(), this is for UI feedback
                    break;
                    
                case "screen_shake":
                    // Hook into camera system
                    break;
                    
                default:
                    LogManager.Trace($"[DialogManager] Unhandled command: {command.CommandName}");
                    break;
            }
        }

        private void PlaySFX(string sfxName)
        {
            // Hook into your audio system
            LogManager.Trace($"[DialogManager] Play SFX: {sfxName}");
        }
*/
    }
