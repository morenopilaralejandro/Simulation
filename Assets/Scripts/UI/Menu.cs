using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    [Header("Menu settings")]
    [SerializeField] private Selectable defaultSelectable;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool deactivateOnHide = true;
    [SerializeField] private bool closeAllPreviousOnBack = false;
    [SerializeField] private bool hasMemory = false;

    private GameObject lastSelected;

    public bool CloseAllPrevious => closeAllPreviousOnBack;

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

    public virtual void SetInteractable(bool isInteractable) 
    { 
        canvasGroup.interactable = canvasGroup.blocksRaycasts = isInteractable;
        if (isInteractable)
            SetDefaultFocus();
    }
    public virtual bool IsInteractable() => canvasGroup.interactable;
    public void SetLastSelected(GameObject obj) => lastSelected = obj;

    public void SetDefaultSelectable(Selectable selectable, bool focusImmediately = true)
    {
        defaultSelectable = selectable;

        if (focusImmediately && selectable != null)
        {
            EventSystem.current.SetSelectedGameObject(selectable.gameObject);
            lastSelected = selectable.gameObject;
        }
    }

    protected void SetDefaultFocus()
    {
        if (hasMemory && lastSelected != null) 
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        } else 
        {
            if (defaultSelectable == null) return;
            EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
        }
    }

    /*  
    private void Update()
    {

        if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }
    */
}
