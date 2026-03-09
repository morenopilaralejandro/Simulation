using UnityEngine;
using UnityEngine.InputSystem;

    /// <summary>
    /// Attach to NPC GameObjects. When the player interacts, starts the appropriate dialog.
    /// Supports different dialogs based on quest state via the ink script's internal logic.
    /// </summary>
    public class NPCDialogTrigger : MonoBehaviour
    {
        /*
        [Header("Dialog Settings")]
        [Tooltip("The ink knot name to start (e.g., 'elder_oak_talk')")]
        [SerializeField] private string _dialogKnotName;
        
        [Tooltip("Which ink story file to use. Default 'main'.")]
        [SerializeField] private string _storyId = "main";
        
        [Tooltip("Override knots based on conditions. Checked in order, first match wins.")]
        [SerializeField] private ConditionalDialog[] _conditionalDialogs;

        [Header("Interaction")]
        [SerializeField] private InputActionReference _interactAction;
        [SerializeField] private GameObject _interactPrompt; // "Press E to talk"
        [SerializeField] private float _interactionRange = 2.5f;
        [SerializeField] private Transform _lookAtTarget; // where the player turns to face

        [Header("Visual Feedback")]
        [SerializeField] private GameObject _questIndicator;    // "!" icon
        [SerializeField] private GameObject _questTurnInIndicator; // "?" icon
        [SerializeField] private GameObject _chatIndicator;     // "..." icon

        private bool _playerInRange = false;
        private bool _isInteracting = false;
        private Transform _playerTransform;
        private IDialogGameDataProvider _gameData;

        private void Start()
        {
            if (_interactPrompt != null)
                _interactPrompt.SetActive(false);
            
            // Find game data provider for conditional checks
            _gameData = FindObjectOfType<DialogGameDataProvider>();
            
            UpdateIndicators();
        }

        private void OnEnable()
        {
            if (_interactAction != null)
            {
                _interactAction.action.Enable();
                _interactAction.action.performed += OnInteractPressed;
            }

            // Listen for dialog end to reset state
            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.OnDialogEnded += OnDialogEnded;
            }
        }

        private void OnDisable()
        {
            if (_interactAction != null)
            {
                _interactAction.action.performed -= OnInteractPressed;
            }

            if (DialogManager.Instance != null)
            {
                DialogManager.Instance.OnDialogEnded -= OnDialogEnded;
            }
        }

        private void OnInteractPressed(InputAction.CallbackContext ctx)
        {
            if (!_playerInRange) return;
            if (_isInteracting) return;
            if (DialogManager.Instance == null) return;
            if (DialogManager.Instance.IsDialogActive) return;

            StartInteraction();
        }

        private void StartInteraction()
        {
            _isInteracting = true;
            
            if (_interactPrompt != null)
                _interactPrompt.SetActive(false);

            // Face the player toward the NPC (optional)
            if (_lookAtTarget != null && _playerTransform != null)
            {
                Vector3 lookDir = (transform.position - _playerTransform.position).normalized;
                lookDir.y = 0;
                if (lookDir != Vector3.zero)
                    _playerTransform.rotation = Quaternion.LookRotation(lookDir);
            }

            // Determine which knot to use
            string knotToUse = ResolveDialogKnot();
            
            DialogManager.Instance.StartDialog(knotToUse, _storyId);
        }

        /// <summary>
        /// Check conditional dialogs first, then fall back to default knot.
        /// This allows the NPC to say different things based on quest progress
        /// even before ink's internal branching kicks in.
        /// 
        /// Useful when you want completely different ink knots for different states,
        /// rather than handling it all inside one knot.
        /// </summary>
        private string ResolveDialogKnot()
        {
            if (_conditionalDialogs != null && _gameData != null)
            {
                foreach (var conditional in _conditionalDialogs)
                {
                    if (string.IsNullOrEmpty(conditional.flagName)) continue;
                    
                    bool currentValue = _gameData.GetFlag(conditional.flagName);
                    if (currentValue == conditional.flagValue)
                    {
                        return conditional.knotName;
                    }
                }
            }

            return _dialogKnotName;
        }

        private void OnDialogEnded()
        {
            _isInteracting = false;
            UpdateIndicators();
            
            // Re-show interact prompt if player is still in range
            if (_playerInRange && _interactPrompt != null)
                _interactPrompt.SetActive(true);
        }

        /// <summary>
        /// Update the overhead indicator (!, ?, ...) based on quest state.
        /// </summary>
        public void UpdateIndicators()
        {
            if (_questIndicator != null)
                _questIndicator.SetActive(false);
            if (_questTurnInIndicator != null)
                _questTurnInIndicator.SetActive(false);
            if (_chatIndicator != null)
                _chatIndicator.SetActive(false);

            if (_gameData == null) return;

            // Example logic - customize for your quest system
            // Priority: Turn-in "?" > New quest "!" > Chat "..."
            // You'd replace these with your actual quest checks
            
            bool hasQuestToTurnIn = false;
            bool hasNewQuest = false;
            
            // Check conditional dialogs to infer state
            if (_conditionalDialogs != null)
            {
                foreach (var cond in _conditionalDialogs)
                {
                    if (cond.knotName.Contains("turn_in") && 
                        _gameData.GetFlag(cond.flagName) == cond.flagValue)
                    {
                        hasQuestToTurnIn = true;
                        break;
                    }
                    if (cond.knotName.Contains("offer") && 
                        _gameData.GetFlag(cond.flagName) == cond.flagValue)
                    {
                        hasNewQuest = true;
                    }
                }
            }

            if (hasQuestToTurnIn && _questTurnInIndicator != null)
                _questTurnInIndicator.SetActive(true);
            else if (hasNewQuest && _questIndicator != null)
                _questIndicator.SetActive(true);
            else if (_chatIndicator != null)
                _chatIndicator.SetActive(true);
        }

        // ============ COLLISION DETECTION ============

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            _playerInRange = true;
            _playerTransform = other.transform;
            
            if (!_isInteracting && _interactPrompt != null)
                _interactPrompt.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            
            _playerInRange = false;
            _playerTransform = null;
            
            if (_interactPrompt != null)
                _interactPrompt.SetActive(false);
        }

        // For 2D games, use these instead:
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            _playerInRange = true;
            _playerTransform = other.transform;
            
            if (!_isInteracting && _interactPrompt != null)
                _interactPrompt.SetActive(true);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;
            
            _playerInRange = false;
            _playerTransform = null;
            
            if (_interactPrompt != null)
                _interactPrompt.SetActive(false);
        }

        // ============ EDITOR ============

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _interactionRange);
        }
#endif
*/
    }
