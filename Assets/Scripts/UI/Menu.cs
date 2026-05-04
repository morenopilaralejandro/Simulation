using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour, IClosableMenu
{
    #region Fields

    [Header("Menu Settings")]
    [SerializeField] private Selectable defaultSelectable;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool deactivateOnHide = false;
    [SerializeField] private bool closeAllPreviousOnBack = false;
    [SerializeField] private bool hasMemory = true;
    [SerializeField] private bool hideWhenNotInteractable = false;
    [SerializeField] private bool startHidden = true;

    [Header("Stack Behavior")]
    [SerializeField] private bool hideWhenCovered = false;
    [SerializeField] private bool dimWhenCovered = false;
    [SerializeField, Range(0f, 1f)] private float dimmedAlpha = 0.5f;

    [Header("Audio")]
    [SerializeField] protected string sfxOpen  = "sfx-menu_tap";
    [SerializeField] protected string sfxClose = "sfx-menu_back";

    private GameObject lastSelected;
    private bool wasInteractable;
    private bool isRestoringFocus;

    public bool CloseAllPrevious => closeAllPreviousOnBack;
    public GameObject LastSelected => lastSelected;

    #endregion

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        if (startHidden) HideImmediate();
    }

    protected virtual void OnEnable()
    {
        UIEvents.OnSelectableSelected += HandleSelectableSelected;
    }

    protected virtual void OnDisable()
    {
        UIEvents.OnSelectableSelected -= HandleSelectableSelected;
    }

    #endregion

    #region Visibility

    public virtual void Show()
    {
        if (deactivateOnHide)
            gameObject.SetActive(true);
        else
            canvasGroup.alpha = 1f;

        SetDefaultFocus();
    }

    public virtual void Hide()
    {
        if (deactivateOnHide)
            gameObject.SetActive(false);
        else
            canvasGroup.alpha = 0f;
    }

    public void SetVisible(bool isVisible)
    {
        canvasGroup.alpha = isVisible ? 1f : 0f;
    }


    private void HideImmediate()
    {
        if (deactivateOnHide)
        {
            gameObject.SetActive(false);
        }
        else
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    #endregion

    #region Interactability

    public virtual void SetInteractable(bool isInteractable)
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = isInteractable;

        if (hideWhenNotInteractable)
            canvasGroup.alpha = isInteractable ? 1f : 0f;

        if (isInteractable && !wasInteractable)
            OnGainedInput();
        else if (!isInteractable && wasInteractable)
            OnLostInput();

        wasInteractable = isInteractable;

        if (isInteractable)
            SetDefaultFocus();
    }

    public virtual bool IsInteractable() => canvasGroup.interactable;

    /// <summary>Override to subscribe input shortcuts. Called when this menu becomes the top of the stack.</summary>
    protected virtual void OnGainedInput() { }

    /// <summary>Override to unsubscribe input shortcuts. Called when this menu is covered or closed.</summary>
    protected virtual void OnLostInput() { }

    #endregion

    #region Stack Lifecycle Hooks (called by MenuManager)

    public virtual void OnOpened()
    {
        if (!string.IsNullOrEmpty(sfxOpen))
            AudioManager.Instance?.PlaySfx(sfxOpen);
    }

    public virtual void OnClosed()
    {
        if (!string.IsNullOrEmpty(sfxClose))
            AudioManager.Instance?.PlaySfx(sfxClose);
    }

    /// <summary>Called when another menu is pushed on top of this one.</summary>
    public virtual void OnCovered()
    {
        if (hideWhenCovered)
            SetVisible(false);
        else if (dimWhenCovered)
            canvasGroup.alpha = dimmedAlpha;
    }

    /// <summary>Called when this menu becomes the top again after a menu above it was closed.</summary>
    public virtual void OnRevealed()
    {
        if (hideWhenCovered || dimWhenCovered)
            canvasGroup.alpha = 1f;
    }

    #endregion

    #region Focus / Memory

    public void SetLastSelected(GameObject obj) => lastSelected = obj;

    public void SetDefaultSelectable(Selectable selectable, bool focusImmediately = true)
    {
        defaultSelectable = selectable;

        if (InputManager.Instance.IsAndroid && !InputManager.Instance.IsUsingController) return;

        if (focusImmediately && selectable != null)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            lastSelected = selectable.gameObject;
        }
    }

    protected void SetDefaultFocus()
    {
        isRestoringFocus = true;

        if (InputManager.Instance.IsAndroid && !InputManager.Instance.IsUsingController) return;

        if (hasMemory && lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
            return;
        }

        if (defaultSelectable == null) return;
        if (EventSystem.current.currentSelectedGameObject == defaultSelectable.gameObject) return;

        EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);

        isRestoringFocus = false;
    }

    private void HandleSelectableSelected(GameObject go)
    {
        if (this == null) return;
        if (!IsInteractable()) return;
        if (go == null) return;
        if (!go.transform.IsChildOf(transform)) return;

        lastSelected = go;

        if (isRestoringFocus) return;
        // TODO logic after the focus is restored
    }

    #endregion

    #region IClosableMenu

    public void RequestClose()
    {
        MenuManager.Instance.RequestClose(this);
    }

    #endregion
}
