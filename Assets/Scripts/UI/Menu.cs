using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Menu : MonoBehaviour
{
    [Header("Menu settings")]
    [SerializeField] private Selectable defaultSelectable;
    [SerializeField] private bool closeAllPreviousOnBack = false;
    private GameObject lastSelected;

    public bool CloseAllPrevious => closeAllPreviousOnBack;

    public virtual void Show()
    {
        gameObject.SetActive(true);
        SetDefaultFocus();
    }

    public virtual void Hide() => gameObject.SetActive(false);

    public virtual void SetInteractable(bool value)
    {
        var cg = GetComponent<CanvasGroup>();
        if (cg) cg.interactable = cg.blocksRaycasts = value;
    }

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
