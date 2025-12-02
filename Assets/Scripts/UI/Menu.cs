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

    public virtual void SetInteractable(bool isInteractable) => canvasGroup.interactable = canvasGroup.blocksRaycasts = isInteractable;
    public virtual bool IsInteractable() => canvasGroup.interactable;

    protected void SetDefaultFocus()
    {
        if (defaultSelectable != null)
        {
            EventSystem.current.SetSelectedGameObject(defaultSelectable.gameObject);
            lastSelected = defaultSelectable.gameObject;
        }
        else if (EventSystem.current.currentSelectedGameObject)
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
    }

    private void Update()
    {
        /*
        if (EventSystem.current.currentSelectedGameObject == null && lastSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = EventSystem.current.currentSelectedGameObject;
        }
        */
    }
}
