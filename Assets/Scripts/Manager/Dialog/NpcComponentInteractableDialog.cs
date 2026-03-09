using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to NPC GameObjects. When the player interacts, starts the appropriate dialog.
/// Supports different dialogs based on quest state via the ink script's internal logic.
/// </summary>
public class NpcComponentInteractableDialog : Interactable
{
    private NpcEntity npcEntity;

    [Header("Dialog Settings")]
    [Tooltip("The ink knot name to start (e.g., 'elder_oak_talk')")]
    [SerializeField] private string _dialogKnotName;
        
    [Tooltip("Which ink story file to use. Default 'main'.")]
    [SerializeField] private string _storyId = "main";
        
    [Tooltip("Override knots based on conditions. Checked in order, first match wins.")]
    [SerializeField] private ConditionalDialog[] _conditionalDialogs;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject _questIndicator;    // "!" icon
    [SerializeField] private GameObject _questTurnInIndicator; // "?" icon
    [SerializeField] private GameObject _chatIndicator;     // "..." icon

    private DialogManager _dialogManager;
    private IDialogGameDataProvider _gameData;
    private bool _isInteracting = false;

    public void Initialize(NpcEntity npcEntity)
    {
        this.npcEntity = npcEntity;
        _dialogManager = DialogManager.Instance;
        _gameData = _dialogManager.DialogGameDataProvider;
        //UpdateIndicators();
    }

    protected override void InteractInternal()
    {
        if (_isInteracting) return;
        if (DialogManager.Instance == null) return;
        if (DialogManager.Instance.IsDialogActive) return;

        StartInteraction();
    }

    private void StartInteraction()
    {
        _isInteracting = true;
            
        // Face the player toward the NPC (optional)

        // Determine which knot to use
        string knotToUse = ResolveDialogKnot();
            
        _dialogManager.StartDialog(knotToUse, _storyId);
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

}
