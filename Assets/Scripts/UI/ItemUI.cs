using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class ItemUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textAmount;
    [SerializeField] private Image imageIcon;

    private readonly AddressableBinding<Sprite> _bindingIcon = new();

    public void SetData(Item item, int? amount = null)
    {
        if (textName != null)
            textName.text = item.ItemName;

        if (textAmount != null)
            textAmount.text = amount?.ToString() ?? "";

        if (imageIcon != null)
        {
            imageIcon.color = item.IconColor;
            _ = SetIconAsync(item.IconSpriteAddress);
        }
    }

    public void Clear()
    {
        if (textName != null)
            textName.text = "";

        if (textAmount != null)
            textAmount.text = "";

        _bindingIcon.Release();
        _bindingIcon.Cancel();

        if (imageIcon != null)
        {
            imageIcon.sprite = null;
            imageIcon.enabled = false;
        }
    }

    public async Task SetIconAsync(string address)
    {
        if (imageIcon == null)
            return;

        imageIcon.enabled = false;

        var asset = await _bindingIcon.LoadAsync(address);

        // In case the object was destroyed while loading.
        if (imageIcon == null)
            return;

        imageIcon.sprite = asset;
        imageIcon.enabled = asset != null;
    }
}
