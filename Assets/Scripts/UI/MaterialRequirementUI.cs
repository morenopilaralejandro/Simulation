using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaterialRequirementUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ItemUI itemUI;
    [SerializeField] private TMP_Text textAmountRequired;
    [SerializeField] private ScrollRectForwarder scrollForwarder;

    private ScrollRect parentScrollRect;
    public ScrollRect ParentScrollRect => parentScrollRect;

    public void SetData(MaterialRequirement materialRequirement)
    {
        if (textAmountRequired != null)
            textAmountRequired.text = materialRequirement.Amount.ToString();

        Item item = ItemFactory.CreateById(materialRequirement.ItemId);

        itemUI.SetData(item, ItemManager.Instance.GetItemCount(item));
    }

    public void SetScrollRect(ScrollRect sr)
    {
        parentScrollRect = sr;
        if (scrollForwarder != null) scrollForwarder.SetScrollRect(sr);
    }

    public void Clear()
    {
        itemUI.Clear();
        if (textAmountRequired != null)
            textAmountRequired.text = "";
    }
}
