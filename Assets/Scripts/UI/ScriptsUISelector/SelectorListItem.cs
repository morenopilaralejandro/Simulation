using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class SelectorListItem<T> : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected Button button;

    protected T data;
    public Button Button => button;
    public T Data => data;

    public event System.Action<SelectorListItem<T>> Clicked;
    public event System.Action<SelectorListItem<T>> Selected;
    public event System.Action<SelectorListItem<T>> PointerEntered;

    public void Bind(T value)
    {
        data = value;
        OnBind(value);
    }

    public void Unbind()
    {
        OnUnbind();
        data = default;
    }

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
