using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaterialRequirementLayout : MonoBehaviour
{
    #region Inspector

    [Header("List")]
    [SerializeField] protected GameObject listItemPrefab;
    [SerializeField] protected RectTransform listItemContainer;
    [SerializeField] protected int preWarmSize = 5;

    [Header("Selector — Scroll")]
    [SerializeField] protected ScrollViewAutoScroll autoScroll;
    [SerializeField] protected ScrollRect scrollRect;

    #endregion

    #region State

    protected readonly Queue<MaterialRequirementUI> pool = new();
    protected readonly List<MaterialRequirementUI> active = new();

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        PreWarmPool();
    }

    #endregion

    #region Menu Overrides

    public void SetData(List<MaterialRequirement> requirements)
    {
        if (autoScroll != null)
        {
            autoScroll.Activate();
            autoScroll.ResetToTop();
        }

        Repopulate(requirements);
    }

    public void Clear()
    {
        ReturnAllToPool();

        if (autoScroll != null)
            autoScroll.Deactivate();
    }

    #endregion

    #region Population

    private void Repopulate(List<MaterialRequirement> requirements)
    {
        ReturnAllToPool();

        if (requirements == null)
            return;

        foreach (MaterialRequirement requirement in requirements)
        {
            MaterialRequirementUI view = GetFromPool();

            Bind(view, requirement);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(listItemContainer);
    }

    private void Bind(
        MaterialRequirementUI view,
        MaterialRequirement requirement)
    {
        view.SetData(requirement);
    }

    private void Unbind(MaterialRequirementUI view)
    {
        view.Clear();
    }

    #endregion

    #region Pool

    private void PreWarmPool()
    {
        for (int i = 0; i < preWarmSize; i++)
        {
            MaterialRequirementUI view = InstantiateItem();

            view.gameObject.SetActive(false);
            pool.Enqueue(view);
        }
    }

    private MaterialRequirementUI InstantiateItem()
    {
        var go = Instantiate(listItemPrefab, listItemContainer); 
        var view = go.GetComponent<MaterialRequirementUI>(); 
        return view;
    }

    protected MaterialRequirementUI GetFromPool()
    {
        MaterialRequirementUI view;

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

    private void ReturnToPool(MaterialRequirementUI view)
    {
        Unbind(view);

        view.gameObject.SetActive(false);
        pool.Enqueue(view);
    }

    private void ReturnAllToPool()
    {
        for (int i = 0; i < active.Count; i++)
        {
            if (active[i] != null)
                ReturnToPool(active[i]);
        }

        active.Clear();
    }

    #endregion
}
