using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class ItemDropUI : MonoBehaviour, IScrollViewPoolItem
{
    [Header("UI")]
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private ScrollRectForwarder scrollForwarder;

    private ScrollRect parentScrollRect;
    public ScrollRect ParentScrollRect => parentScrollRect;

    public void SetData(Item item, int amount) => itemUI.SetData(item, amount);
    public void Clear() => itemUI.Clear();

    public void SetScrollRect(ScrollRect sr)
    {
        parentScrollRect = sr;
        if (scrollForwarder != null) scrollForwarder.SetScrollRect(sr);
    }
}
