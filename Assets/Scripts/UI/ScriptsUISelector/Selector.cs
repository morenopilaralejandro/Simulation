using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Selector<T, TItem> : Menu, IClosableMenu
    where TItem : SelectorListItem<T>
{
    #region Inspector

    [Header("Selector — List")]
    [SerializeField] protected GameObject     listItemPrefab;
    [SerializeField] protected RectTransform  listItemContainer;
    [SerializeField] protected int            preWarmSize = 25;

    [Header("Selector — Scroll")]
    [SerializeField] protected ScrollViewAutoScroll autoScroll;
    [SerializeField] protected ScrollRect           scrollRect;

    [Header("Selector — Behavior")]
    [SerializeField] protected bool closeOnSelect = false;

    #endregion

    #region State

    protected ISelectorSource<T>      source;
    protected ISelectorFilter<T>      filter;
    protected ISelectorClickAction<T> clickAction;

    protected readonly Queue<TItem> pool   = new();
    protected readonly List<TItem>  active = new();

    #endregion

    #region Unity Lifecycle

    protected virtual void Start()
    {
        PreWarmPool();
    }

    #endregion

    #region Public API

    public void Open(ISelectorSource<T>      src,
                     ISelectorClickAction<T> action,
                     ISelectorFilter<T>      flt = null)
    {
        source      = src;
        clickAction = action;
        filter      = flt;
        MenuManager.Instance.OpenMenu(this);
    }

    /// <summary>Re-applies the filter on the existing source. Useful when the user changes filter UI.</summary>
    public void ApplyFilter(ISelectorFilter<T> flt)
    {
        filter = flt;
        Repopulate();
    }

    public void Refresh() => Repopulate();

    #endregion

    #region Menu Overrides

    public override void Show()
    {
        base.Show();
        if (autoScroll != null)
        {
            autoScroll.Activate();
            autoScroll.ResetToTop();
        }
        Repopulate();
    }

    public override void Hide()
    {
        ReturnAllToPool();
        if (autoScroll != null) autoScroll.Deactivate();
        base.Hide();
    }

    public override void SetInteractable(bool interactable)
    {
        base.SetInteractable(interactable);
        if (autoScroll != null)
        {
            if (interactable) autoScroll.Activate();
            else              autoScroll.Deactivate();
        }
    }

    #endregion

    #region Subclass Contract

    /// <summary>Apply per-context data binding (e.g., kit/variant tweaks before display).</summary>
    protected abstract void Bind(TItem view, T data);

    /// <summary>Called when an item leaves the active set. Default unbinds and detaches click handler.</summary>
    protected virtual void Unbind(TItem view)
    {
        view.Clicked -= HandleItemClicked;
        view.Unbind();
    }

    #endregion

    #region Population

    protected virtual void Repopulate()
    {
        ReturnAllToPool();
        if (source == null) return;

        foreach (var data in source.Enumerate())
        {
            if (filter != null && !filter.Matches(data)) continue;

            var view = GetFromPool();
            view.Clicked += HandleItemClicked;
            Bind(view, data);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(listItemContainer);

        // Default focus on the first item.
        if (active.Count > 0)
            SetDefaultSelectable(active[0].Button);
    }

    private void HandleItemClicked(SelectorListItem<T> item)
    {
        clickAction?.Execute(item.Data, this);
        if (closeOnSelect) RequestClose();
    }

    #endregion

    #region Pool

    private void PreWarmPool()
    {
        for (int i = 0; i < preWarmSize; i++)
        {
            var view = InstantiateItem();
            view.gameObject.SetActive(false);
            pool.Enqueue(view);
        }
    }

    private TItem InstantiateItem()
    {
        var go = Instantiate(listItemPrefab, listItemContainer);
        var view = go.GetComponent<TItem>();
        view.SetScrollRect(scrollRect);
        return view;
    }

    protected TItem GetFromPool()
    {
        TItem view;
        if (pool.Count > 0)
        {
            view = pool.Dequeue();
            view.gameObject.SetActive(true);
        }
        else
        {
            view = InstantiateItem();
        }
        view.transform.SetAsLastSibling();
        active.Add(view);
        return view;
    }

    protected void ReturnToPool(TItem view)
    {
        Unbind(view);
        view.gameObject.SetActive(false);
        pool.Enqueue(view);
    }

    protected void ReturnAllToPool()
    {
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i] != null) ReturnToPool(active[i]);
        }
        active.Clear();
    }

    #endregion
}
