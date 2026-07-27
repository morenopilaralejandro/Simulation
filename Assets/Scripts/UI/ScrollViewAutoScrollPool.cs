using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollViewAutoScrollPool<TItem> : MonoBehaviour where TItem : Component
{
    [Header("Pool")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private RectTransform container;
    [SerializeField] private int preWarmSize = 25;

    [Header("Scroll")]
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private ScrollViewAutoScroll autoScroll;

    private readonly Queue<TItem> pool = new();
    private readonly List<TItem> active = new();

    public IReadOnlyList<TItem> ActiveItems => active;

    private void Awake()
    {
        PreWarm();
    }

    #region Public

    public void Populate<T>(IEnumerable<T> items, Action<TItem, T> bind)
    {
        Clear();

        foreach (var data in items)
        {
            var item = Get();
            bind(item, data);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void Clear(Action<TItem> unbind = null)
    {
        for (int i = 0; i < active.Count; i++)
        {
            Return(active[i], unbind);
        }

        active.Clear();
    }

    public void ActivateScroll()
    {
        if (autoScroll == null) return;

        autoScroll.Activate();
        autoScroll.ResetToTop();
    }

    public void DeactivateScroll()
    {
        autoScroll?.Deactivate();
    }

    public TItem Get()
    {
        TItem item;

        if (pool.Count > 0)
        {
            item = pool.Dequeue();
            item.gameObject.SetActive(true);
        }
        else
        {
            item = InstantiateItem();
        }

        item.transform.SetAsLastSibling();
        active.Add(item);

        return item;
    }

    public void Return(TItem item, Action<TItem> unbind = null)
    {
        if (item == null) return;

        unbind?.Invoke(item);

        if (item is IScrollViewPoolItem poolItem)
            poolItem.Clear();

        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    #endregion

    #region Private

    private void PreWarm()
    {
        for (int i = 0; i < preWarmSize; i++)
        {
            var item = InstantiateItem();
            item.gameObject.SetActive(false);
            pool.Enqueue(item);
        }
    }

    private TItem InstantiateItem()
    {
        var go = Instantiate(itemPrefab, container);

        var item = go.GetComponent<TItem>();

        if (item == null)
        {
            Debug.LogError($"{itemPrefab.name} is missing component {typeof(TItem).Name}");
            return null;
        }

        if (item is IScrollViewPoolItem poolItem)
            poolItem.SetScrollRect(scrollRect);

        return item;
    }

    #endregion
}
