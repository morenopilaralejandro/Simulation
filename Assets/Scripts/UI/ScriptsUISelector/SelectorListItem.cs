using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SelectorListItem<T> : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected Button button;
    [SerializeField] private ScrollRectForwarder scrollForwarder;

    protected T data;
    protected ScrollRect parentScrollRect;

    public Button Button => button;
    public T Data => data;
    public ScrollRect ParentScrollRect => parentScrollRect;

    public event System.Action<SelectorListItem<T>> Clicked;
    public event System.Action<SelectorListItem<T>> Selected;
    public event System.Action<SelectorListItem<T>> PointerEntered;

    /// <summary>Inject the parent ScrollRect once when the item is created or pooled.</summary>
    public void SetScrollRect(ScrollRect sr)
    {
        parentScrollRect = sr;
        if (scrollForwarder != null) scrollForwarder.SetScrollRect(sr);
    }

    public void Bind(T value) { data = value; OnBind(value); }
    public void Unbind()      { OnUnbind(); data = default; ClearListeners(); }

    protected abstract void OnBind(T value);
    protected abstract void OnUnbind();

    private void ClearListeners()
    {
        Clicked        = null;
        Selected       = null;
        PointerEntered = null;
    }

    // Wired from the prefab's UnityEvents (Button OnClick, EventTrigger, etc.)
    public void OnListItemClicked()         => Clicked?.Invoke(this);
    public void OnListItemSelected()        => Selected?.Invoke(this);
    public void OnListItemPointerEnter()    => PointerEntered?.Invoke(this);
}
