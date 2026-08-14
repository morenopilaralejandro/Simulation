using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Aremoreno.Enums.Character;
using Aremoreno.Enums.Wing;

public class WingUI : MonoBehaviour
{
    #region Fields

    [Header("UI Elements")]
    [SerializeField] private Image imageIcon;
    [SerializeField] private Image[] imageElements;
    [SerializeField] private Image imageEquipped;
    [SerializeField] private Image imageEvolution;
    [SerializeField] private TMP_Text textName;
    [SerializeField] private TMP_Text textDescription;

    private int _setVersion;
    private readonly AddressableBinding<Sprite> _bindingEvolution = new();

    #endregion

    #region Logic

    public void SetData(Wing wing)
    {
        if (wing == null)
        {
            Clear();
            return;
        }

        // Icon
        if (imageIcon != null)
            imageIcon.color = ColorManager.GetWingColor(wing.WingColorType);

        // Elements
        if (imageElements != null)
        {
            // Hide all element icons first
            foreach (var image in imageElements)
            {
                if (image != null)
                    image.enabled = false;
            }

            // Assuming wing.Elements is an array/list with 1 or 2 WingElementType values.
            if (wing.Elements != null)
            {
                int count = Mathf.Min(imageElements.Length, wing.Elements.Length);

                for (int i = 0; i < count; i++)
                {
                    if (imageElements[i] == null)
                        continue;

                    imageElements[i].sprite = IconManager.Instance.Element.GetIcon(wing.Elements[i]);
                    imageElements[i].enabled = true;
                }
            }
        }

        // Equipped
        if (imageEquipped != null)
            imageEquipped.enabled = wing.IsEquipped();

        // Evolution
        if (imageEvolution != null) 
        {
            if (wing.CurrentEvolution != WingEvolution.None) 
            {
                _ = SetEvolutionAsync(wing.WingEvolutionAddress);
            } else 
            {
                imageEvolution.enabled = false;
            }
        }

        // Name
        if (textName != null)
            textName.text = wing.WingName;

        // Description
        if (textDescription != null)
            textDescription.text = wing.WingDescription;
    }

    public void Clear()
    {
        if (imageIcon != null)
            imageIcon.color = Color.white;

        if (imageElements != null)
        {
            foreach (var image in imageElements)
            {
                if (image != null)
                    image.enabled = false;
            }
        }

        if (imageEquipped != null)
            imageEquipped.enabled = false;

        if (imageEvolution != null)
            imageEvolution.enabled = false;

        if (textName != null)
            textName.text = string.Empty;

        if (textDescription != null)
            textDescription.text = string.Empty;

        _bindingEvolution.Release();
        _bindingEvolution.Cancel();
        _setVersion++;
    }

    #endregion

    #region Addressable

    private async System.Threading.Tasks.Task SetEvolutionAsync(string address)
    {
        int version = ++_setVersion;
        imageEvolution.enabled = false;
        var task = _bindingEvolution.LoadAsync(address);

        var asset = await task;

        if (version != _setVersion) return;

        imageEvolution.sprite = asset;
        imageEvolution.enabled = true;
    }

    #endregion
}
