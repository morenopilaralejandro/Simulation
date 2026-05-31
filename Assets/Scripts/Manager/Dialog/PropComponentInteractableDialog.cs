using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Attach to InteractableProp. When the player interacts, starts a system dialog.
/// </summary>
public class PropComponentInteractableDialog : MonoBehaviour, IInteractable
{
    [Header("Dialog Settings")]
    [Tooltip("The ink knot name to start (e.g., 'elder_oak_talk')")]
    [SerializeField] private string _dialogKnotName;
        
    [Tooltip("Which ink story file to use. Default 'main'.")]
    [SerializeField] private string _storyId = "main";
        
    [Tooltip("Override knots based on conditions. Checked in order, first match wins.")]
    [SerializeField] private ConditionalDialog[] _conditionalDialogs;

    public void Interact()
    {
        if (DialogManager.Instance == null) return;
        if (DialogManager.Instance.IsDialogActive) return;

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
        if (_conditionalDialogs != null)
        {
            foreach (var conditional in _conditionalDialogs)
            {
                if (string.IsNullOrEmpty(conditional.flagName)) continue;
                    
                bool currentValue = DialogManager.Instance.DialogGameDataProvider.GetFlag(conditional.flagName);
                if (currentValue == conditional.flagValue)
                {
                    return conditional.knotName;
                }
            }
        }

        return _dialogKnotName;
    }

}
